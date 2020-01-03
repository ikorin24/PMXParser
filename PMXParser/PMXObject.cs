﻿#nullable enable
using System;
using System.Collections.ObjectModel;

namespace MMDTools
{
    /// <summary>PMX data object</summary>
    public class PMXObject
    {
        public PMXVersion Version { get; internal set; }

        /// <summary>Get name of pmx data</summary>
        public string Name { get; internal set; } = string.Empty;
        /// <summary>Get English name of pmx data</summary>
        public string NameEnglish { get; internal set; } = string.Empty;
        /// <summary>Get comment of pmx data</summary>
        public string Comment { get; internal set; } = string.Empty;
        /// <summary>Get English comment of pmx data</summary>
        public string CommentEnglish { get; internal set; } = string.Empty;

        public ReadOnlyCollection<Vertex> VertexList { get; internal set; } = null!;
        public ReadOnlyCollection<Surface> SurfaceList { get; internal set; } = null!;

        public ReadOnlyCollection<string> TextureList { get; internal set; } = null!;

        public ReadOnlyCollection<Material> MaterialList { get; internal set; } = null!;

        public ReadOnlyCollection<Bone> BoneList { get; internal set; } = null!;

        public ReadOnlyCollection<Morph> MorphList { get; internal set; } = null!;

        public ReadOnlyCollection<DisplayFrame> DisplayFrameList { get; internal set; } = null!;

        public ReadOnlyCollection<RigidBody> RigidBodyList { get; internal set; } = null!;

        public ReadOnlyCollection<Joint> JointList { get; internal set; } = null!;

        public ReadOnlyCollection<SoftBody> SoftBodyList { get; internal set; } = null!;

        internal PMXObject()
        {

        }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !(left == right);
        }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !(left == right);
        }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }
    }

    public class Vertex
    {
        private int _boneIndex1;
        private int _boneIndex2;
        private int _boneIndex3;
        private int _boneIndex4;
        private float _weight1;
        private float _weight2;
        private float _weight3;
        private float _weight4;
        private Vector3 _c;
        private Vector3 _r0;
        private Vector3 _r1;

        public Vector3 Posision;
        public Vector3 Normal;
        public Vector2 UV;
        public int AdditionalUVCount;
        public Vector2 AdditionalUV1;
        public Vector2 AdditionalUV2;
        public Vector2 AdditionalUV3;
        public Vector2 AdditionalUV4;
        public WeightTransformType WeightTransformType;
        public float EdgeRatio;

        public void SetBDEF1Params(int boneIndex1)
        {
            _boneIndex1 = boneIndex1;
        }

        public void SetBDEF2Params(int boneIndex1, int boneIndex2, float weight1, float weight2)
        {
            _boneIndex1 = boneIndex1;
            _boneIndex2 = boneIndex2;
            _weight1 = weight1;
            _weight2 = weight2;
        }

        public void SetBDEF4Params(int boneIndex1, int boneIndex2, int boneIndex3, int boneIndex4, float weight1, float weight2, float weight3, float weight4)
        {
            _boneIndex1 = boneIndex1;
            _boneIndex2 = boneIndex2;
            _boneIndex3 = boneIndex3;
            _boneIndex4 = boneIndex4;
            _weight1 = weight1;
            _weight2 = weight2;
            _weight3 = weight3;
            _weight4 = weight4;
        }

        public void SetSDEFParams(int boneIndex1, int boneIndex2, float weight1, float weight2, Vector3 c, Vector3 r0, Vector3 r1)
        {
            _boneIndex1 = boneIndex1;
            _boneIndex2 = boneIndex2;
            _weight1 = weight1;
            _weight2 = weight2;
            _c = c;
            _r0 = r0;
            _r1 = r1;
        }

        public void SetQDEF4Params(int boneIndex1, int boneIndex2, int boneIndex3, int boneIndex4, float weight1, float weight2, float weight3, float weight4)
        {
            _boneIndex1 = boneIndex1;
            _boneIndex2 = boneIndex2;
            _boneIndex3 = boneIndex3;
            _boneIndex4 = boneIndex4;
            _weight1 = weight1;
            _weight2 = weight2;
            _weight3 = weight3;
            _weight4 = weight4;
        }
    }

    public struct Surface
    {
        public int V1 { get; internal set; }
        public int V2 { get; internal set; }
        public int V3 { get; internal set; }
    }

    public class Material
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public Color Diffuse { get; internal set; }
        public Color Specular { get; internal set; }
        public float Shininess { get; internal set; }
        public Color Ambient { get; internal set; }
        public MaterialDrawFlag DrawFlag { get; internal set; }
        public Color EdgeColor { get; internal set; }
        public float EdgeSize { get; internal set; }
        public int Texture { get; internal set; }
        public int SphereTextre { get; internal set; }
        public SphereTextureMode SphereTextureMode { get; internal set; }
        public SharedToonMode SharedToonMode { get; internal set; }
        public int ToonTexture { get; internal set; }
        public string Memo { get; internal set; } = string.Empty;
        public int VertexCount { get; internal set; }
    }

    public class Bone
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public Vector3 Position { get; internal set; }
        public int ParentBone { get; internal set; }
        public int TransformDepth { get; internal set; }
        public BoneFlag BoneFlag { get; internal set; }
        public int ConnectedBone { get; internal set; }
        public Vector3 PositionOffset { get; internal set; }
        public int AttatchParent { get; internal set; }
        public float AttatchRatio { get; internal set; }
        public Vector3 AxisVec { get; internal set; }
        public Vector3 XAxisVec { get; internal set; }
        public Vector3 ZAxisVec { get; internal set; }
        public int Key { get; internal set; }
        public int IKTarget { get; internal set; }
        public int IterCount { get; internal set; }
        public float MaxRadianPerIter { get; internal set; }
        public int IKLinkCount { get; internal set; }
        public ReadOnlyCollection<IKLink> IKLinks { get; internal set; } = null!;
    }

    public class Morph
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public MorphTarget MorphTarget { get; internal set; }
        public MorphType MorphType { get; internal set; }
        public ReadOnlyCollection<GroupMorphElement> GroupMorphElements { get; internal set; } = null!;
        public ReadOnlyCollection<VertexMorphElement> VertexMorphElements { get; internal set; } = null!;
        public ReadOnlyCollection<BoneMorphElement> BoneMorphElements { get; internal set; } = null!;
        public ReadOnlyCollection<UVMorphElement> UVMorphElements { get; internal set; } = null!;
        public ReadOnlyCollection<MaterialMorphElement> MaterialMorphElements { get; internal set; } = null!;
        public ReadOnlyCollection<FlipMorphElement> FlipMorphElements { get; internal set; } = null!;
        public ReadOnlyCollection<ImpulseMorphElement> ImpulseMorphElements { get; internal set; } = null!;
    }

    public class GroupMorphElement
    {
        public int TargetMorph { get; internal set; }
        public float MorphRatio { get; internal set; }
    }

    public class VertexMorphElement
    {
        public int TargetVertex { get; internal set; }
        public Vector3 PosOffset { get; internal set; }
    }

    public class BoneMorphElement
    {
        public int TargetBone { get; internal set; }
        public Vector3 Translate { get; internal set; }
        public Vector4 Quaternion { get; internal set; }
    }

    public class UVMorphElement
    {
        public int TargetVertex { get; internal set; }
        public Vector4 UVOffset { get; internal set; }
    }

    public class MaterialMorphElement
    {
        public int Material { get; internal set; }
        public bool IsAllMaterialTarget => Material == -1;
        public MaterialMorphCalcMode CalcMode { get; internal set; }
        public Color Diffuse { get; internal set; }
        public Color Specular { get; internal set; }
        public float Shininess { get; internal set; }
        public Color Ambient { get; internal set; }
        public Color EdgeColor { get; internal set; }
        public float EdgeSize { get; internal set; }
        public Color TextureCoef { get; internal set; }
        public Color SphereTextureCoef { get; internal set; }
        public Color ToonTextureCoef { get; internal set; }
    }

    public class FlipMorphElement
    {
        public int TargetMorph { get; internal set; }
        public float MorphRatio { get; internal set; }
    }
    
    public class ImpulseMorphElement
    {
        public int TargetRigidBody { get; internal set; }
        public bool IsLocal { get; internal set; }
        public Vector3 Velocity { get; internal set; }
        public Vector3 RotationTorque { get; internal set; }
    }

    public class DisplayFrame
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public DisplayFrameType Type { get; internal set; }
        public ReadOnlyCollection<DisplayFrameElement> Elements { get; internal set; } = null!;
    }

    public class RigidBody
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public int Bone { get; internal set; } = -1;
        public bool HasBone => Bone != -1;
        public byte Group { get; internal set; }
        public ushort GroupTarget { get; internal set; }
        public RigidBodyShape Shape { get; internal set; }
        public Vector3 Size { get; internal set; }
        public Vector3 Position { get; internal set; }
        public Vector3 RotationRadian { get; internal set; }
        public float Mass { get; internal set; }
        public float TranslationAttenuation { get; internal set; }
        public float RotationAttenuation { get; internal set; }
        public float Recoil { get; internal set; }
        public float Friction { get; internal set; }
        public RigidBodyPhysicsType PhysicsType { get; internal set; }
    }

    public class Joint
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public JointType Type { get; internal set; }
        public int RigidBody1 { get; internal set; }
        public int RigidBody2 { get; internal set; }
        public Vector3 Position { get; internal set; }
        public Vector3 RotationRadian { get; internal set; }
        public Vector3 TranslationMinLimit { get; internal set; }
        public Vector3 TranslationMaxLimit { get; internal set; }
        public Vector3 RotationRadianMinLimit { get; internal set; }
        public Vector3 RotationRadianMaxLimit { get; internal set; }
        public Vector3 TranslationSpring { get; internal set; }
        public Vector3 RotationSpring { get; internal set; }
    }

    public class SoftBody
    {
        public string Name { get; internal set; } = string.Empty;
        public string NameEnglish { get; internal set; } = string.Empty;
        public SoftBodyShape Shape { get; internal set; }
        public int TargetMaterial { get; internal set; }
        public byte Group { get; internal set; }
        public ushort GroupTarget { get; internal set; }
        public SoftBodyModeFlag Mode { get; internal set; }
        public int BLinkDistance { get; internal set; }
        public int ClusterCount { get; internal set; }
        public float TotalMass { get; internal set; }
        public float CollisionMargin { get; internal set; }
        public SoftBodyAeroModel AeroModel { get; internal set; }
        public SoftBodyConfig Config { get; internal set; } = null!;
        public SoftBodyCluster Cluster { get; internal set; } = null!;
        public SoftBodyIteration Iteration { get; internal set; } = null!;
        public SoftBodyMaterial Material { get; internal set; } = null!;
        public ReadOnlyCollection<AnchorRigidBody> AnchorRigidBodies { get; internal set; } = null!;
        public ReadOnlyCollection<int> PinnedVertex { get; internal set; } = null!;
    }

    public class SoftBodyConfig
    {
        public float VCF { get; internal set; }
        public float DP { get; internal set; }
        public float DG { get; internal set; }
        public float LF { get; internal set; }
        public float PR { get; internal set; }
        public float VC { get; internal set; }
        public float DF { get; internal set; }
        public float MT { get; internal set; }
        public float CHR { get; internal set; }
        public float KHR { get; internal set; }
        public float SHR { get; internal set; }
        public float AHR { get; internal set; }
    }

    public class SoftBodyCluster
    {
        public float SRHR_CL { get; internal set; }
        public float SKHR_CL { get; internal set; }
        public float SSHR_CL { get; internal set; }
        public float SR_SPLT_CL { get; internal set; }
        public float SK_SPLT_CL { get; internal set; }
        public float SS_SPLT_CL { get; internal set; }
    }

    public class SoftBodyIteration
    {
        public int V_IT { get; internal set; }
        public int P_IT { get; internal set; }
        public int D_IT { get; internal set; }
        public int C_IT { get; internal set; }
    }

    public class SoftBodyMaterial
    {
        public float LST { get; internal set; }
        public float AST { get; internal set; }
        public float VST { get; internal set; }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(Bone, IsEnableAngleLimited, MinLimit, MaxLimit);
        }

        public static bool operator ==(IKLink left, IKLink right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IKLink left, IKLink right)
        {
            return !(left == right);
        }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetType, TargetIndex);
        }

        public static bool operator ==(DisplayFrameElement left, DisplayFrameElement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DisplayFrameElement left, DisplayFrameElement right)
        {
            return !(left == right);
        }
    }

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

        public override int GetHashCode()
        {
            return HashCode.Combine(RigidBody, Vertex, IsNearMode);
        }

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
        public static bool Has(this BoneFlag source, BoneFlag flag)
        {
            return (source & flag) == flag;
        }
    }
}