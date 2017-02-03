using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using SharpDX.MediaFoundation;

namespace AvaloniaAV.MediaFoundation
{
    public abstract class MediaFoundationModule
    {
        protected MediaFoundationModule()
        {
            MediaManager.Startup();
            Application.Current.OnExit += (obj, args) => MediaManager.Shutdown();
#if !REFERENCE
            AvaloniaLocator.CurrentMutable.Bind<ISystemCameraProvider>().ToSingleton<SystemCameraProvider>();
#endif
            AvaloniaLocator.CurrentMutable.Bind<AvService>().ToSingleton<AvService>();
        }
    }
}
