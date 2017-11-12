using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SharpDX.DXGI;

namespace AvaloniaAV.MediaFoundation
{
    public partial class CapturePlayer
    {
        private CancellationTokenSource tokenSource;

        public void StartCapture(SystemCamera camera)
        {
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();
            StartCaptureCore(camera, tokenSource.Token);
        }

        public void StopCapture()
        {
            tokenSource?.Cancel();
        }

        partial void StartCaptureCore(SystemCamera camera, CancellationToken token);

        public IObservable<Surface> CurrentSurface { get; private set; }
    }
}
