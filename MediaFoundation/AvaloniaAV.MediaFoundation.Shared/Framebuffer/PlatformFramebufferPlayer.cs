using AvaloniaAV.Framebuffer;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reactive.Linq;

namespace AvaloniaAV.MediaFoundation.Framebuffer
{
    class PlatformFramebufferPlayer : IPlatformFramebufferPlayer
    {
        private readonly StreamPlayer underlyingPlayer;
        private IFramebufferPlayback currentPlayback;

        public PlatformFramebufferPlayer(AvService avService)
        {
            underlyingPlayer = avService.GetStreamPlayer();
            Playback = underlyingPlayer.Duration
                .Select(duration => new FramebufferStreamPlayback(underlyingPlayer, duration));
            Playback.Subscribe(playback =>
            {
                currentPlayback?.Dispose();
                currentPlayback = playback;
            });
        }

        public IObservable<IFramebufferPlayback> Playback { get; }

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
    }
}
