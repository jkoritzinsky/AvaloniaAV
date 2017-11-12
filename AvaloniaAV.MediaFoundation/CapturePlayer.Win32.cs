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
        private MediaAttributes captureEngineAttributes = new MediaAttributes();
        private CaptureEngineClassFactory captureFactory = new CaptureEngineClassFactory();

        public CapturePlayer(Device device)
        {
            using (var manager = new DXGIDeviceManager())
            {
                manager.ResetDevice(device);

                captureEngineAttributes.Set(CaptureEngineAttributeKeys.D3DManager, manager);
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
                    var captureEngine = new CaptureEngine(captureFactory);
                    captureEngine.CaptureEngineEvent += evt => OnEngineEvent(captureEngine, evt);
                    captureEngine.Initialize(captureEngineAttributes, null, videoSource);
                }
            }
        }

        private void OnEngineEvent(CaptureEngine engine, MediaEvent mediaEvent)
        {
            throw new NotImplementedException();
        }
    }
}
