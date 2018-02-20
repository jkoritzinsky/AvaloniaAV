using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaAV.MediaFoundation;

namespace AvaloniaAV.Direct2D1
{
    class PlatformPlayer : IPlatformPlayer
    {
        private readonly StreamPlayer underlyingPlayer;

        public PlatformPlayer(AvService avService, SharpDX.Direct2D1.Device d2DDevice, SharpDX.WIC.ImagingFactory factory)
        {
            underlyingPlayer = avService.GetStreamPlayer(gpu: true);
            Playback = underlyingPlayer.Duration
                .Select(duration => new StreamPlayback(d2DDevice, factory, underlyingPlayer, duration))
                .DisposeCurrentOnNext();
        }

        public void Dispose()
        {
            underlyingPlayer.Dispose();
        }

        public void OpenUri(Uri uri)
        {
            underlyingPlayer.Open(uri);
        }

        public void OpenStream(Stream stream, Uri uri)
        {
            underlyingPlayer.Open(stream, uri);
        }

        public IObservable<IControllablePlayback> Playback { get; }
    }
}
