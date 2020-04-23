#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MMDTools.Unmanaged
{
    internal static class StreamHelperUnmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void NextRawString(this Stream source, int byteSize, StringEncoding encoding, RawString* dest)
        {
            if(byteSize == 0) {
                *dest = default;
                return;
            }
            StreamHelper.Read(source, byteSize, out var result);
            *dest = new RawString(result, encoding);
        }
    }
}
