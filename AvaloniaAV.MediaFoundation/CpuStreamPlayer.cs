using System;
using System.Collections.Generic;
using System.Text;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation
{
    class CpuStreamPlayer : StreamPlayer
    {
        public CpuStreamPlayer(SharpDX.DXGI.Device device, int fps) : base(device, fps)
        {
        }

        public override Surface Surface => CpuSurface;

        public Surface CpuSurface { get; private set; }

        protected override void OnNewSurface(int x, int y)
        {
            base.OnNewSurface(x, y);
            using (var texture = new Texture2D(Device, new Texture2DDescription
            {
                Format = Format.B8G8R8A8_UNorm,
                Width = x,
                Height = y,
                ArraySize = 1,
                MipLevels = 1,
                SampleDescription = new SampleDescription
                {
                    Count = 1
                },
                CpuAccessFlags = CpuAccessFlags.Read,
                Usage = ResourceUsage.Staging,
            }))
            {
                CpuSurface?.Dispose();
                CpuSurface = texture.QueryInterface<Surface>();
            }
        }

        protected override void UpdateOutputSurface()
        {
            base.UpdateOutputSurface();
            using (var targetResource = TargetSurface.QueryInterface<SharpDX.Direct3D11.Resource>())
            using (var cpuResource = CpuSurface.QueryInterface<SharpDX.Direct3D11.Resource>())
            using (var device3 = Device.QueryInterface<SharpDX.Direct3D11.Device3>())
            {
                Device.ImmediateContext.ResolveSubresource(targetResource, 0, cpuResource, 0, Format.B8G8R8A8_UNorm);
            }
        }
    }
}
