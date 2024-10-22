﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MMDTools.Unmanaged
{
    [DebuggerTypeProxy(typeof(ReadOnlyRawArrayDebuggerTypeProxy<>))]
    [DebuggerDisplay("ReadOnlyRawArray<{typeof(T).Name}>[{Length}]")]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct ReadOnlyRawArray<T> : IEquatable<ReadOnlyRawArray<T>> where T : unmanaged
    {
        // RawArray, DisposableRawArray と同じメモリレイアウトでなければならない

        private readonly IntPtr _ptr;
        private readonly int _length;

        public readonly int Length => _length;
        public ref readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if((uint)index >= (uint)_length) {
                    ThrowOutOfRange();
                    static void ThrowOutOfRange() => throw new IndexOutOfRangeException(nameof(index));
                }
                return ref ((T*)_ptr)[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsSpan() => ((RawArray<T>)this).AsSpan();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsSpan(int start) => ((RawArray<T>)this).AsSpan(start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsSpan(int start, int length) => ((RawArray<T>)this).AsSpan(start, length);

        public override bool Equals(object? obj) => obj is ReadOnlyRawArray<T> array && Equals(array);

        public bool Equals(ReadOnlyRawArray<T> other)
        {
            return _ptr == other._ptr &&
                   _length == other._length;
        }

        public override int GetHashCode() => HashCode.Combine(_ptr, _length);
    }

    internal sealed class ReadOnlyRawArrayDebuggerTypeProxy<T> where T : unmanaged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyRawArray<T> _entity;

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

        public ReadOnlyRawArrayDebuggerTypeProxy(ReadOnlyRawArray<T> entity) => _entity = entity;
    }
}
