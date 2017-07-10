using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer
{
    public interface IPlatformFramebufferPlayer : IDisposable
    {
        void OpenUri(Uri uri);
        void OpenStream(Stream stream, Uri uri);

        IObservable<IFramebufferPlayback> Playback { get; }
    }
}
