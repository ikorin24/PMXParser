#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MMDTools.Unmanaged
{
    [DebuggerTypeProxy(typeof(DisposableRawArrayDebuggerTypeProxy<>))]
    [DebuggerDisplay("DisposableRawArray<{typeof(T).Name}>[{Length}]")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct DisposableRawArray<T> : IDisposable, IEquatable<DisposableRawArray<T>> where T : unmanaged, IDisposable
    {
        // RawArray, ReadOnlyRawArray と同じメモリレイアウトにしなければならない

        private readonly IntPtr _ptr;
        private readonly int _length;
        public readonly int Length => _length;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ((T*)_ptr)[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal DisposableRawArray(int length)
        {
            if(length < 0) { throw new ArgumentOutOfRangeException(nameof(length), $"{nameof(length)} is negative value."); }
            if(length == 0) {
                this = default;
                return;
            }
            var byteLen = sizeof(T) * length;
            _ptr = Marshal.AllocHGlobal(byteLen);
            new Span<T>((T*)_ptr, length).Fill(default);
            _length = length;
            UnmanagedMemoryChecker.RegisterNewAllocatedBytes(byteLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if(_ptr != IntPtr.Zero) {
                foreach(var item in AsSpan()) {
                    item.Dispose();
                }
                UnmanagedMemoryChecker.RegisterReleasedBytes(sizeof(T) * Length);
                Marshal.FreeHGlobal(_ptr);
                Unsafe.AsRef(_ptr) = IntPtr.Zero;   // Clear pointer into null for safety.
            }
            Unsafe.AsRef(Length) = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => new Span<T>((T*)_ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start)
        {
            if((uint)start >= (uint)Length) { throw new ArgumentOutOfRangeException(); }
            return new Span<T>((T*)_ptr + start, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start, int length)
        {
            if((uint)start >= (uint)Length) { throw new ArgumentOutOfRangeException(); }
            if((uint)start + (uint)length >= (uint)Length) { throw new ArgumentOutOfRangeException(); }
            return new Span<T>((T*)_ptr + start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlyRawArray<TTo> AsReadOnly<TTo>() where TTo : unmanaged
        {
            if(sizeof(T) != sizeof(TTo)) { throw new InvalidCastException("Type size of element is mismatch."); }
            ref var this_ = ref Unsafe.AsRef(this);
            return Unsafe.As<DisposableRawArray<T>, ReadOnlyRawArray<TTo>>(ref this_);
        }

        public override bool Equals(object? obj) => obj is DisposableRawArray<T> array && Equals(array);

        public bool Equals(DisposableRawArray<T> other)
        {
            return _ptr == other._ptr &&
                   _length == other._length;
        }

        public override int GetHashCode() => HashCode.Combine(_ptr, _length);
    }

    internal sealed class DisposableRawArrayDebuggerTypeProxy<T> where T : unmanaged, IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DisposableRawArray<T> _entity;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var items = new T[_entity.Length];
                _entity.AsSpan().CopyTo(items);
                return items;
            }
        }

        public DisposableRawArrayDebuggerTypeProxy(DisposableRawArray<T> entity) => _entity = entity;
    }
}
