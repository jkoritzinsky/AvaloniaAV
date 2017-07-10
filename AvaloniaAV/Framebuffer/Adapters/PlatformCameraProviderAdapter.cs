using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer.Adapters
{
    class PlatformCameraProviderAdapter : IPlatformPlayerProvider
    {
        private readonly IPlatformFramebufferPlayerProvider provider;

        public PlatformCameraProviderAdapter(IPlatformFramebufferPlayerProvider playerProvider)
        {
            provider = playerProvider;
        }

        public IPlatformCameraPlayer CreateCameraPlayer() => new PlatformCameraAdapter(provider.CreateCameraPlayer());

        public IPlatformPlayer CreatePlayer() => new PlatformPlayerAdapter(provider.CreatePlayer());
    }
}
