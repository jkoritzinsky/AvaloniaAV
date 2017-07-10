using System;
using System.Reactive.Linq;

namespace AvaloniaAV.Framebuffer.Adapters
{
    internal class PlatformCameraAdapter : IPlatformCameraPlayer
    {
        private IPlatformCameraFramebufferPlayer platformCameraFramebufferPlayer;

        public PlatformCameraAdapter(IPlatformCameraFramebufferPlayer platformCameraFramebufferPlayer)
        {
            this.platformCameraFramebufferPlayer = platformCameraFramebufferPlayer;
        }

        public IObservable<IPlayback> Playback => platformCameraFramebufferPlayer.Playback.Select(playback => new PlaybackAdapter(playback));

        public void OpenCamera(SystemCamera camera) => platformCameraFramebufferPlayer.OpenCamera(camera);
    }
}