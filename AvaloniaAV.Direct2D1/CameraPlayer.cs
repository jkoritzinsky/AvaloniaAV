using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaAV.MediaFoundation;
using System.Reactive.Subjects;
using SharpDX.Direct2D1;
using SharpDX.WIC;

namespace AvaloniaAV.Direct2D1
{
    class CameraPlayer : IPlatformCameraPlayer
    {
        private readonly ImagingFactory imagingFactory;
        private CapturePlayer underlyingPlayer;

        private Subject<CapturePlayback> playbackSubject = new Subject<CapturePlayback>();
        private CapturePlayback currentPlayback;
        private Device device;

        public CameraPlayer(AvService avService, Device device, ImagingFactory factory)
        {
            this.device = device;
            underlyingPlayer = avService.GetCapturePlayer();
            imagingFactory = factory;
        }

        public void OpenCamera(SystemCamera camera)
        {
            underlyingPlayer.StartCapture(camera);
            var newPlayback = new CapturePlayback(device, imagingFactory, underlyingPlayer);
            playbackSubject.OnNext(newPlayback);
            currentPlayback?.Dispose();
            currentPlayback = newPlayback;
        }

        public IObservable<IPlayback> Playback => playbackSubject;
    }
}
