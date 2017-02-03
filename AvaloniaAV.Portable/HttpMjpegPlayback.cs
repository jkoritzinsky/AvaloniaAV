using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.Reactive.Subjects;

namespace AvaloniaAV.Portable
{
    public class HttpMjpegPlayback : IControllablePlayback
    {
        public static readonly IEnumerable<string> SupportedSchemes = new List<string>
        {
            "http",
            "https",
            "mjpeg"
        };

        // magic 2 byte header for JPEG images
        private readonly byte[] JpegHeader = { 0xff, 0xd8 };

        // pull down 1024 bytes at a time
        private const int ChunkSize = 1024;

        private Uri stream;
        private CancellationTokenSource tokenSource;

        public TimeSpan? Duration => null;

        private Subject<Frame> frameSubject = new Subject<Frame>();
        public IObservable<Frame> CurrentFrame => frameSubject;

        public HttpMjpegPlayback(Uri streamUrl, CancellationTokenSource tokenSource = null)
        {
            stream = streamUrl;
            this.tokenSource = tokenSource ?? new CancellationTokenSource();
            Task.Run(() => StartStream(this.tokenSource.Token), tokenSource.Token);
        }

        private async Task StartStream(CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(stream, HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType;
                
                if (contentType == null || contentType.MediaType != "multipart/x-mixed-replace")
                    throw new InvalidOperationException("Invalid content-type header.");

                string boundary = contentType.Parameters.FirstOrDefault(param => param.Name == "boundary")?.Value;

                if(boundary == null)
                    throw new InvalidOperationException("Invalid content-type header.");

                byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

                using (var imageStream = await response.Content.ReadAsStreamAsync())
                using (var br = new BinaryReader(imageStream))
                {
                    try
                    {
                        byte[] buff = br.ReadBytes(ChunkSize);

                        while (!token.IsCancellationRequested)
                        {
                            using (var frameStream = new MemoryStream())
                            {

                                // find the JPEG header
                                int imageStart = buff.FindSubArray(JpegHeader);

                                if (imageStart != -1)
                                {
                                    // copy the start of the JPEG image to the imageBuffer
                                    int size = buff.Length - imageStart;

                                    frameStream.Write(buff, imageStart, size);

                                    while (true)
                                    {
                                        buff = br.ReadBytes(ChunkSize);

                                        // find the boundary text
                                        int imageEnd = buff.FindSubArray(boundaryBytes);
                                        if (imageEnd == -1)
                                        {
                                            // copy all of the data to the imageBuffer
                                            frameStream.Write(buff, 0, buff.Length);
                                            size += buff.Length;
                                        }
                                        else
                                        {
                                            frameStream.Write(buff, 0, imageEnd);
                                            size += imageEnd;

                                            frameStream.Seek(0, SeekOrigin.Begin);
                                            frameSubject.OnNext(new Frame(new Bitmap(frameStream)));

                                            // copy the leftover data to the start
                                            Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

                                            // fill the remainder of the buffer with new data and start over
                                            byte[] temp = br.ReadBytes(imageEnd);

                                            Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void Play()
        {
        }

        public void Pause()
        {
        }

        public void Seek(TimeSpan span)
        {
        }

        public void Dispose()
        {
            tokenSource.Cancel();
        }
    }
}
