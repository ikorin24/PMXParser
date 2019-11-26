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

    public enum WeightTransformType : byte
    {
        BDEF1 = 0,
        BDEF2 = 1,
        BDEF4 = 2,
        SDEF = 3,
    }
}
