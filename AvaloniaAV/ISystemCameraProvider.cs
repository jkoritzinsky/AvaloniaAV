using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaAV
{
    public interface ISystemCameraProvider
    {
        Task<IEnumerable<SystemCamera>> Cameras { get; }
    }
}