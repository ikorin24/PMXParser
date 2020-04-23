#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MMDTools.Unmanaged
{
    public unsafe readonly struct RawArray<T> : IDisposable where T : unmanaged
    {
        private readonly T* _ptr;
        public readonly int Length;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _ptr[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawArray(int length)
        {
            if(length < 0) { throw new ArgumentOutOfRangeException(nameof(length), $"{nameof(length)} is negative value."); }
            _ptr = (T*)Marshal.AllocHGlobal(sizeof(T) * length);
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Unmanaged.Delete(_ptr);
            Unsafe.AsRef(Length) = 0;
        }
    }
}
