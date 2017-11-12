using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.Platform;
using Serilog;

namespace RenderTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SharpDX.Configuration.EnableReleaseOnFinalizer = true;

            AppBuilder.Configure<App>()
                .UseWin32()
                .UseDirect2D1()
                //.AVUseMediaFoundation().UseAvaloniaAV()
                .AVUseAcceleratedDirect2D()
                .UseAvaloniaAVStyles()
                .Start<MainWindow>();
        }
    }
}
