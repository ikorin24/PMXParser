#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MMDTools.Unmanaged
{
    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct ReadOnlyRawString : IEquatable<ReadOnlyRawString>
    {
        private readonly RawString _rawString;
        public readonly int ByteLength => _rawString.ByteLength;
        public readonly StringEncoding Encoding => _rawString.Encoding;

        internal ReadOnlyRawString(RawString rawString) => _rawString = rawString;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Encoding GetEncoding() => _rawString.GetEncoding();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsSpan() => _rawString.AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _rawString.ToString();

        public override bool Equals(object? obj) => obj is ReadOnlyRawString str && Equals(str);

        public bool Equals(ReadOnlyRawString other) => _rawString.Equals(other._rawString);

        public override int GetHashCode() => HashCode.Combine(_rawString);
    }
}
