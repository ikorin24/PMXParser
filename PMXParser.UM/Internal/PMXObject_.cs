#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MMDTools.Unmanaged
{
    internal unsafe struct PMXObject_ : IDisposable
    {
        /// <summary>Get PMX file version</summary>
        public PMXVersion Version;

        /// <summary>Get name of pmx data</summary>
        public RawString Name;
        /// <summary>Get English name of pmx data</summary>
        public RawString NameEnglish;
        /// <summary>Get comment of pmx data</summary>
        public RawString Comment;
        /// <summary>Get English comment of pmx data</summary>
        public RawString CommentEnglish;

        ///// <summary>Get <see cref="Vertex"/> list</summary>
        public RawArray<Vertex> VertexList;

        /// <summary>Get <see cref="Surface"/> list</summary>
        public RawArray<Surface> SurfaceList;

        /// <summary>Get list of texture file path</summary>
        public DisposableRawArray<RawString> TextureList;

        /// <summary>Get <see cref="Material_"/> list</summary>
        public DisposableRawArray<Material_> MaterialList;

        /// <summary>Get <see cref="Bone_"/> list</summary>
        public DisposableRawArray<Bone_> BoneList;

        /// <summary>Get <see cref="Morph_"/> list</summary>
        public DisposableRawArray<Morph_> MorphList;

        /// <summary>Get <see cref="DisplayFrame_"/> list</summary>
        public DisposableRawArray<DisplayFrame_> DisplayFrameList;

        /// <summary>Get <see cref="RigidBody_"/> list</summary>
        public DisposableRawArray<RigidBody_> RigidBodyList;

        /// <summary>Get <see cref="Joint_"/> list</summary>
        public DisposableRawArray<Joint_> JointList;

        /// <summary>Get <see cref="SoftBody_"/> list</summary>
        public DisposableRawArray<SoftBody_> SoftBodyList;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            Comment.Dispose();
            CommentEnglish.Dispose();
            VertexList.Dispose();
            SurfaceList.Dispose();
            TextureList.Dispose();
            MaterialList.Dispose();
            BoneList.Dispose();
            MorphList.Dispose();
            DisplayFrameList.Dispose();
            RigidBodyList.Dispose();
            JointList.Dispose();
            SoftBodyList.Dispose();
        }
    }

    [DebuggerDisplay("Material (Name={Name})")]
    internal struct Material_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public Color Diffuse;
        public Color Specular;
        public float Shininess;
        public Color Ambient;
        public MaterialDrawFlag DrawFlag;
        public Color EdgeColor;
        public float EdgeSize;
        public int Texture;
        public int SphereTextre;
        public SphereTextureMode SphereTextureMode;
        public SharedToonMode SharedToonMode;
        public int ToonTexture;
        public RawString Memo;
        public int VertexCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            Memo.Dispose();
        }
    }

    [DebuggerDisplay("Bone (Name={Name})")]
    internal struct Bone_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public Vector3 Position;
        public int ParentBone;
        public int TransformDepth;
        public BoneFlag BoneFlag;
        public int ConnectedBone;
        public Vector3 PositionOffset;
        public int AttatchParent;
        public float AttatchRatio;
        public Vector3 AxisVec;
        public Vector3 XAxisVec;
        public Vector3 ZAxisVec;
        public int Key;
        public int IKTarget;
        public int IterCount;
        public float MaxRadianPerIter;
        public RawArray<IKLink> IKLinks;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            IKLinks.Dispose();
        }
    }

    [DebuggerDisplay("Morph (Name={Name})")]
    internal struct Morph_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public MorphTarget MorphTarget;
        public MorphType MorphType;
        public RawArray<GroupMorphElement> GroupMorphElements;
        public RawArray<VertexMorphElement> VertexMorphElements;
        public RawArray<BoneMorphElement> BoneMorphElements;
        public RawArray<UVMorphElement> UVMorphElements;
        public RawArray<MaterialMorphElement> MaterialMorphElements;
        public RawArray<FlipMorphElement> FlipMorphElements;
        public RawArray<ImpulseMorphElement> ImpulseMorphElements;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            GroupMorphElements.Dispose();
            VertexMorphElements.Dispose();
            BoneMorphElements.Dispose();
            UVMorphElements.Dispose();
            MaterialMorphElements.Dispose();
            FlipMorphElements.Dispose();
            ImpulseMorphElements.Dispose();
        }
    }

    [DebuggerDisplay("DisplayFrame (Name={Name})")]
    internal struct DisplayFrame_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public DisplayFrameType Type;
        public RawArray<DisplayFrameElement> Elements;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            Elements.Dispose();
        }
    }

    [DebuggerDisplay("RigidBody (Name={Name})")]
    internal struct RigidBody_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public int Bone;
        public bool HasBone;        // Bone >= 0
        public byte Group;
        public ushort GroupTarget;
        public RigidBodyShape Shape;
        public Vector3 Size;
        public Vector3 Position;
        public Vector3 RotationRadian;
        public float Mass;
        public float TranslationAttenuation;
        public float RotationAttenuation;
        public float Recoil;
        public float Friction;
        public RigidBodyPhysicsType PhysicsType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
        }
    }

    [DebuggerDisplay("Joint (Name={Name})")]
    internal struct Joint_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public JointType Type;
        public int RigidBody1;
        public int RigidBody2;
        public Vector3 Position;
        public Vector3 RotationRadian;
        public Vector3 TranslationMinLimit;
        public Vector3 TranslationMaxLimit;
        public Vector3 RotationRadianMinLimit;
        public Vector3 RotationRadianMaxLimit;
        public Vector3 TranslationSpring;
        public Vector3 RotationSpring;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
        }
    }

    [DebuggerDisplay("SoftBody (Name={Name})")]
    internal struct SoftBody_ : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public SoftBodyShape Shape;
        public int TargetMaterial;
        public byte Group;
        public ushort GroupTarget;
        public SoftBodyModeFlag Mode;
        public int BLinkDistance;
        public int ClusterCount;
        public float TotalMass;
        public float CollisionMargin;
        public SoftBodyAeroModel AeroModel;
        public SoftBodyConfig Config;
        public SoftBodyCluster Cluster;
        public SoftBodyIteration Iteration;
        public SoftBodyMaterial Material;
        public RawArray<AnchorRigidBody> AnchorRigidBodies;
        public RawArray<int> PinnedVertex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            AnchorRigidBodies.Dispose();
        }
    }
}
