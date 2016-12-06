using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace AvaloniaAV
{
    public class VideoViewer : TemplatedControl
    {
        public static readonly StyledProperty<VideoSource> SourceProperty
            = AvaloniaProperty.Register<VideoViewer, VideoSource>(nameof(Source));

        private Image image;

        public VideoViewer()
        {
            this.GetObservable(SourceProperty).Where(source => source != null).Select(source => source.CurrentFrame).Switch()
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(UpdateFrame);
        }

        public VideoSource Source
        {
            get
            {
                return GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            image = e.NameScope.Get<Image>("Image");
        }

        private void UpdateFrame(IBitmap frame)
        {
            image.Source = frame;
        }
    }
}
