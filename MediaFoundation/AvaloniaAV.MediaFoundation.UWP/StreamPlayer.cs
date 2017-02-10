using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AvaloniaAV.MediaFoundation
{
    public partial class StreamPlayer
    {
        private string TempPathForUri(Stream stream, Uri uri)
        {
            return uri.ToString();
        }
    }
}
