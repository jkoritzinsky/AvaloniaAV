using System;

namespace AvaloniaAV
{
    public interface IControllablePlayback: IPlayback
    {
        TimeSpan? Duration { get; }

        void Play();
        void Pause();
        void Seek(TimeSpan span);
    }
}