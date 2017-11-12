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
            InitializeLogging();

            AppBuilder.Configure<App>()
                .UseWin32()
                .UseDirect2D1()
                //.AVUseMediaFoundation().UseAvaloniaAV()
                .AVUseAcceleratedDirect2D()
                .UseAvaloniaAVStyles()
                .Start<MainWindow>();
        }

        // This will be made into a runtime configuration extension soon!
        private static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}
