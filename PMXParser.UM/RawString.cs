#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MMDTools.Unmanaged
{
    [DebuggerDisplay("{ToString()}")]
    public unsafe readonly struct RawString : IDisposable
    {
        private readonly IntPtr _headPointer;
        public readonly int ByteLength;
        public readonly StringEncoding Encoding;

        /// <summary>Create <see cref="RawString"/> from pointer and length of bytes.</summary>
        /// <param name="source">copy source</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawString(ReadOnlySpan<byte> source, StringEncoding encoding)
        {
            if(source.Length == 0) {
                _headPointer = default;
                ByteLength = default;
                Encoding = encoding;
                return;
            }
            _headPointer = Marshal.AllocHGlobal(source.Length);
            ByteLength = source.Length;
            Encoding = encoding;
            source.CopyTo(new Span<byte>((void*)_headPointer, source.Length));
            UnmanagedMemoryChecker.RegisterNewAllocatedBytes(ByteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString()
        {
            if(ByteLength == 0) { return string.Empty; }
            var enc = Encoding switch
            {
                StringEncoding.UTF16 => System.Text.Encoding.Unicode,
                StringEncoding.UTF8 => System.Text.Encoding.UTF8,
                _ => throw new NotSupportedException(),
            };
            return enc.GetString((byte*)_headPointer, ByteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>((void*)_headPointer, ByteLength);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if(_headPointer != IntPtr.Zero) {
                UnmanagedMemoryChecker.RegisterReleasedBytes(ByteLength);
                Marshal.FreeHGlobal(_headPointer);
                Unsafe.AsRef(_headPointer) = IntPtr.Zero;     // Clear pointer into null for safety.
            }
            Unsafe.AsRef(ByteLength) = 0;
        }
    }
}
