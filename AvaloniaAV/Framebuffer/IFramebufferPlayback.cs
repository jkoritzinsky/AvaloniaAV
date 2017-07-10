using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer
{
    public interface IFramebufferPlayback : IDisposable
    {
        void Play();
        void Pause();
        void Seek(TimeSpan time);

        IObservable<FramebufferFrame> CurrentFrame { get; }

        TimeSpan? Duration { get; }
    }
}
