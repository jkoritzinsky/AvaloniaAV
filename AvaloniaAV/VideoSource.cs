using System;
using System.Reactive.Subjects;
using Avalonia.Media.Imaging;

namespace AvaloniaAV
{
    public abstract class VideoSource
    {
        public VideoSource()
        {
            CurrentFrame = new Subject<IBitmap>();
        }

        public Subject<IBitmap> CurrentFrame { get; }
    }
}
