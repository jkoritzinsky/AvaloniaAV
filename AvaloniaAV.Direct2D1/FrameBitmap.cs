using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Direct2D1.Media;
using SharpDX.DXGI;

namespace AvaloniaAV.Direct2D1
{
    class FrameBitmap : Avalonia.Media.Imaging.Bitmap
    {
        public FrameBitmap(SharpDX.Direct2D1.Bitmap bitmap)
            :base(new D2DBitmapImpl(bitmap))
        {
        }
    }
}
