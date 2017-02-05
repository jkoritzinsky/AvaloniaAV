using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;

namespace AvaloniaAV.Portable
{
    internal class PlatformPlayer : IPlatformPlayer
    {
        private Subject<IControllablePlayback> playbackSubject = new Subject<IControllablePlayback>();
        public IObservable<IControllablePlayback> Playback => playbackSubject;

        public void Dispose()
        {
            playbackSubject.Dispose();
        }

        public void OpenStream(Stream stream, Uri uri)
        {
            throw new NotSupportedException("No support for playing files from a resource stream");
        }

        public void OpenUri(Uri uri)
        {
            if (HttpMjpegPlayback.SupportedSchemes.Contains(uri.Scheme))
            {
                playbackSubject.OnNext(new HttpMjpegPlayback(uri));
            }
            else
            {
                throw new NotSupportedException("No support for playing files of this type or scheme");
            }
        }
    }
}