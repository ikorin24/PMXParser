﻿#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MMDTools.Unmanaged
{
    [DebuggerTypeProxy(typeof(DisposableRawArrayDebuggerTypeProxy<>))]
    [DebuggerDisplay("DisposableRawArray<{typeof(T).Name}>[{Length}]")]
    public unsafe readonly struct DisposableRawArray<T> : IDisposable where T : unmanaged, IDisposable
    {
        private readonly IntPtr _ptr;
        public readonly int Length;

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
            Length = length;
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