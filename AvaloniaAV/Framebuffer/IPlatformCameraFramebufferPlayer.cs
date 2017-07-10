using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer
{
    public interface IPlatformCameraFramebufferPlayer
    {
        void OpenCamera(SystemCamera camera);

        IObservable<IFramebufferPlayback> Playback { get; }
    }
}
