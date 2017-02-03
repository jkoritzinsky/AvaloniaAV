using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Portable
{
    class PlatformPlayerProvider : IPlatformPlayerProvider
    {
        public IPlatformCameraPlayer CreateCameraPlayer()
        {
            throw new PlatformNotSupportedException("Portable camera support is not supported");
        }

        public IPlatformPlayer CreatePlayer()
        {
            return new PlatformPlayer();
        }
    }
}
