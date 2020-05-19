#nullable enable
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MMDTools
{
    internal static class StreamHelper
    {
        [ThreadStatic]
        private static byte[]? _tlsBuffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] GetTLSBuffer(int minLength)
        {
            var tlsBuffer = _tlsBuffer;
            if(tlsBuffer != null) {
                if(tlsBuffer.Length >= minLength) {
                    return tlsBuffer;
                }
                else {
                    ArrayPool<byte>.Shared.Return(tlsBuffer);
                }
            }
            return _tlsBuffer = ArrayPool<byte>.Shared.Rent(minLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReleaseBuffer()
        {
            if(_tlsBuffer != null) {
                ArrayPool<byte>.Shared.Return(_tlsBuffer);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string NextString(this Stream source, int byteSize, Encoding encoding)
        {
            if(byteSize == 0) { return string.Empty; }
            Read(source, byteSize, out var result);
            fixed(byte* ptr = result) {
                return encoding.GetString(ptr, result.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextInt32(this Stream source)
        {
            Read(source, sizeof(int), out var result);
            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short NextInt16(this Stream source)
        {
            Read(source, sizeof(short), out var result);
            return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort NextUint16(this Stream source)
        {
            Read(source, sizeof(ushort), out var result);
            return Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextSingle(this Stream source)
        {
            Read(source, sizeof(float), out var result);
            return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte NextByte(this Stream source)
        {
            Read(source, sizeof(byte), out var result);
            return result[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NextBytes(this Stream source, int byteSize, out ReadOnlySpan<byte> bytes)
        {
            Read(source, byteSize, out var result);
            bytes = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextDataOfSize(this Stream source, byte byteSize)
        {
            // byteSize must be [1 <= byteSize <= 4]

            Read(source, byteSize, out var buf);
            return byteSize switch
            {
                1 => (int)(byte)buf[0],
                2 => (int)Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(buf)),
                4 => (int)Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(buf)),
                _ => throw new InvalidOperationException("Invalid byte size. Byte size must be 1, 2, or 4."),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextSignedDataOfSize(this Stream source, byte byteSize)
        {
            // byteSize must be [1 <= byteSize <= 4]

            Read(source, byteSize, out var buf);
            return byteSize switch
            {
                1 => (int)(sbyte)buf[0],
                2 => (int)Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(buf)),
                4 => Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(buf)),
                _ => throw new InvalidOperationException("Invalid byte size. Byte size must be 1, 2, or 4."),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Read(Stream stream, int length, out Span<byte> result)
        {
            var buf = GetTLSBuffer(length);
            if(stream.Read(buf, 0, length) != length) { throw new EndOfStreamException(); }
            result = buf.AsSpan(0, length);
        }
    }
}
