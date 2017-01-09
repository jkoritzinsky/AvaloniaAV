using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.MediaFoundation;
using Device = SharpDX.DXGI.Device;

namespace AvaloniaAV.MediaFoundation
{
    public class StreamPlayer : IDisposable
    {
        private readonly Device device;
        private readonly MediaEngineEx engine;
        private readonly TimeSpan frameTime;
        private Task frameLoopTask;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public StreamPlayer(Device device, int fps)
        {
            this.device = device;
            using (var manager = new DXGIDeviceManager())
            using (var factory = new MediaEngineClassFactory())
            using (var attributes = new MediaEngineAttributes(2))
            {
                manager.ResetDevice(device);
                attributes.DxgiManager = manager;
                attributes.VideoOutputFormat = (int)Format.R32G32B32A32_Float;
                using (var baseEngine = new MediaEngine(factory, attributes, playbackCallback: OnEngineEvent))
                {
                    engine = baseEngine.QueryInterface<MediaEngineEx>();
                }
            }

            frameTime = TimeSpan.FromSeconds(1.0 / fps);
            currentState.OnNext(StreamPlayerState.NoSource);
        }

        private void OnEngineEvent(MediaEngineEvent mediaevent, long param1, int param2)
        {
            switch (mediaevent)
            {
                case MediaEngineEvent.DurationChange:
                    duration.OnNext(TimeSpan.FromSeconds(engine.Duration));
                    break;
                case MediaEngineEvent.LoadStart:
                    currentState.OnNext(StreamPlayerState.LoadingSource);
                    break;
                case MediaEngineEvent.LoadedMetadata:
                    int x, y;
                    duration?.OnNext(TimeSpan.FromSeconds(engine.Duration));
                    engine.GetNativeVideoSize(out x, out y);
                    using (var d3DDevice = device.QueryInterface<SharpDX.Direct3D11.Device>())
                    using (var texture = new Texture2D(d3DDevice, new Texture2DDescription
                    {
                        Format = Format.R32G32B32A32_Float,
                        Width = x,
                        Height = y
                    }))
                    {
                        Surface?.Dispose();
                        Surface = texture.QueryInterface<Surface>();
                    } 
                    break;
                case MediaEngineEvent.CanPlay:
                    currentState.OnNext(StreamPlayerState.CanPlay);
                    break;
                case MediaEngineEvent.CanPlayThrough:
                    currentState.OnNext(StreamPlayerState.CanPlayFully);
                    break;
            }
        }

        public void Open(Uri uri)
        {
            tokenSource.Cancel();
            frameLoopTask = null;
            engine.Source = uri.ToString();
            engine.Load();
            tokenSource = new CancellationTokenSource();
        }

        // Bug in SharpDX makes the ByteStream constructor unusable from PCLs
        //public void Open(Stream stream, Uri uri)
        //{
        //    tokenSource.Cancel();
        //    frameLoopTask = null;
        //    engine.SetSourceFromByteStream(new ByteStream(stream), uri.ToString());
        //    engine.Load();
        //    tokenSource = new CancellationTokenSource();
        //}

        public void Play()
        {
            if (frameLoopTask == null)
            {
                frameLoopTask = Task.Run(() => FrameLoopAsync(tokenSource.Token), tokenSource.Token);
            }
            engine.Play();
        }

        public void Pause()
        {
            engine.Pause();
        }

        public void Seek(TimeSpan time)
        {
            engine.CurrentTime = time.TotalSeconds;
        }

        private async Task FrameLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (engine.OnVideoStreamTick(out long time))
                {
                    engine.GetNativeVideoSize(out int width, out int height);
                    engine.TransferVideoFrame(Surface, null, new RawRectangle(0, 0, width, height), null);
                    currentTime.OnNext(new TimeSpan(time));
                }
                await Task.Delay(frameTime, token);
            }
        }

        private readonly Subject<TimeSpan> currentTime = new Subject<TimeSpan>();
        public IObservable<TimeSpan> CurrentTime => currentTime;

        private readonly Subject<TimeSpan> duration = new Subject<TimeSpan>();
        public IObservable<TimeSpan> Duration => duration;

        private readonly Subject<StreamPlayerState> currentState = new Subject<StreamPlayerState>();
        public IObservable<StreamPlayerState> CurrentState => currentState;

        public Surface Surface { get; private set; }

        public void Dispose()
        {
            engine.Shutdown();
            engine.Dispose();
            Surface.Dispose();
        }
    }
}
