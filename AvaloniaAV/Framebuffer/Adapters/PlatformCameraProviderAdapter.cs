using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer.Adapters
{
    class PlatformCameraProviderAdapter : IPlatformPlayerProvider
    {
        private readonly IPlatformRenderInterface renderInterface;
        private readonly IPlatformFramebufferPlayerProvider provider;

        public PlatformCameraProviderAdapter(IPlatformRenderInterface renderInterface, IPlatformFramebufferPlayerProvider playerProvider)
        {
            provider = playerProvider;
            this.renderInterface = renderInterface;
        }

        public IPlatformCameraPlayer CreateCameraPlayer() => new PlatformCameraAdapter(renderInterface, provider.CreateCameraPlayer());

        public IPlatformPlayer CreatePlayer() => new PlatformPlayerAdapter(renderInterface, provider.CreatePlayer());
    }
}
