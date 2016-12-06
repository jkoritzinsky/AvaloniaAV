using System;

namespace AvaloniaAV
{
    interface ISupportPlaybackControl
    {
        void Play();
        void Pause();
        void Seek(TimeSpan time);
        IObservable<TimeSpan> CurrentTimeIndex { get; }
    }
}
