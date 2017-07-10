using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Framebuffer
{
    class PlaybackAdapter : IControllablePlayback
    {
        private readonly IFramebufferPlayback framebufferPlayback;

        public PlaybackAdapter(IFramebufferPlayback playback)
        {
            framebufferPlayback = playback;
        }

        public TimeSpan? Duration => framebufferPlayback.Duration;

        public IObservable<Frame> CurrentFrame => framebufferPlayback.CurrentFrame.Select(frame => new Frame(frame));

        public void Dispose() => framebufferPlayback.Dispose();

        public void Pause() => framebufferPlayback.Pause();

        public void Play() => framebufferPlayback.Play();

        public void Seek(TimeSpan time) => framebufferPlayback.Seek(time);
    }
}
