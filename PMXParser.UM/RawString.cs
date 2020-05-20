#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MMDTools.Unmanaged
{
    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct RawString : IDisposable, IEquatable<RawString>
    {
        private readonly IntPtr _headPointer;
        private readonly int _byteLength;
        public readonly int ByteLength => _byteLength;
        public readonly StringEncoding Encoding;

        /// <summary>Create <see cref="RawString"/> from pointer and length of bytes.</summary>
        /// <param name="source">copy source</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawString(ReadOnlySpan<byte> source, StringEncoding encoding)
        {
            if(source.Length == 0) {
                _headPointer = default;
                _byteLength = default;
                Encoding = encoding;
                return;
            }
            _headPointer = Marshal.AllocHGlobal(source.Length);
            _byteLength = source.Length;
            Encoding = encoding;
            source.CopyTo(new Span<byte>((void*)_headPointer, source.Length));
            UnmanagedMemoryChecker.RegisterNewAllocatedBytes(_byteLength);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Encoding GetEncoding()
        {
            return Encoding switch
            {
                StringEncoding.UTF16 => System.Text.Encoding.Unicode,
                StringEncoding.UTF8 => System.Text.Encoding.UTF8,
                _ => throw new NotSupportedException("invalid encoding"),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return GetEncoding().GetString((byte*)_headPointer, _byteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>((void*)_headPointer, _byteLength);

        public readonly ReadOnlyRawString AsReadOnly() => new ReadOnlyRawString(this);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if(_headPointer != IntPtr.Zero) {
                UnmanagedMemoryChecker.RegisterReleasedBytes(_byteLength);
                Marshal.FreeHGlobal(_headPointer);
                Unsafe.AsRef(_headPointer) = IntPtr.Zero;     // Clear pointer into null for safety.
            }
            Unsafe.AsRef(_byteLength) = 0;
        }

        public override bool Equals(object? obj) => obj is RawString @string && Equals(@string);

        public bool Equals(RawString other)
        {
            return _headPointer == other._headPointer &&
                   _byteLength == other._byteLength &&
                   Encoding == other.Encoding;
        }

        public override int GetHashCode() => HashCode.Combine(_headPointer, _byteLength, Encoding);

        public static implicit operator ReadOnlyRawString(RawString rawString) => new ReadOnlyRawString(rawString);
    }
}
