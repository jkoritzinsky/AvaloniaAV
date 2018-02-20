using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

namespace AvaloniaAV
{
    public class VideoViewer : TemplatedControl
    {
        public const string FrameViewerPart = "PART_FrameViewer";
        public const string PlayPauseButtonPart = "PART_PlayPauseButton";

        public static readonly StyledProperty<Uri> VideoUriProperty
            = AvaloniaProperty.Register<VideoViewer, Uri>(nameof(VideoUri));

        private static readonly DirectProperty<VideoViewer, IControllablePlayback> CurrentPlaybackProperty
            = AvaloniaProperty.RegisterDirect<VideoViewer, IControllablePlayback>(nameof(CurrentPlayback)
                , viewer => viewer.CurrentPlayback, (viewer, playback) => viewer.CurrentPlayback = playback);

        private static readonly DirectProperty<VideoViewer, bool> IsPlayingProperty
            = AvaloniaProperty.RegisterDirect<VideoViewer, bool>(nameof(IsPlaying), viewer => viewer.IsPlaying);

        static VideoViewer()
        {
            VideoUriProperty.Changed.AddClassHandler<VideoViewer>((viewer, e) => viewer.OpenUri((Uri)e.NewValue));
        }

        public VideoViewer()
        {
            player = AvaloniaLocator.Current.GetService<IPlatformPlayerProvider>().CreatePlayer();
            Bind(CurrentPlaybackProperty, player.Playback);

            PseudoClass(CurrentPlaybackProperty, playback => playback.Duration != null, ":seekable");
            PseudoClass(IsPlayingProperty, ":playing");
        }

        private void OpenUri(Uri uri)
        {
            if (uri.IsAbsoluteUri && uri.Scheme == "resm")
            {
                var uriWithoutScheme = new Uri(uri.ToString().Substring("resm:".Length), UriKind.Relative);
                player.OpenStream(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(uri), uriWithoutScheme);
            }
            else
            {
                player.OpenUri(uri);
            }
            IsPlaying = false;
        }

        private IPlatformPlayer player;
        
        private Button playPauseButton;
        private Image frameViewer;

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            if (playPauseButton != null)
            {
                playPauseButton.Click -= ToggleIsPlaying; 
            }
            if (frameViewer != null)
            {
                frameViewer[Image.SourceProperty] = AvaloniaProperty.UnsetValue; 
            }

            base.OnTemplateApplied(e);

            playPauseButton = e.NameScope.Get<Button>(PlayPauseButtonPart);
            frameViewer = e.NameScope.Get<Image>(FrameViewerPart);
            frameViewer[!Image.SourceProperty] = this.GetObservable(CurrentPlaybackProperty)
                .SelectMany(playback => playback?.CurrentFrame ?? Observable.Never<Frame>())
                .DisposeCurrentOnNext()
                .Select(frame => new Bitmap(frame.FrameBitmap))
                .DisposeCurrentOnNext().ToBinding();

            playPauseButton.Click += ToggleIsPlaying;
        }

        private void ToggleIsPlaying(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
            {
                CurrentPlayback?.Pause();
            }
            else
            {
                CurrentPlayback?.Play();
            }

            if (CurrentPlayback != null)
            {
                IsPlaying = !IsPlaying;
            }
        }
        
        public Uri VideoUri
        {
            get
            {
                return GetValue(VideoUriProperty);
            }
            set
            {
                SetValue(VideoUriProperty, value);
            }
        }

        private bool isPlaying;

        private bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
            set
            {
                SetAndRaise(IsPlayingProperty, ref isPlaying, value);
            }
        }

        private IControllablePlayback currentPlayback;

        private IControllablePlayback CurrentPlayback
        {
            get
            {
                return currentPlayback;
            }
            set
            {
                SetAndRaise(CurrentPlaybackProperty, ref currentPlayback, value);
            }
        }
    }
}
