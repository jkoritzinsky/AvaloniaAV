using Avalonia;
using AvaloniaAV.Framebuffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaAV.MediaFoundation.Framebuffer
{
    class FramebufferCameraProvider : IPlatformFramebufferPlayerProvider
    {
        public IPlatformCameraFramebufferPlayer CreateCameraPlayer()
        {
            throw new NotImplementedException();
        }

        public IPlatformFramebufferPlayer CreatePlayer() => new PlatformFramebufferPlayer(AvaloniaLocator.Current.GetService<AvService>());
    }
}
