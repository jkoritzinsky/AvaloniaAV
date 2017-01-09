using System;

namespace AvaloniaAV
{
    public interface IPlayback
    {
        TimeSpan? Duration { get; }

        void Play();
        void Pause();
        void Seek(TimeSpan span);

        IObservable<Frame> CurrentFrame { get; }
    }
}