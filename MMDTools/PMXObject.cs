#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace MMDTools
{
    /// <summary>PMX data object</summary>
    public class PMXObject
    {
        /// <summary>Get name of pmx data</summary>
        public string Name { get; internal set; } = string.Empty;
        /// <summary>Get English name of pmx data</summary>
        public string NameEnglish { get; internal set; } = string.Empty;
        /// <summary>Get comment of pmx data</summary>
        public string Comment { get; internal set; } = string.Empty;
        /// <summary>Get English comment of pmx data</summary>
        public string CommentEnglish { get; internal set; } = string.Empty;

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

    public struct IKLink : IEquatable<IKLink>
    {
        public int Bone;
        public bool IsEnableAngleLimited;
        public Vector3 MinLimit;
        public Vector3 MaxLimit;

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
        public DisplayFrameElementTarget TargetType;
        public int TargetIndex;

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

    [Flags]
    public enum MaterialDrawFlag : byte
    {
        BothSidesDrawing =       0b00000001,
        GroundShadow =           0b00000010,
        DrawingInSelfShadowMap = 0b00000100,
        SelfShadowDrawing =      0b00001000,
        EdgeDrawing =            0b00010000,
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
    }
}
