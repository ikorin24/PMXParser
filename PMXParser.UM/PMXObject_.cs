#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MMDTools.Unmanaged
{
    public unsafe readonly struct PMXObject : IDisposable
    {
        /// <summary>pointer to <see cref="PMXObject_"/></summary>
        private readonly IntPtr _ptr;

        internal PMXObject_* Entity => (PMXObject_*)_ptr;

        private unsafe PMXObject(PMXObject_* ptr) => _ptr = (IntPtr)ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static PMXObject New()
        {
            var ptr = (PMXObject_*)Marshal.AllocHGlobal(sizeof(PMXObject_));
            *ptr = default;             // Initialized memory for safety.
            return new PMXObject(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            // easy check
            if(_ptr == IntPtr.Zero) { return; }

            // thread-safe check
            var p = Interlocked.Exchange(ref Unsafe.AsRef(_ptr), IntPtr.Zero);
            if(p == IntPtr.Zero) { return; }

            ((PMXObject_*)p)->Dispose();
            Marshal.FreeHGlobal(p);
        }
    }

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

        /// <summary>Get <see cref="Vertex"/> list</summary>
        public RawArray<Vertex> VertexList;

        /// <summary>Get <see cref="Surface"/> list</summary>
        public RawArray<Surface> SurfaceList;

        /// <summary>Get list of texture file path</summary>
        public RawArray<RawString> TextureList;

        /// <summary>Get <see cref="Material"/> list</summary>
        public RawArray<Material> MaterialList;

        /// <summary>Get <see cref="Bone"/> list</summary>
        public RawArray<Bone> BoneList;

        /// <summary>Get <see cref="Morph"/> list</summary>
        public RawArray<Morph> MorphList;

        /// <summary>Get <see cref="DisplayFrame"/> list</summary>
        public RawArray<DisplayFrame> DisplayFrameList;

        /// <summary>Get <see cref="RigidBody"/> list</summary>
        public RawArray<RigidBody> RigidBodyList;

        /// <summary>Get <see cref="Joint"/> list</summary>
        public RawArray<Joint> JointList;

        /// <summary>Get <see cref="SoftBody"/> list</summary>
        public RawArray<SoftBody> SoftBodyList;

        public readonly void Dispose()
        {
            //throw new NotImplementedException("配列とその各要素の持つリソースを再帰的に全て破棄しなければならない");
            Name.Dispose();
            NameEnglish.Dispose();
            Comment.Dispose();
            CommentEnglish.Dispose();
            VertexList.Dispose();
            SurfaceList.Dispose();
        }
    }

    public enum StringEncoding : byte
    {
        UTF16 = 0,
        UTF8 = 1,
    }

    [DebuggerDisplay("({X}, {Y})")]
    public struct Vector2 : IEquatable<Vector2>
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2 vector ? Equals(vector) : false;
        }

        public bool Equals(Vector2 other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }
    }

    [DebuggerDisplay("({X}, {Y}, {Z})")]
    public struct Vector3 : IEquatable<Vector3>
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector3 vector ? Equals(vector) : false;
        }

        public bool Equals(Vector3 other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !(left == right);
        }
    }

    [DebuggerDisplay("({X}, {Y}, {Z}, {W})")]
    public struct Vector4 : IEquatable<Vector4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector4 vector ? Equals(vector) : false;
        }

        public bool Equals(Vector4 other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z &&
                   W == other.W;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !(left == right);
        }
    }

    [DebuggerDisplay("(R={R}, G={G}, B={B}, A={A})")]
    public struct Color : IEquatable<Color>
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            A = 1f;
        }

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override bool Equals(object? obj)
        {
            return obj is Color color ? Equals(color) : false;
        }

        public bool Equals(Color other)
        {
            return R == other.R &&
                   G == other.G &&
                   B == other.B &&
                   A == other.A;
        }

        public override int GetHashCode() => HashCode.Combine(R, G, B, A);

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }
    }

    [DebuggerDisplay("Pos=({Position.X}, {Position.Y}, {Position.Z})")]
    public struct Vertex : IDisposable
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

        public readonly void Dispose() { }   // nop
    }

    [DebuggerDisplay("({V1}, {V2}, {V3})")]
    public struct Surface : IDisposable
    {
        public int V1;
        public int V2;
        public int V3;
        public readonly void Dispose() { }   // nop
    }

    [DebuggerDisplay("Material (Name={Name})")]
    public struct Material : IDisposable
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

        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            Memo.Dispose();
        }
    }

    [DebuggerDisplay("Bone (Name={Name})")]
    public struct Bone : IDisposable
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

        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
            IKLinks.Dispose();
        }
    }

    [DebuggerDisplay("Morph (Name={Name})")]
    public struct Morph : IDisposable
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

    [DebuggerDisplay("GroupMorphElement (TargetMorph={TargetMorph})")]
    public struct GroupMorphElement : IDisposable
    {
        public int TargetMorph;
        public float MorphRatio;

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("VertexMorphElement (TargetVertex={TargetVertex})")]
    public struct VertexMorphElement : IDisposable
    {
        public int TargetVertex;
        public Vector3 PosOffset;

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("BoneMorphElement (TargetBone={TargetBone})")]
    public struct BoneMorphElement : IDisposable
    {
        public int TargetBone;
        public Vector3 Translate;
        public Vector4 Quaternion;

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("UVMorphElement (TargetVertex={TargetVertex})")]
    public struct UVMorphElement : IDisposable
    {
        public int TargetVertex;
        public Vector4 UVOffset;

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("MaterialMorphElement (Material={Material})")]
    public struct MaterialMorphElement : IDisposable
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

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("FlipMorphElement (TargetMorph={TargetMorph})")]
    public struct FlipMorphElement : IDisposable
    {
        public int TargetMorph;
        public float MorphRatio;

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("ImpulseMorphElement (TargetRigidBody={TargetRigidBody})")]
    public struct ImpulseMorphElement : IDisposable
    {
        public int TargetRigidBody;
        public bool IsLocal;
        public Vector3 Velocity;
        public Vector3 RotationTorque;

        public readonly void Dispose() { }  // nop
    }

    [DebuggerDisplay("DisplayFrame (Name={Name})")]
    public struct DisplayFrame : IDisposable
    {
        public RawString Name;
        public RawString NameEnglish;
        public DisplayFrameType Type;
        public RawArray<DisplayFrameElement> Elements;

        public readonly void Dispose()
        {
            Elements.Dispose();
        }
    }

    [DebuggerDisplay("RigidBody (Name={Name})")]
    public struct RigidBody : IDisposable
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

        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
        }
    }

    [DebuggerDisplay("Joint (Name={Name})")]
    public struct Joint : IDisposable
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

        public readonly void Dispose()
        {
            Name.Dispose();
            NameEnglish.Dispose();
        }
    }

    [DebuggerDisplay("SoftBody (Name={Name})")]
    public struct SoftBody
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
    public struct IKLink : IEquatable<IKLink>
    {
        public int Bone { get; internal set; }
        public bool IsEnableAngleLimited { get; internal set; }
        public Vector3 MinLimit { get; internal set; }
        public Vector3 MaxLimit { get; internal set; }

        public override bool Equals(object? obj)
        {
            return obj is IKLink link && Equals(link);
        }

        public bool Equals(IKLink other)
        {
            return Bone == other.Bone &&
                   IsEnableAngleLimited == other.IsEnableAngleLimited &&
                   MinLimit.Equals(other.MinLimit) &&
                   MaxLimit.Equals(other.MaxLimit);
        }

        public override int GetHashCode() => HashCode.Combine(Bone, IsEnableAngleLimited, MinLimit, MaxLimit);

        public static bool operator ==(IKLink left, IKLink right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IKLink left, IKLink right)
        {
            return !(left == right);
        }
    }

    [DebuggerDisplay("DisplayFrameElement (TargetType={TargetType}, TargetIndex={TargetIndex})")]
    public struct DisplayFrameElement : IEquatable<DisplayFrameElement>
    {
        public DisplayFrameElementTarget TargetType { get; internal set; }
        public int TargetIndex { get; internal set; }

        public override bool Equals(object? obj)
        {
            return obj is DisplayFrameElement element && Equals(element);
        }

        public bool Equals(DisplayFrameElement other)
        {
            return TargetType == other.TargetType &&
                   TargetIndex == other.TargetIndex;
        }

        public override int GetHashCode() => HashCode.Combine(TargetType, TargetIndex);

        public static bool operator ==(DisplayFrameElement left, DisplayFrameElement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DisplayFrameElement left, DisplayFrameElement right)
        {
            return !(left == right);
        }
    }

    [DebuggerDisplay("AnchorRigidBody (RigidBody={RigidBody}, Vertex={Vertex}, IsNearMode={IsNearMode})")]
    public struct AnchorRigidBody : IEquatable<AnchorRigidBody>
    {
        public int RigidBody { get; internal set; }
        public int Vertex { get; internal set; }
        public bool IsNearMode { get; internal set; }

        public override bool Equals(object? obj)
        {
            return obj is AnchorRigidBody body && Equals(body);
        }

        public bool Equals(AnchorRigidBody other)
        {
            return RigidBody == other.RigidBody &&
                   Vertex == other.Vertex &&
                   IsNearMode == other.IsNearMode;
        }

        public override int GetHashCode() => HashCode.Combine(RigidBody, Vertex, IsNearMode);

        public static bool operator ==(AnchorRigidBody left, AnchorRigidBody right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AnchorRigidBody left, AnchorRigidBody right)
        {
            return !(left == right);
        }
    }

    public enum PMXVersion
    {
        /// <summary>PMX Ver 2.0</summary>
        V20 = 20,
        /// <summary>PMX Ver 2.1</summary>
        V21 = 21,
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
