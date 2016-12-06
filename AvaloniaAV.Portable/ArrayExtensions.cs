using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaAV.Portable
{
    static class ArrayExtensions
    {
        public static int FindSubArray<T>(this T[] buff, T[] search)
        {
            // enumerate the buffer but don't overstep the bounds
            for (int start = 0; start < buff.Length - search.Length; start++)
            {
                // we found the first character
                if (buff[start].Equals(search[0]))
                {
                    int next;

                    // traverse the rest of the bytes
                    for (next = 1; next < search.Length; next++)
                    {
                        // if we don't match, bail
                        if (!buff[start + next].Equals(search[next]))
                            break;
                    }

                    if (next == search.Length)
                        return start;
                }
            }
            // not found
            return -1;
        }
    }
}
