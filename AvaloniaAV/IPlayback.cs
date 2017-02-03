using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV
{
    public interface IPlayback : IDisposable
    {
        IObservable<Frame> CurrentFrame { get; }
    }
}
