using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace AvaloniaAV.MediaFoundation
{
    class SystemCameraProvider : ISystemCameraProvider
    {
        public Task<IEnumerable<SystemCamera>> Cameras
        {
            get
            {
                async Task<IEnumerable<SystemCamera>> GetCamerasAsync()
                {
                    var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                    return cameraDevices.Select(device => new SystemCamera(device.Name, device.Id));
                }
                return GetCamerasAsync();
            }
        }
    }
}
