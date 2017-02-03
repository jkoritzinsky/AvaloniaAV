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
        private readonly CapturePlayer player;
        private readonly DeviceContext context;

        public CapturePlayback(SharpDX.Direct2D1.Device device, CapturePlayer player)
        {
            this.player = player;
            context = new DeviceContext(device, DeviceContextOptions.EnableMultithreadedOptimizations);

            CurrentFrame = player.CurrentSurface.Select(surface => CreateFrame(surface));
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
        private FrameBitmap lastBitmap;
        private Frame CreateFrame(Surface frameSurface)
        {
            if (lastSurface != frameSurface)
            {
                lastSurface = frameSurface;
                var bitmap = new Bitmap1(context, frameSurface);
                lastBitmap = new FrameBitmap(bitmap);
            }
            return new Frame(lastBitmap);
        }

        public IObservable<Frame> CurrentFrame { get; private set; }

        public void Dispose()
        {
            CurrentFrame = null;
            context.Dispose();
        }
    }
}
