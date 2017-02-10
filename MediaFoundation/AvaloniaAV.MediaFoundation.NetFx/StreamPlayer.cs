using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.MediaFoundation
{
    public partial class StreamPlayer
    {
        private string TempPathForUri(Stream stream, Uri uri)
        {
            var path = Path.GetTempFileName();
            using (var file = File.OpenWrite(path))
            {
                stream.CopyTo(file);
            }
            return path;
        }
    }
}
