using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation
{
    public class AvService
    {
        private Device device;
        public StreamPlayer GetStreamPlayer(int fps = 60)
        {
            InitDevice();

            return new StreamPlayer(device.QueryInterface<SharpDX.DXGI.Device>(), fps);
        }

        public CapturePlayer GetCapturePlayer()
        {
#if NETFX
            InitDevice();
            return new CapturePlayer(device.QueryInterface<SharpDX.DXGI.Device>());
#else
            return new CapturePlayer();
#endif
        }

        private void InitDevice()
        {
            if (device != null) return;

            device = new Device(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport,
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_12_0,
                FeatureLevel.Level_12_1);

            using (var multithread = device.QueryInterface<Multithread>())
            {
                multithread.SetMultithreadProtected(true);
            }
        }
    }
}
