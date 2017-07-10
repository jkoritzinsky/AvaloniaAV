using System;
using Avalonia.Media.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace AvaloniaAV
{
    public class Frame
    {
        public Frame(IBitmap frameBitmap, TimeSpan time = default(TimeSpan))
        {
            FrameBitmap = frameBitmap;
            Time = time;
        }

        public Frame(Framebuffer.FramebufferFrame frame)
        {
            Time = frame.Time;
            using (var framebuffer = frame.Framebuffer.Lock())
            {
                var writableBitmap = new WritableBitmap(framebuffer.Width, framebuffer.Height, framebuffer.Format);
                using (var bitmapBuffer = writableBitmap.Lock())
                {
                    unsafe
                    {
                        for (int i = 0; i < framebuffer.Height; i++)
                        {
                            Unsafe.CopyBlock((byte*)framebuffer.Address + i * framebuffer.RowBytes, (byte*)bitmapBuffer.Address + i * bitmapBuffer.RowBytes, (uint)bitmapBuffer.RowBytes); 
                        } 
                    }
                }
                FrameBitmap = writableBitmap;
            }
        }

        public TimeSpan Time { get; }
        public IBitmap FrameBitmap { get; }
    }
}