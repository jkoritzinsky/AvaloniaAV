using System;
using Avalonia.Media.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace AvaloniaAV
{
    public class Frame : IDisposable
    {
        public Frame(IRef<IBitmapImpl> frameBitmap, TimeSpan time = default(TimeSpan))
        {
            FrameBitmap = frameBitmap;
            Time = time;
        }

        public Frame(IPlatformRenderInterface renderInterface, Framebuffer.FramebufferFrame frame)
        {
            Time = frame.Time;
            using (var framebuffer = frame.Framebuffer.Lock())
            {
                var writableBitmap = RefCountable.Create(renderInterface.CreateWritableBitmap(framebuffer.Width, framebuffer.Height, framebuffer.Format));
                using (var bitmapBuffer = writableBitmap.Item.Lock())
                {
                    unsafe
                    {
                        for (int i = 0; i < framebuffer.Height; i++)
                        {
                            Unsafe.CopyBlockUnaligned(
                                (byte*)framebuffer.Address + i * framebuffer.RowBytes,
                                (byte*)bitmapBuffer.Address + i * bitmapBuffer.RowBytes,
                                (uint)bitmapBuffer.RowBytes); 
                        } 
                    }
                }
                FrameBitmap = writableBitmap;
            }
        }

        public TimeSpan Time { get; }
        public IRef<IBitmapImpl> FrameBitmap { get; }

        public void Dispose()
        {
            FrameBitmap.Dispose();
        }
    }
}