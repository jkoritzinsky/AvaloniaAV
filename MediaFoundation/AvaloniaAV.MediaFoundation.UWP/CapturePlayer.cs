using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Capture;
using SharpDX;
using SharpDX.DXGI;

namespace AvaloniaAV.MediaFoundation
{
    public partial class CapturePlayer
    {
        [DllImport("D3D11.dll")]
        private static extern int GetDXGIInterfaceFromObject([MarshalAs(UnmanagedType.IInspectable)] object obj,
            Guid interfaceGuid, out IntPtr interfacePtr);

        private static Surface GetDxgiSurface(IDirect3DSurface wrapperDSurface)
        {
            var hResult = GetDXGIInterfaceFromObject(wrapperDSurface, Utilities.GetGuidFromType(typeof(Surface)), out IntPtr interfacePtr);
            var result = new Result(hResult);
            result.CheckError();
            return new Surface(interfacePtr);
        }

        async partial void StartCaptureCore(SystemCamera camera, CancellationToken token)
        {
            var surfaceSubject = new Subject<Surface>();
            CurrentSurface = surfaceSubject;
            
            using (var capture = new MediaCapture())
            {
                await capture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = camera.SystemIdentifier,
                    SharingMode = MediaCaptureSharingMode.SharedReadOnly
                });

                await capture.StartPreviewAsync();

                while (!token.IsCancellationRequested)
                {
                    var frame = await capture.GetPreviewFrameAsync();

                    var d3DSurface = frame.Direct3DSurface;
                    if (d3DSurface != null)
                    {
                        surfaceSubject.OnNext(GetDxgiSurface(d3DSurface)); 
                    }
                }
                await capture.StopPreviewAsync(); 
            }
        }
    }
}
