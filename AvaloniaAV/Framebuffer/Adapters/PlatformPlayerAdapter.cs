using System;
using System.IO;
using System.Reactive.Linq;

namespace AvaloniaAV.Framebuffer.Adapters
{
    internal class PlatformPlayerAdapter : IPlatformPlayer
    {
        private IPlatformFramebufferPlayer platformFramebufferPlayer;

        public PlatformPlayerAdapter(IPlatformFramebufferPlayer platformFramebufferPlayer)
        {
            this.platformFramebufferPlayer = platformFramebufferPlayer;
        }

        public IObservable<IControllablePlayback> Playback => platformFramebufferPlayer.Playback.Select(playback => new PlaybackAdapter(playback));

        public void Dispose() => platformFramebufferPlayer.Dispose();

        public void OpenStream(Stream stream, Uri uri) => platformFramebufferPlayer.OpenStream(stream, uri);

        public void OpenUri(Uri uri) => platformFramebufferPlayer.OpenUri(uri);
    }
}