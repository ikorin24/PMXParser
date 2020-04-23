#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MMDTools.Unmanaged
{
    internal unsafe static class Unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T* New<T>() where T : unmanaged
        {
            var ptr = (T*)Marshal.AllocHGlobal(sizeof(T));

            // Initialized memory for safety.
            *ptr = new T();
            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Delete<T>(in T* ptr) where T : unmanaged => Delete((void*)ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Delete(in void* ptr)
        {
            var p = (IntPtr)ptr;
            Marshal.FreeHGlobal(p);     // Do nothing when pointer is null.

            // Clear pointer into null for safety.
            Unsafe.AsRef(p) = IntPtr.Zero;
        }
    }
}
