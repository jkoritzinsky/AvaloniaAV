using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX.MediaFoundation;

namespace AvaloniaAV.MediaFoundation
{
    class SystemCameraProvider : ISystemCameraProvider
    {
        public Task<IEnumerable<SystemCamera>> Cameras
        {
            get
            {
                IEnumerable<SystemCamera> GetCameras()
                {
                    using (var deviceAttributes = new MediaAttributes(1))
                    {
                        deviceAttributes.Set(CaptureDeviceAttributeKeys.SourceType,
                            CaptureDeviceAttributeKeys.SourceTypeVideoCapture.Guid);
                        var sources = MediaFactory.EnumDeviceSources(deviceAttributes);
                        foreach (var source in sources)
                        {
                            yield return
                                new SystemCamera(source.Get(CaptureDeviceAttributeKeys.FriendlyName),
                                    source.Get(CaptureDeviceAttributeKeys.SourceTypeVidcapSymbolicLink));
                            source.Dispose();
                        }
                    }
                }

                return Task.FromResult(GetCameras());
            }
        }
    }
}