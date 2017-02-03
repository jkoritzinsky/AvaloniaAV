using System;
using Avalonia.Media.Imaging;

namespace AvaloniaAV
{
    public class Frame
    {
        public Frame(IBitmap frameBitmap, TimeSpan time = default(TimeSpan))
        {
            FrameBitmap = frameBitmap;
            Time = time;
        }

        public TimeSpan Time { get; }
        public IBitmap FrameBitmap { get; }
    }
}