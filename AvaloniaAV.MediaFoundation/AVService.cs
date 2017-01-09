using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace AvaloniaAV.MediaFoundation
{
    public class AvService
    {
        public StreamPlayer GetStreamPlayer(int fps)
        {
            using (var device = new Device(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport,
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_12_0,
                FeatureLevel.Level_12_1))
            using (var multithread = device.QueryInterface<Multithread>())
            {
                multithread.SetMultithreadProtected(true);
                return new StreamPlayer(device.QueryInterface<SharpDX.DXGI.Device>(), fps);
            }
        }
    }
}
