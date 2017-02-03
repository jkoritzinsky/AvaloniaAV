using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using AvaloniaAV.MediaFoundation;

[assembly:ExportAvaloniaModule("AvaloniaAV", typeof(AvaloniaAV.Direct2D1.Module), ForRenderingSubsystem = "Direct2D1")]

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
