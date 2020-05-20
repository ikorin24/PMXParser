#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MMDTools.Unmanaged
{
    [DebuggerTypeProxy(typeof(RawArrayDebuggerTypeProxy<>))]
    [DebuggerDisplay("RawArray<{typeof(T).Name}>[{Length}]")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct RawArray<T> : IDisposable, IEquatable<RawArray<T>> where T : unmanaged
    {
        // DisposableRawArray, ReadOnlyRawArray と同じメモリレイアウトにしなければならない

        private readonly IntPtr _ptr;
        private readonly int _length;

        public readonly int Length => _length;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ((T*)_ptr)[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawArray(int length)
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
                UnmanagedMemoryChecker.RegisterReleasedBytes(sizeof(T) * _length);
                Marshal.FreeHGlobal(_ptr);
                Unsafe.AsRef(_ptr) = IntPtr.Zero;   // Clear pointer into null for safety.
            }
            Unsafe.AsRef(_length) = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => new Span<T>((T*)_ptr, _length);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start)
        {
            if((uint)start >= (uint)_length) { throw new ArgumentOutOfRangeException(); }
            return new Span<T>((T*)_ptr + start, _length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start, int length)
        {
            if((uint)start >= (uint)_length) { throw new ArgumentOutOfRangeException(); }
            if((uint)start + (uint)length >= (uint)_length) { throw new ArgumentOutOfRangeException(); }
            return new Span<T>((T*)_ptr + start, length);
        }

        public override bool Equals(object? obj) => obj is RawArray<T> array && Equals(array);

        public bool Equals(RawArray<T> other)
        {
            return _ptr == other._ptr &&
                   _length == other._length;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_ptr, _length);
        }

        public static implicit operator ReadOnlyRawArray<T>(RawArray<T> array) => Unsafe.As<RawArray<T>, ReadOnlyRawArray<T>>(ref array);
        public static implicit operator RawArray<T>(ReadOnlyRawArray<T> array) => Unsafe.As<ReadOnlyRawArray<T>, RawArray<T>>(ref array);
    }

    internal sealed class RawArrayDebuggerTypeProxy<T> where T : unmanaged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly RawArray<T> _entity;

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

        public RawArrayDebuggerTypeProxy(RawArray<T> entity) => _entity = entity;
    }
}
