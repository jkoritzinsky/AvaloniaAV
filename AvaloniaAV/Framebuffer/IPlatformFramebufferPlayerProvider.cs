using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer
{
    public interface IPlatformFramebufferPlayerProvider
    {
        IPlatformFramebufferPlayer CreatePlayer();
        IPlatformCameraFramebufferPlayer CreateCameraPlayer();
    }
}
