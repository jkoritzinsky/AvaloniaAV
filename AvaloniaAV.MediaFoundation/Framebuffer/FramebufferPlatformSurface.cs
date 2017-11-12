using Avalonia.Controls.Platform.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Platform;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation.Framebuffer
{
    class FramebufferPlatformSurface : IFramebufferPlatformSurface
    {
        private Surface surface;

        public FramebufferPlatformSurface(Surface surface)
        {
            this.surface = surface;
        }

        public ILockedFramebuffer Lock()
        {
            return new LockedFramebuffer(surface);
        }
    }
}
