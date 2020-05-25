#nullable enable
using System.IO;
using System.Runtime.CompilerServices;

namespace MMDTools.Unmanaged
{
    internal static class StreamHelperUnmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static RawString NextRawString(this Stream source, int byteSize, StringEncoding encoding)
        {
            if(byteSize == 0) {
                return default;
            }
            StreamHelper.Read(source, byteSize, out var result);
            return new RawString(result, encoding);
        }
    }
}
