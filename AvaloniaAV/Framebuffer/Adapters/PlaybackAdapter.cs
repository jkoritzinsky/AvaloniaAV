using Avalonia.Platform;
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
        private readonly IPlatformRenderInterface renderInterface;
        private readonly IFramebufferPlayback framebufferPlayback;

        public PlaybackAdapter(IPlatformRenderInterface renderInterface, IFramebufferPlayback playback)
        {
            framebufferPlayback = playback;
            this.renderInterface = renderInterface;
        }

        public TimeSpan? Duration => framebufferPlayback.Duration;

        public IObservable<Frame> CurrentFrame
            => framebufferPlayback.CurrentFrame.Select(frame => new Frame(renderInterface, frame)).DisposeCurrentOnNext();

        public void Dispose() => framebufferPlayback.Dispose();

        public void Pause() => framebufferPlayback.Pause();

        public void Play() => framebufferPlayback.Play();

        public void Seek(TimeSpan time) => framebufferPlayback.Seek(time);
    }
}
