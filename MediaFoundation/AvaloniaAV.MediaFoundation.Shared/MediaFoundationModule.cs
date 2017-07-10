using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using SharpDX.MediaFoundation;
using Avalonia.Controls;

namespace Avalonia
{
    public static class AppBuilderExtensions
    {
        public static TAppBuilder AVUseMediaFoundation<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            new AvaloniaAV.MediaFoundation.MediaFoundationModule();
            return builder;
        }
    }
}


namespace AvaloniaAV.MediaFoundation
{
    using AvaloniaAV.Framebuffer;
    using AvaloniaAV.MediaFoundation.Framebuffer;
    using SharpDX.DXGI;

    public class MediaFoundationModule
    {
        internal protected MediaFoundationModule()
        {
            if (AvaloniaLocator.Current.GetService<Device>() == null)
            {
                AvaloniaLocator.CurrentMutable.Bind<Device>().ToConstant(CreateDxgiDevice());
            }

            MediaManager.Startup();
            Application.Current.OnExit += (obj, args) => MediaManager.Shutdown();
#if !REFERENCE
            AvaloniaLocator.CurrentMutable.Bind<ISystemCameraProvider>().ToSingleton<SystemCameraProvider>();
#endif
            AvaloniaLocator.CurrentMutable.Bind<AvService>().ToSingleton<AvService>();

            AvaloniaLocator.CurrentMutable.Bind<IPlatformFramebufferPlayerProvider>().ToSingleton<FramebufferCameraProvider>();
        }

        private Device CreateDxgiDevice()
        {

            var featureLevels = new[]
            {
                    SharpDX.Direct3D.FeatureLevel.Level_11_1,
                    SharpDX.Direct3D.FeatureLevel.Level_11_0,
                    SharpDX.Direct3D.FeatureLevel.Level_10_1,
                    SharpDX.Direct3D.FeatureLevel.Level_10_0,
                    SharpDX.Direct3D.FeatureLevel.Level_9_3,
                    SharpDX.Direct3D.FeatureLevel.Level_9_2,
                    SharpDX.Direct3D.FeatureLevel.Level_9_1,
            };

            using (var d3dDevice = new SharpDX.Direct3D11.Device(
                SharpDX.Direct3D.DriverType.Hardware,
                SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport |
                SharpDX.Direct3D11.DeviceCreationFlags.VideoSupport,
                featureLevels))
            {
                return d3dDevice.QueryInterface<SharpDX.DXGI.Device>();
            }

        }
    }
}
