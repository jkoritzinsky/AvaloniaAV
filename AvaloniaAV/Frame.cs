using System;
using Avalonia.Media.Imaging;

namespace AvaloniaAV
{
    public class Frame
    {
        public Frame(Bitmap frameBitmap, TimeSpan time = default(TimeSpan))
        {
            FrameBitmap = frameBitmap;
            Time = time;
        }

        public TimeSpan Time { get; }
        public Bitmap FrameBitmap { get; }
    }
}