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
        // 先頭へのポインタは void* や IntPtr 型でフィールドに持ってはいけない。
        // T が再帰的に参照またはポインタを含む場合、.NET Framework で Span<T> を作ることができない。
        // (ただし.NET Core では可能。)
        // Span<RawString> を作成可能にするために、ポインタを ulong で保持する。
        // このフィールドのポインタは常にアンマネージドヒープメモリを指すため、
        // Span<RawString> を .NET Framework で作成しても問題は起こらない。
        private readonly ulong _headPointer;
        public readonly int ByteLength;
        public readonly StringEncoding Encoding;

        /// <summary>Create <see cref="RawString"/> from pointer and length of bytes.</summary>
        /// <param name="source">copy source</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawString(ReadOnlySpan<byte> source, StringEncoding encoding)
        {
            _headPointer = (ulong)Marshal.AllocHGlobal(source.Length);
            ByteLength = source.Length;
            Encoding = encoding;
            source.CopyTo(new Span<byte>((void*)_headPointer, source.Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(Encoding encoding)
        {
            return encoding?.GetString((byte*)_headPointer, ByteLength) ?? throw new ArgumentNullException(nameof(encoding));
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
            Marshal.FreeHGlobal((IntPtr)_headPointer);    // Do nothing when pointer is null.
            Unsafe.AsRef(_headPointer) = 0;               // Clear pointer into null for safety.
            Unsafe.AsRef(ByteLength) = 0;
        }
    }
}
