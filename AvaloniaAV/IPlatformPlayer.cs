using System;
using System.IO;

namespace AvaloniaAV
{
    public interface IPlatformPlayer : IDisposable
    {
        void OpenUri(Uri uri);
        void OpenStream(Stream stream, Uri uri);
        void OpenCamera(SystemCamera camera);

        IObservable<IPlayback> Playback { get; }
    }
}