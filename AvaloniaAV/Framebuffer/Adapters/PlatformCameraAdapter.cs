using Avalonia.Platform;
using System;
using System.Reactive.Linq;

namespace AvaloniaAV.Framebuffer.Adapters
{
    internal class PlatformCameraAdapter : IPlatformCameraPlayer
    {
        private readonly IPlatformRenderInterface renderInterface;
        private IPlatformCameraFramebufferPlayer platformCameraFramebufferPlayer;

        public PlatformCameraAdapter(IPlatformRenderInterface renderInterface, IPlatformCameraFramebufferPlayer platformCameraFramebufferPlayer)
        {
            this.platformCameraFramebufferPlayer = platformCameraFramebufferPlayer;
            this.renderInterface = renderInterface;
        }

        public IObservable<IPlayback> Playback => platformCameraFramebufferPlayer.Playback.Select(playback => new PlaybackAdapter(renderInterface, playback));

        public void OpenCamera(SystemCamera camera) => platformCameraFramebufferPlayer.OpenCamera(camera);
    }
}