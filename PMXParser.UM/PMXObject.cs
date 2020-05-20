#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace MMDTools.Unmanaged
{
    public unsafe sealed class PMXObject : IDisposable
    {
        /// <summary>pointer to <see cref="PMXObject_"/></summary>
        private readonly IntPtr _ptr;

        internal PMXObject_* Entity => (PMXObject_*)_ptr;

        /// <summary>Get PMX file version</summary>
        public PMXVersion Version => Entity->Version;

        /// <summary>Get name of pmx data</summary>
        public ReadOnlyRawString Name => Entity->Name;
        /// <summary>Get English name of pmx data</summary>
        public ReadOnlyRawString NameEnglish => Entity->NameEnglish;
        /// <summary>Get comment of pmx data</summary>
        public ReadOnlyRawString Comment => Entity->Comment;
        /// <summary>Get English comment of pmx data</summary>
        public ReadOnlyRawString CommentEnglish => Entity->CommentEnglish;

        /// <summary>Get <see cref="Vertex"/> list</summary>
        public ReadOnlyRawArray<Vertex> VertexList => Entity->VertexList;

        /// <summary>Get <see cref="Surface"/> list</summary>
        public ReadOnlyRawArray<Surface> SurfaceList => Entity->SurfaceList;

        /// <summary>Get list of texture file path</summary>
        public ReadOnlyRawArray<ReadOnlyRawString> TextureList => Entity->TextureList.AsReadOnly<ReadOnlyRawString>();

        /// <summary>Get <see cref="Material"/> list</summary>
        public ReadOnlyRawArray<Material> MaterialList => Entity->MaterialList.AsReadOnly<Material>();

        /// <summary>Get <see cref="Bone"/> list</summary>
        public ReadOnlyRawArray<Bone> BoneList => Entity->BoneList.AsReadOnly<Bone>();

        /// <summary>Get <see cref="Morph"/> list</summary>
        public ReadOnlyRawArray<Morph> MorphList => Entity->MorphList.AsReadOnly<Morph>();

        /// <summary>Get <see cref="DisplayFrame"/> list</summary>
        public ReadOnlyRawArray<DisplayFrame> DisplayFrameList => Entity->DisplayFrameList.AsReadOnly<DisplayFrame>();

        /// <summary>Get <see cref="RigidBody"/> list</summary>
        public ReadOnlyRawArray<RigidBody> RigidBodyList => Entity->RigidBodyList.AsReadOnly<RigidBody>();

        /// <summary>Get <see cref="Joint"/> list</summary>
        public ReadOnlyRawArray<Joint> JointList => Entity->JointList.AsReadOnly<Joint>();

        /// <summary>Get <see cref="SoftBody"/> list</summary>
        public ReadOnlyRawArray<SoftBody> SoftBodyList => Entity->SoftBodyList.AsReadOnly<SoftBody>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe PMXObject()
        {
            var ptr = Marshal.AllocHGlobal(sizeof(PMXObject_));

            // Initialized memory for safety.
            *((PMXObject_*)ptr) = default;

            UnmanagedMemoryChecker.RegisterNewAllocatedBytes(sizeof(PMXObject_));
            _ptr = ptr;
        }

        ~PMXObject() => Dispose(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            // easy check
            if(_ptr == IntPtr.Zero) { return; }

            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            // thread-safe check
            var p = Interlocked.Exchange(ref Unsafe.AsRef(_ptr), IntPtr.Zero);
            if(p == IntPtr.Zero) { return; }

            ((PMXObject_*)p)->Dispose();
            Marshal.FreeHGlobal(p);
            UnmanagedMemoryChecker.RegisterReleasedBytes(sizeof(PMXObject_));

            UnmanagedMemoryChecker.AssertResourceReleased();
        }
    }
}
