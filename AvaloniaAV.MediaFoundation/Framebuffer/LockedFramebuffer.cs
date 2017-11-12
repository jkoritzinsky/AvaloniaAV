using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Platform;
using Avalonia;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation.Framebuffer
{
    class LockedFramebuffer : ILockedFramebuffer
    {
        private Surface surface;

        public LockedFramebuffer(Surface surface)
        {
            this.surface = surface;
            var rect = this.surface.Map(SharpDX.DXGI.MapFlags.Read);
            Address = rect.DataPointer;
            RowBytes = rect.Pitch;
            using (var texture = this.surface.QueryInterface<Texture2D>())
            {
                var description = texture.Description;
                Width = description.Width;
                Height = description.Height;
                Dpi = new Vector(96, 96);
                Format = description.Format.ToAvalonia();
            }
        }

        public IntPtr Address { get; }

        public int Width { get; }

        public int Height { get; }

        public int RowBytes { get; }

        public Vector Dpi { get; }

        public PixelFormat Format { get; }

        public void Dispose()
        {
            surface.Unmap();
        }
    }
}
