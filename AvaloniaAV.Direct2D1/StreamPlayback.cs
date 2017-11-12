using System;
using System.Reactive.Linq;
using AvaloniaAV.MediaFoundation;
using SharpDX.Direct2D1;
using SharpDX.DXGI;

namespace AvaloniaAV.Direct2D1
{
    class StreamPlayback : IControllablePlayback
    {
        private readonly SharpDX.WIC.ImagingFactory factory;
        private readonly StreamPlayer player;
        private readonly DeviceContext context;

        public StreamPlayback(SharpDX.Direct2D1.Device device, SharpDX.WIC.ImagingFactory factory, StreamPlayer player, TimeSpan? duration)
        {
            this.player = player;
            context = new DeviceContext(device, DeviceContextOptions.EnableMultithreadedOptimizations);
            Duration = duration;

            CurrentFrame = player.CurrentTime.Select(time => CreateFrame(player.Surface, time));
            this.factory = factory;
        }

        public TimeSpan? Duration { get; }
        public void Play()
        {
            player.Play();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Seek(TimeSpan span)
        {
            player.Seek(span);
        }


        private Surface lastSurface;
        private Bitmap1 lastSurfaceBitmap;
        private Frame CreateFrame(Surface frameSurface, TimeSpan frameTime)
        {
            if (lastSurface != frameSurface)
            {
                lastSurface = frameSurface;
                var surfaceBitmap = new Bitmap1(context, frameSurface);
                lastSurfaceBitmap = surfaceBitmap;
            }
            var frameBitmap = new Bitmap(context, lastSurfaceBitmap.PixelSize,
                new BitmapProperties(lastSurfaceBitmap.PixelFormat,
                    lastSurfaceBitmap.DotsPerInch.Width,
                    lastSurfaceBitmap.DotsPerInch.Height));
            frameBitmap.CopyFromBitmap(lastSurfaceBitmap);
            return new Frame(new FrameBitmap(factory, frameBitmap), frameTime);
        }

        public IObservable<Frame> CurrentFrame { get; private set; }

        public void Dispose()
        {
            CurrentFrame = null;
            context.Dispose();
        }
    }
}
