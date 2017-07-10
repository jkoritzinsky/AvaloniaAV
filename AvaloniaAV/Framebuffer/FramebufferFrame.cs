using Avalonia.Controls.Platform.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer
{
    public class FramebufferFrame
    {
        public FramebufferFrame(IFramebufferPlatformSurface framebuffer, TimeSpan time = default(TimeSpan))
        {
            Framebuffer = framebuffer;
            Time = time;
        }

        public TimeSpan Time { get; }
        public IFramebufferPlatformSurface Framebuffer { get; }
    }
}
