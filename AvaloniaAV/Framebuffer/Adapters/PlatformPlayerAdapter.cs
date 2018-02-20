using Avalonia.Platform;
using System;
using System.IO;
using System.Reactive.Linq;

namespace AvaloniaAV.Framebuffer.Adapters
{
    internal class PlatformPlayerAdapter : IPlatformPlayer
    {
        private readonly IPlatformRenderInterface renderInterface;
        private IPlatformFramebufferPlayer platformFramebufferPlayer;

        public PlatformPlayerAdapter(IPlatformRenderInterface renderInterface, IPlatformFramebufferPlayer platformFramebufferPlayer)
        {
            this.platformFramebufferPlayer = platformFramebufferPlayer;
            this.renderInterface = renderInterface;
        }

        public IObservable<IControllablePlayback> Playback => platformFramebufferPlayer.Playback.Select(playback => new PlaybackAdapter(renderInterface, playback));

        public void Dispose() => platformFramebufferPlayer.Dispose();

        public void OpenStream(Stream stream, Uri uri) => platformFramebufferPlayer.OpenStream(stream, uri);

        public void OpenUri(Uri uri) => platformFramebufferPlayer.OpenUri(uri);
    }
}