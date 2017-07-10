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
            CurrentFrame = underlyingPlayer.CurrentTime.Select(time => SetNewFrame(time, underlyingPlayer.Device, underlyingPlayer.Surface));
        }


        private Surface lastGpuSurface;
        private Texture2D cpuTexture;
        private FramebufferFrame SetNewFrame(TimeSpan time, SharpDX.DXGI.Device device, Surface gpuSurface)
        {
            using (var d3DDevice = device.QueryInterface<SharpDX.Direct3D11.Device>())
            {
                if (lastGpuSurface != gpuSurface)
                {
                    cpuTexture?.Dispose();
                    cpuTexture = new Texture2D(d3DDevice, new Texture2DDescription
                    {
                        Format = gpuSurface.Description.Format,
                        Width = gpuSurface.Description.Width,
                        Height = gpuSurface.Description.Height,
                        ArraySize = 1,
                        MipLevels = 1,
                        SampleDescription = new SampleDescription
                        {
                            Count = 1
                        },
                        CpuAccessFlags = CpuAccessFlags.Read,
                        Usage = ResourceUsage.Staging,
                    });
                    lastGpuSurface = gpuSurface;
                }

                using (var context = new DeviceContext(d3DDevice))
                using (var gpuTexture = gpuSurface.QueryInterface<Texture2D>())
                {
                    context.CopyResource(gpuTexture, cpuTexture);
                }
            }

            return new FramebufferFrame(new FramebufferPlatformSurface(cpuTexture.QueryInterface<Surface>()), time);
        }

        public IObservable<FramebufferFrame> CurrentFrame { get; }

        public TimeSpan? Duration => duration;

        public void Dispose()
        {
            cpuTexture?.Dispose();
        }

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
    }
}
