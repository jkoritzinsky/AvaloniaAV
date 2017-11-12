using AvaloniaAV.Framebuffer;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using SharpDX.DXGI;
using Avalonia;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation.Framebuffer
{
    class FramebufferStreamPlayback : IFramebufferPlayback
    {
        private StreamPlayer underlyingPlayer;
        private TimeSpan? duration;

        public FramebufferStreamPlayback(StreamPlayer underlyingPlayer, TimeSpan? duration)
        {
            this.underlyingPlayer = underlyingPlayer;
            this.duration = duration;
            CurrentFrame = underlyingPlayer.CurrentTime.Select(time => SetNewFrame(time, underlyingPlayer.Surface));
        }

        private FramebufferFrame SetNewFrame(TimeSpan time, Surface cpuSurface)
        {
            return new FramebufferFrame(new FramebufferPlatformSurface(cpuSurface), time);
        }

        public IObservable<FramebufferFrame> CurrentFrame { get; }

        public TimeSpan? Duration => duration;

        public void Pause()
        {
            underlyingPlayer.Pause();
        }

        public void Play()
        {
            underlyingPlayer.Play();
        }

        public void Seek(TimeSpan time)
        {
            underlyingPlayer.Seek(time);
        }

        public void Dispose()
        {
        }
    }
}
