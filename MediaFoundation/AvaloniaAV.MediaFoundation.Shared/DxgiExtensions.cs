using Avalonia.Platform;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaAV.MediaFoundation
{
    static class DxgiExtensions
    {
        public static PixelFormat ToAvalonia(this Format format)
        {
            switch (format)
            {
                case Format.B8G8R8A8_Typeless:
                case Format.B8G8R8A8_UNorm:
                case Format.B8G8R8A8_UNorm_SRgb:
                    return PixelFormat.Bgra8888;
                case Format.R8G8B8A8_SInt:
                case Format.R8G8B8A8_SNorm:
                case Format.R8G8B8A8_Typeless:
                case Format.R8G8B8A8_UInt:
                case Format.R8G8B8A8_UNorm:
                case Format.R8G8B8A8_UNorm_SRgb:
                    return PixelFormat.Rgba8888;
                default:
                    throw new ArgumentException(nameof(format));
            }
        }
    }
}
