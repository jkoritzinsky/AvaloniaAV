using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;

namespace AvaloniaAV.MediaFoundation
{
    public partial class CapturePlayer
    {
        //private CaptureEngineClassFactory factory;
        private MediaAttributes captureEngineAttributes = new MediaAttributes();

        public CapturePlayer(Device device)
        {
            using (var manager = new DXGIDeviceManager())
            {
                manager.ResetDevice(device);

                //captureEngineAttributes.Set(CaptureEngineAttributeKeys.D3DManager, manager); 
            }

        }

        partial void StartCaptureCore(SystemCamera camera, CancellationToken token)
        {

            using (var deviceAttributes = new MediaAttributes(2))
            {
                deviceAttributes.Set(CaptureDeviceAttributeKeys.SourceType, CaptureDeviceAttributeKeys.SourceTypeVideoCapture.Guid);
                deviceAttributes.Set(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink, camera.SystemIdentifier);

                MediaFactory.CreateDeviceSource(deviceAttributes, out MediaSource videoSource);

                using (videoSource)
                {
                    // var captureEngine = new CaptureEngine(factory);
                    // captureEngine.Initialize(event => OnEngineEvent(captureEngine, event), factory, attributes, null, videoSource);
                    throw new NotImplementedException();  
                }
            }
        }
    }
}
