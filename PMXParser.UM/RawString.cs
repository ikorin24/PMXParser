#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MMDTools.Unmanaged
{
    public unsafe readonly struct RawString : IDisposable
    {
        private readonly byte* _head;
        public readonly int ByteLength;
        public readonly StringEncoding Encoding;

        /// <summary>Create <see cref="RawString"/> from pointer and length of bytes.</summary>
        /// <param name="source">copy source</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawString(ReadOnlySpan<byte> source, StringEncoding encoding)
        {
            _head = (byte*)Marshal.AllocHGlobal(source.Length);
            source.CopyTo(new Span<byte>(_head, source.Length));
            ByteLength = source.Length;
            Encoding = encoding;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(Encoding encoding)
        {
            return encoding?.GetString(_head, ByteLength) ?? throw new ArgumentNullException(nameof(encoding));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString()
        {
            if(ByteLength == 0) { return string.Empty; }
            var enc = Encoding switch
            {
                StringEncoding.UTF16 => System.Text.Encoding.Unicode,
                StringEncoding.UTF8 => System.Text.Encoding.UTF8,
                _ => throw new NotImplementedException(),
            };
            return enc.GetString(_head, ByteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(_head, ByteLength);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Unmanaged.Delete(_head);
            Unsafe.AsRef(ByteLength) = 0;
        }
    }
}
