#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
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

    [DebuggerDisplay("({X}, {Y})")]
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X;
        public float Y;

        public Vector2(float x, float y) => (X, Y) = (x, y);

        public readonly override bool Equals(object? obj) => obj is Vector2 vector ? Equals(vector) : false;

        public readonly bool Equals(Vector2 other) => (X == other.X) && (Y == other.Y);

        public readonly override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);

        public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);
    }

    [DebuggerDisplay("({X}, {Y}, {Z})")]
    public struct Vector3 : IEquatable<Vector3>
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z) => (X, Y, Z) = (x, y, z);

        public readonly override bool Equals(object? obj) => obj is Vector3 vector ? Equals(vector) : false;

        public readonly bool Equals(Vector3 other) => (X == other.X) && (Y == other.Y) && (Z == other.Z);

        public readonly override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);

        public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);
    }

    [DebuggerDisplay("({X}, {Y}, {Z}, {W})")]
    public struct Vector4 : IEquatable<Vector4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w) => (X, Y, Z, W) = (x, y, z, w);

        public readonly override bool Equals(object? obj) => obj is Vector4 vector ? Equals(vector) : false;

        public readonly bool Equals(Vector4 other) => (X == other.X) && (Y == other.Y) && (Z == other.Z) && (W == other.W);

        public readonly override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public static bool operator ==(Vector4 left, Vector4 right) => left.Equals(right);

        public static bool operator !=(Vector4 left, Vector4 right) => !(left == right);
    }

    [DebuggerDisplay("(R={R}, G={G}, B={B}, A={A})")]
    public struct Color : IEquatable<Color>
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color(float r, float g, float b) : this(r, g, b, 1f) { }

        public Color(float r, float g, float b, float a) => (R, G, B, A) = (r, g, b, a);

        public readonly override bool Equals(object? obj) => obj is Color color ? Equals(color) : false;

        public readonly bool Equals(Color other) => (R == other.R) && (G == other.G) && (B == other.B) && (A == other.A);

        public readonly override int GetHashCode() => HashCode.Combine(R, G, B, A);

        public static bool operator ==(Color left, Color right) => left.Equals(right);

        public static bool operator !=(Color left, Color right) => !(left == right);
    }

    [DebuggerDisplay("Pos=({Position.X}, {Position.Y}, {Position.Z})")]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public int AdditionalUVCount;
        public Vector4 AdditionalUV1;
        public Vector4 AdditionalUV2;
        public Vector4 AdditionalUV3;
        public Vector4 AdditionalUV4;
        public WeightTransformType WeightTransformType;
        public int BoneIndex1;
        public int BoneIndex2;
        public int BoneIndex3;
        public int BoneIndex4;
        public float Weight1;
        public float Weight2;
        public float Weight3;
        public float Weight4;
        public Vector3 C;
        public Vector3 R0;
        public Vector3 R1;
        public float EdgeRatio;
    }

    [DebuggerDisplay("({V1}, {V2}, {V3})")]
    public struct Surface
    {
        public int V1;
        public int V2;
        public int V3;
    }

    [DebuggerDisplay("Material (Name={Name})")]
    public readonly struct Material
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly Color Diffuse;
        public readonly Color Specular;
        public readonly float Shininess;
        public readonly Color Ambient;
        public readonly MaterialDrawFlag DrawFlag;
        public readonly Color EdgeColor;
        public readonly float EdgeSize;
        public readonly int Texture;
        public readonly int SphereTextre;
        public readonly SphereTextureMode SphereTextureMode;
        public readonly SharedToonMode SharedToonMode;
        public readonly int ToonTexture;
        public readonly ReadOnlyRawString Memo;
        public readonly int VertexCount;
    }

    [DebuggerDisplay("Bone (Name={Name})")]
    public readonly struct Bone
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly Vector3 Position;
        public readonly int ParentBone;
        public readonly int TransformDepth;
        public readonly BoneFlag BoneFlag;
        public readonly int ConnectedBone;
        public readonly Vector3 PositionOffset;
        public readonly int AttatchParent;
        public readonly float AttatchRatio;
        public readonly Vector3 AxisVec;
        public readonly Vector3 XAxisVec;
        public readonly Vector3 ZAxisVec;
        public readonly int Key;
        public readonly int IKTarget;
        public readonly int IterCount;
        public readonly float MaxRadianPerIter;
        public readonly ReadOnlyRawArray<IKLink> IKLinks;
    }

    [DebuggerDisplay("Morph (Name={Name})")]
    public readonly struct Morph
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly MorphTarget MorphTarget;
        public readonly MorphType MorphType;
        public readonly ReadOnlyRawArray<GroupMorphElement> GroupMorphElements;
        public readonly ReadOnlyRawArray<VertexMorphElement> VertexMorphElements;
        public readonly ReadOnlyRawArray<BoneMorphElement> BoneMorphElements;
        public readonly ReadOnlyRawArray<UVMorphElement> UVMorphElements;
        public readonly ReadOnlyRawArray<MaterialMorphElement> MaterialMorphElements;
        public readonly ReadOnlyRawArray<FlipMorphElement> FlipMorphElements;
        public readonly ReadOnlyRawArray<ImpulseMorphElement> ImpulseMorphElements;
    }

    [DebuggerDisplay("GroupMorphElement (TargetMorph={TargetMorph})")]
    public struct GroupMorphElement
    {
        public int TargetMorph;
        public float MorphRatio;
    }

    [DebuggerDisplay("VertexMorphElement (TargetVertex={TargetVertex})")]
    public struct VertexMorphElement
    {
        public int TargetVertex;
        public Vector3 PosOffset;
    }

    [DebuggerDisplay("BoneMorphElement (TargetBone={TargetBone})")]
    public struct BoneMorphElement
    {
        public int TargetBone;
        public Vector3 Translate;
        public Vector4 Quaternion;
    }

    [DebuggerDisplay("UVMorphElement (TargetVertex={TargetVertex})")]
    public struct UVMorphElement
    {
        public int TargetVertex;
        public Vector4 UVOffset;
    }

    [DebuggerDisplay("MaterialMorphElement (Material={Material})")]
    public struct MaterialMorphElement
    {
        public int Material;
        public bool IsAllMaterialTarget => Material == -1;
        public MaterialMorphCalcMode CalcMode;
        public Color Diffuse;
        public Color Specular;
        public float Shininess;
        public Color Ambient;
        public Color EdgeColor;
        public float EdgeSize;
        public Color TextureCoef;
        public Color SphereTextureCoef;
        public Color ToonTextureCoef;
    }

    [DebuggerDisplay("FlipMorphElement (TargetMorph={TargetMorph})")]
    public struct FlipMorphElement
    {
        public int TargetMorph;
        public float MorphRatio;
    }

    [DebuggerDisplay("ImpulseMorphElement (TargetRigidBody={TargetRigidBody})")]
    public struct ImpulseMorphElement
    {
        public int TargetRigidBody;
        public bool IsLocal;
        public Vector3 Velocity;
        public Vector3 RotationTorque;
    }
    
    [DebuggerDisplay("DisplayFrame (Name={Name})")]
    public readonly struct DisplayFrame
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly DisplayFrameType Type;
        public readonly ReadOnlyRawArray<DisplayFrameElement> Elements;
    }

    [DebuggerDisplay("RigidBody (Name={Name})")]
    public readonly struct RigidBody
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly int Bone;
        public readonly bool HasBone;        // Bone >= 0
        public readonly byte Group;
        public readonly ushort GroupTarget;
        public readonly RigidBodyShape Shape;
        public readonly Vector3 Size;
        public readonly Vector3 Position;
        public readonly Vector3 RotationRadian;
        public readonly float Mass;
        public readonly float TranslationAttenuation;
        public readonly float RotationAttenuation;
        public readonly float Recoil;
        public readonly float Friction;
        public readonly RigidBodyPhysicsType PhysicsType;
    }

    [DebuggerDisplay("Joint (Name={Name})")]
    public readonly struct Joint
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly JointType Type;
        public readonly int RigidBody1;
        public readonly int RigidBody2;
        public readonly Vector3 Position;
        public readonly Vector3 RotationRadian;
        public readonly Vector3 TranslationMinLimit;
        public readonly Vector3 TranslationMaxLimit;
        public readonly Vector3 RotationRadianMinLimit;
        public readonly Vector3 RotationRadianMaxLimit;
        public readonly Vector3 TranslationSpring;
        public readonly Vector3 RotationSpring;
    }

    [DebuggerDisplay("SoftBody (Name={Name})")]
    public readonly struct SoftBody
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly SoftBodyShape Shape;
        public readonly int TargetMaterial;
        public readonly byte Group;
        public readonly ushort GroupTarget;
        public readonly SoftBodyModeFlag Mode;
        public readonly int BLinkDistance;
        public readonly int ClusterCount;
        public readonly float TotalMass;
        public readonly float CollisionMargin;
        public readonly SoftBodyAeroModel AeroModel;
        public readonly SoftBodyConfig Config;
        public readonly SoftBodyCluster Cluster;
        public readonly SoftBodyIteration Iteration;
        public readonly SoftBodyMaterial Material;
        public readonly ReadOnlyRawArray<AnchorRigidBody> AnchorRigidBodies;
        public readonly ReadOnlyRawArray<int> PinnedVertex;
    }

    public struct SoftBodyConfig
    {
        public float VCF;
        public float DP;
        public float DG;
        public float LF;
        public float PR;
        public float VC;
        public float DF;
        public float MT;
        public float CHR;
        public float KHR;
        public float SHR;
        public float AHR;
    }

    public struct SoftBodyCluster
    {
        public float SRHR_CL;
        public float SKHR_CL;
        public float SSHR_CL;
        public float SR_SPLT_CL;
        public float SK_SPLT_CL;
        public float SS_SPLT_CL;
    }

    public struct SoftBodyIteration
    {
        public int V_IT;
        public int P_IT;
        public int D_IT;
        public int C_IT;
    }

    public struct SoftBodyMaterial
    {
        public float LST;
        public float AST;
        public float VST;
    }


    [DebuggerDisplay("IKLink (Bone={Bone})")]
    public struct IKLink
    {
        public int Bone;
        public bool IsEnableAngleLimited;
        public Vector3 MinLimit;
        public Vector3 MaxLimit;
    }

    [DebuggerDisplay("DisplayFrameElement (TargetType={TargetType}, TargetIndex={TargetIndex})")]
    public struct DisplayFrameElement
    {
        public DisplayFrameElementTarget TargetType;
        public int TargetIndex;
    }

    [DebuggerDisplay("AnchorRigidBody (RigidBody={RigidBody}, Vertex={Vertex}, IsNearMode={IsNearMode})")]
    public struct AnchorRigidBody
    {
        public int RigidBody;
        public int Vertex;
        public bool IsNearMode;
    }

    public enum PMXVersion
    {
        /// <summary>PMX Ver 2.0</summary>
        V20 = 20,
        /// <summary>PMX Ver 2.1</summary>
        V21 = 21,
    }

    public enum StringEncoding : byte
    {
        UTF16 = 0,
        UTF8 = 1,
    }

    [Flags]
    public enum MaterialDrawFlag : byte
    {
        BothSidesDrawing = 0x01,
        GroundShadow = 0x02,
        DrawingInSelfShadowMap = 0x04,
        SelfShadowDrawing = 0x08,
        EdgeDrawing = 0x10,
        VertexColor = 0x20,
        PointDrawing = 0x40,
        LineDrawing = 0x80,
    }

    public enum SphereTextureMode : byte
    {
        Disabled = 0,
        Mult = 1,
        Add = 2,
        SubTexture = 3,
    }

    public enum SharedToonMode : byte
    {
        TextureIndex = 0,
        SharedToon = 1,
    }

    [Flags]
    public enum BoneFlag : short
    {
        ConnectionDestination = 0x0001,
        Rotatable = 0x0002,
        Translatable = 0x0004,
        Visible = 0x0008,
        Editable = 0x0010,
        IK = 0x0020,
        LocalAttached = 0x0080,
        RotationAttach = 0x0100,
        TranslationAttach = 0x0200,
        FixedAxis = 0x0400,
        LocalAxis = 0x0800,
        TransformAfterPhysics = 0x1000,
        ExternalParentTransform = 0x2000,
    }

    public enum WeightTransformType : byte
    {
        BDEF1 = 0,
        BDEF2 = 1,
        BDEF4 = 2,
        SDEF = 3,
        QDEF = 4,
    }

    public enum MorphTarget : byte
    {
        SystemReserved = 0,
        Eyebrow = 1,
        Eye = 2,
        Mouth = 3,
        Other = 4,
    }

    public enum MorphType : byte
    {
        Group = 0,
        Vertex = 1,
        Bone = 2,
        UV = 3,
        AdditionalUV1 = 4,
        AdditionalUV2 = 5,
        AdditionalUV3 = 6,
        AdditionalUV4 = 7,
        Material = 8,
        Flip = 9,
        Impulse = 10,
    }

    public enum MaterialMorphCalcMode : byte
    {
        Mult = 0,
        Add = 1,
    }

    public enum DisplayFrameType : byte
    {
        Normal,
        Special,
    }

    public enum DisplayFrameElementTarget : byte
    {
        Bone = 0,
        Morph = 1,
    }

    public enum RigidBodyShape : byte
    {
        Sphere = 0,
        Box = 1,
        Capsule = 2,
    }

    public enum RigidBodyPhysicsType : byte
    {
        Static = 0,
        Dynamic = 1,
        DynamicAndBonePosition = 2,
    }

    public enum JointType : byte
    {
        Spring6DOF = 0,
        NoSpring6DOF = 1,
        P2P = 2,
        ConeTwist = 3,
        Slider = 4,
        Hinge = 5,
    }

    public enum SoftBodyShape : byte
    {
        TriBesh,
        Rope,
    }

    [Flags]
    public enum SoftBodyModeFlag : byte
    {
        CreateBLink = 0x01,
        CreateCluster = 0x02,
        CrossedLink = 0x04,
    }

    public enum SoftBodyAeroModel : int
    {
        VPoint = 0,
        VTwoSided = 1,
        VOneSided = 2,
        FTwoSided = 3,
        FOneSided = 4,
    }

    public static class FlagsEnumExtension
    {
        // This is compatible with Enum.HasFlag method.
        // Enum.HasFlag method is bad performance because of boxing. About 20 times slower.
        // (This happens only before dotnet core 2.0. The JIT compiler after dotnet core 2.1 avoid it.)

        public static bool Has(this BoneFlag source, BoneFlag flag)
        {
            return (source & flag) == flag;
        }
    }
}
