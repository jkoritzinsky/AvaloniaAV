using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Avalonia;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace AvaloniaAV.MediaFoundation
{
    public partial class CapturePlayer
    {
        [DllImport("D3D11.dll")]
        private static extern int GetDXGIInterfaceFromObject([MarshalAs(UnmanagedType.IInspectable)] object obj,
            Guid interfaceGuid, out IntPtr interfacePtr);

        private readonly Subject<Surface> surfaceSubject = new Subject<Surface>();
        public CapturePlayer()
        {
            CurrentSurface = surfaceSubject;
        }

        private static Surface CreatePresentedSurfaceForCurrentCapture(int width, int height)
        {
            var device = AvaloniaLocator.Current.GetService<SharpDX.DXGI.Device>();
            using (var d3D11Device = device.QueryInterface<SharpDX.Direct3D11.Device>())
            using (var texture = new Texture2D(d3D11Device,
                new Texture2DDescription
                {
                    Width = width,
                    Height = height,
                    Format = Format.B8G8R8A8_UNorm,
                    CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
                }))

            {
                return texture.QueryInterface<Surface>();
            }
        }

        private static Surface GetDxgiSurface(IDirect3DSurface wrapperDSurface)
        {
            var hResult = GetDXGIInterfaceFromObject(wrapperDSurface, Utilities.GetGuidFromType(typeof(Surface)), out IntPtr interfacePtr);
            var result = new Result(hResult);
            result.CheckError();
            return new Surface(interfacePtr);
        }

        async partial void StartCaptureCore(SystemCamera camera, CancellationToken token)
        {
            using (var capture = new MediaCapture())
            {
                await capture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = camera.SystemIdentifier,
                    SharingMode = MediaCaptureSharingMode.SharedReadOnly
                }).AsTask(token).ConfigureAwait(false);

                await capture.StartPreviewAsync().AsTask(token).ConfigureAwait(false);

                Surface surfaceToRender = null;
                while (!token.IsCancellationRequested)
                {   
                    var frame = await capture.GetPreviewFrameAsync().AsTask(token).ConfigureAwait(false);

                    var bitmap = frame.SoftwareBitmap;
                    if (bitmap != null)
                    {
                        RenderSoftwareBitmapToSurface(bitmap, ref surfaceToRender);
                    }

                    var d3DSurface = frame.Direct3DSurface;
                    if (d3DSurface != null)
                    {
                        RenderD3DSurface(d3DSurface, ref surfaceToRender);
                    }
                    if (surfaceToRender != null)
                    {
                        surfaceSubject.OnNext(surfaceToRender); 
                    }
                }
                await capture.StopPreviewAsync().AsTask(token).ConfigureAwait(false); 
            }
        }

        private static void RenderD3DSurface(IDirect3DSurface d3DSurface, ref Surface surfaceToRender)
        {
            using (d3DSurface)
            {
                // TODO: Support more BGRA formats
                Debug.Assert(d3DSurface.Description.Format == DirectXPixelFormat.B8G8R8A8UIntNormalized);
                if (surfaceToRender == null)
                {
                    surfaceToRender = CreatePresentedSurfaceForCurrentCapture(d3DSurface.Description.Width,
                        d3DSurface.Description.Height);
                }
                using (var frameSurface = GetDxgiSurface(d3DSurface))
                {
                    // TODO: Copy within the GPU, not the CPU
                    frameSurface.Map(SharpDX.DXGI.MapFlags.Read, out DataStream frameStream);
                    surfaceToRender.Map(SharpDX.DXGI.MapFlags.Write, out DataStream surfaceStream);
                    using (frameStream)
                    using (surfaceStream)
                    {
                        frameStream.CopyTo(surfaceStream);
                    }

                    frameSurface.Unmap();
                    surfaceToRender.Unmap();
                }
            }
        }

        private static void RenderSoftwareBitmapToSurface(SoftwareBitmap bitmap, ref Surface surfaceToRender)
        {
            using (bitmap)
            {
                var bgraBitmap = bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8
                    ? bitmap
                    : SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8);
                try
                {
                    if (surfaceToRender == null)
                    {
                        surfaceToRender = CreatePresentedSurfaceForCurrentCapture(bgraBitmap.PixelWidth,
                            bgraBitmap.PixelHeight);
                    }
                    var rect = surfaceToRender.Map(SharpDX.DXGI.MapFlags.Write, out DataStream surfaceStream);
                    var buffer = new Windows.Storage.Streams.Buffer((uint) (bgraBitmap.PixelHeight * rect.Pitch));
                    bgraBitmap.CopyToBuffer(buffer);
                    
                    using (var bufferStream = buffer.AsStream())
                    using(surfaceStream)
                    {
                        bufferStream.CopyTo(surfaceStream);
                    }

                    surfaceToRender.Unmap();
                }
                finally
                {
                    if (bgraBitmap != bitmap)
                    {
                        bgraBitmap.Dispose();
                    }
                }
            }
        }
    }
}
