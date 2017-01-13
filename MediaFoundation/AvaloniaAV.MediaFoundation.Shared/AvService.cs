using Avalonia;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation
{
    public class AvService
    {
        public AvService()
        {
            using (var multithreaded = AvaloniaLocator.Current.GetService<SharpDX.DXGI.Device>().QueryInterface<DeviceMultithread>())
            {
                multithreaded.SetMultithreadProtected(true);
            }
        }

        public StreamPlayer GetStreamPlayer(int fps = 60)
        {
            return new StreamPlayer(AvaloniaLocator.Current.GetService<SharpDX.DXGI.Device>(), fps);
        }

        public CapturePlayer GetCapturePlayer()
        {
#if NETFX
            return new CapturePlayer(AvaloniaLocator.Current.GetService<SharpDX.DXGI.Device>());
#else
            return new CapturePlayer();
#endif
        }
    }
}
