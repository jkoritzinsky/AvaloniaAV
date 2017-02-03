﻿using System;
using System.Reactive.Linq;
using AvaloniaAV.MediaFoundation;
using SharpDX.Direct2D1;
using SharpDX.DXGI;

namespace AvaloniaAV.Direct2D1
{
    class StreamPlayback : IControllablePlayback
    {
        private readonly StreamPlayer player;
        private readonly DeviceContext context;

        public StreamPlayback(SharpDX.Direct2D1.Device device, StreamPlayer player, TimeSpan? duration)
        {
            this.player = player;
            context = new DeviceContext(device, DeviceContextOptions.EnableMultithreadedOptimizations);
            Duration = duration;

            CurrentFrame = player.CurrentTime.Select(time => CreateFrame(player.Surface, time));
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
        private FrameBitmap lastBitmap;
        private Frame CreateFrame(Surface frameSurface, TimeSpan frameTime)
        {
            if (lastSurface != frameSurface)
            {
                lastSurface = frameSurface;
                var bitmap = new Bitmap1(context, frameSurface);
                lastBitmap = new FrameBitmap(bitmap);
            }
            return new Frame(lastBitmap, frameTime);
        }

        public IObservable<Frame> CurrentFrame { get; private set; }

        public void Dispose()
        {
            CurrentFrame = null;
            context.Dispose();
        }
    }
}