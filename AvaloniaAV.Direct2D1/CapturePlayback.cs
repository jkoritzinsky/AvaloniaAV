using AvaloniaAV.MediaFoundation;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Direct2D1
{
    class CapturePlayback : IPlayback
    {
        private readonly SharpDX.WIC.ImagingFactory factory;
        private readonly CapturePlayer player;
        private readonly DeviceContext context;

        public CapturePlayback(SharpDX.Direct2D1.Device device, SharpDX.WIC.ImagingFactory factory, CapturePlayer player)
        {
            this.player = player;
            context = new DeviceContext(device, DeviceContextOptions.EnableMultithreadedOptimizations);

            CurrentFrame = player.CurrentSurface.Select(surface => CreateFrame(surface));
            this.factory = factory;
        }

        public TimeSpan? Duration => null;

        public void Play()
        {
        }

        public void Pause()
        {
        }

        public void Seek(TimeSpan span)
        {
        }


        private Surface lastSurface;
        private Bitmap1 lastSurfaceBitmap;
        private Frame CreateFrame(Surface frameSurface)
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
            return new Frame(new FrameBitmap(factory, frameBitmap));
        }

        public IObservable<Frame> CurrentFrame { get; private set; }

        public void Dispose()
        {
            CurrentFrame = null;
            context.Dispose();
        }
    }
}
