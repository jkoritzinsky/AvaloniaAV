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

    public class MediaFoundationModule
    {
        internal protected MediaFoundationModule()
        {
            MediaManager.Startup();
            Application.Current.OnExit += (obj, args) => MediaManager.Shutdown();
#if !REFERENCE
            AvaloniaLocator.CurrentMutable.Bind<ISystemCameraProvider>().ToSingleton<SystemCameraProvider>();
#endif
            AvaloniaLocator.CurrentMutable.Bind<AvService>().ToSingleton<AvService>();

            AvaloniaLocator.CurrentMutable.Bind<IPlatformFramebufferPlayerProvider>().ToSingleton<FramebufferCameraProvider>();
        }
    }
}
