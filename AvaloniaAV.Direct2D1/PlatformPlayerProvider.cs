using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaAV.MediaFoundation;

namespace AvaloniaAV.Direct2D1
{
    class PlatformPlayerProvider : IPlatformPlayerProvider
    {
        public IPlatformPlayer CreatePlayer()
        {
            return new PlatformPlayer(AvaloniaLocator.Current.GetService<AvService>(),
                AvaloniaLocator.Current.GetService<SharpDX.Direct2D1.Device>(),
                AvaloniaLocator.Current.GetService<SharpDX.WIC.ImagingFactory>());
        }

        public IPlatformCameraPlayer CreateCameraPlayer()
        {
            return new CameraPlayer(AvaloniaLocator.Current.GetService<AvService>(),
                AvaloniaLocator.Current.GetService<SharpDX.Direct2D1.Device>(),
                AvaloniaLocator.Current.GetService<SharpDX.WIC.ImagingFactory>());
        }
    }
}
