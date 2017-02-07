using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV
{
    public class PlayPauseButton : Button
    {
        public static readonly DirectProperty<PlayPauseButton, bool> IsPlayingProperty =
            AvaloniaProperty.RegisterDirect<PlayPauseButton, bool>(nameof(IsPlaying),
                button => button.IsPlaying,
                (button, showPlay) => button.IsPlaying = showPlay);

        public PlayPauseButton()
        {
            PseudoClass(IsPlayingProperty, ":playing");
        }

        private bool showPlay;
        public bool IsPlaying
        {
            get
            {
                return showPlay;
            }
            set
            {
                SetAndRaise(IsPlayingProperty, ref showPlay, value);
            }
        }
    }
}
