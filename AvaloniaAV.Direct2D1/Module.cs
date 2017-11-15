using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using AvaloniaAV.MediaFoundation;
using Avalonia.Controls;

[assembly:ExportAvaloniaModule("AvaloniaAV", typeof(AvaloniaAV.Direct2D1.Module), ForRenderingSubsystem = "Direct2D1")]

namespace Avalonia
{
    public static class AppBuilderExtensions
    {
        public static TAppBuilder AVUseAcceleratedDirect2D<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder: AppBuilderBase<TAppBuilder>, new()
        {
            return builder.AfterSetup(_ => new AvaloniaAV.Direct2D1.Module());
        }
    }
}

namespace AvaloniaAV.Direct2D1
{
    class Module : MediaFoundationModule
    {
        public Module()
        {
            AvaloniaLocator.CurrentMutable.Bind<IPlatformPlayerProvider>().ToSingleton<PlatformPlayerProvider>();
        }
    }
}
