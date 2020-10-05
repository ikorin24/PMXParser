#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

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
        internal unsafe PMXObject()
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
    public struct Vertex : IEquatable<Vertex>
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

        public override bool Equals(object? obj) => obj is Vertex vertex && Equals(vertex);

        public bool Equals(Vertex other)
        {
            return Position.Equals(other.Position) &&
                   Normal.Equals(other.Normal) &&
                   UV.Equals(other.UV) &&
                   AdditionalUVCount == other.AdditionalUVCount &&
                   AdditionalUV1.Equals(other.AdditionalUV1) &&
                   AdditionalUV2.Equals(other.AdditionalUV2) &&
                   AdditionalUV3.Equals(other.AdditionalUV3) &&
                   AdditionalUV4.Equals(other.AdditionalUV4) &&
                   WeightTransformType == other.WeightTransformType &&
                   BoneIndex1 == other.BoneIndex1 &&
                   BoneIndex2 == other.BoneIndex2 &&
                   BoneIndex3 == other.BoneIndex3 &&
                   BoneIndex4 == other.BoneIndex4 &&
                   Weight1 == other.Weight1 &&
                   Weight2 == other.Weight2 &&
                   Weight3 == other.Weight3 &&
                   Weight4 == other.Weight4 &&
                   C.Equals(other.C) &&
                   R0.Equals(other.R0) &&
                   R1.Equals(other.R1) &&
                   EdgeRatio == other.EdgeRatio;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Position);
            hash.Add(Normal);
            hash.Add(UV);
            hash.Add(AdditionalUVCount);
            hash.Add(AdditionalUV1);
            hash.Add(AdditionalUV2);
            hash.Add(AdditionalUV3);
            hash.Add(AdditionalUV4);
            hash.Add(WeightTransformType);
            hash.Add(BoneIndex1);
            hash.Add(BoneIndex2);
            hash.Add(BoneIndex3);
            hash.Add(BoneIndex4);
            hash.Add(Weight1);
            hash.Add(Weight2);
            hash.Add(Weight3);
            hash.Add(Weight4);
            hash.Add(C);
            hash.Add(R0);
            hash.Add(R1);
            hash.Add(EdgeRatio);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("({V1}, {V2}, {V3})")]
    public struct Surface : IEquatable<Surface>
    {
        public int V1;
        public int V2;
        public int V3;

        public override bool Equals(object? obj) => obj is Surface surface && Equals(surface);

        public bool Equals(Surface other)
        {
            return V1 == other.V1 &&
                   V2 == other.V2 &&
                   V3 == other.V3;
        }

        public override int GetHashCode() => HashCode.Combine(V1, V2, V3);
    }

    [DebuggerDisplay("Material (Name={Name})")]
    public readonly struct Material : IEquatable<Material>
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

        public override bool Equals(object? obj) => obj is Material material && Equals(material);

        public bool Equals(Material other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   Diffuse.Equals(other.Diffuse) &&
                   Specular.Equals(other.Specular) &&
                   Shininess == other.Shininess &&
                   Ambient.Equals(other.Ambient) &&
                   DrawFlag == other.DrawFlag &&
                   EdgeColor.Equals(other.EdgeColor) &&
                   EdgeSize == other.EdgeSize &&
                   Texture == other.Texture &&
                   SphereTextre == other.SphereTextre &&
                   SphereTextureMode == other.SphereTextureMode &&
                   SharedToonMode == other.SharedToonMode &&
                   ToonTexture == other.ToonTexture &&
                   Memo.Equals(other.Memo) &&
                   VertexCount == other.VertexCount;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(NameEnglish);
            hash.Add(Diffuse);
            hash.Add(Specular);
            hash.Add(Shininess);
            hash.Add(Ambient);
            hash.Add(DrawFlag);
            hash.Add(EdgeColor);
            hash.Add(EdgeSize);
            hash.Add(Texture);
            hash.Add(SphereTextre);
            hash.Add(SphereTextureMode);
            hash.Add(SharedToonMode);
            hash.Add(ToonTexture);
            hash.Add(Memo);
            hash.Add(VertexCount);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("Bone (Name={Name})")]
    public readonly struct Bone : IEquatable<Bone>
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

        public override bool Equals(object? obj) => obj is Bone bone && Equals(bone);

        public bool Equals(Bone other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   Position.Equals(other.Position) &&
                   ParentBone == other.ParentBone &&
                   TransformDepth == other.TransformDepth &&
                   BoneFlag == other.BoneFlag &&
                   ConnectedBone == other.ConnectedBone &&
                   PositionOffset.Equals(other.PositionOffset) &&
                   AttatchParent == other.AttatchParent &&
                   AttatchRatio == other.AttatchRatio &&
                   AxisVec.Equals(other.AxisVec) &&
                   XAxisVec.Equals(other.XAxisVec) &&
                   ZAxisVec.Equals(other.ZAxisVec) &&
                   Key == other.Key &&
                   IKTarget == other.IKTarget &&
                   IterCount == other.IterCount &&
                   MaxRadianPerIter == other.MaxRadianPerIter &&
                   IKLinks.Equals(other.IKLinks);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(NameEnglish);
            hash.Add(Position);
            hash.Add(ParentBone);
            hash.Add(TransformDepth);
            hash.Add(BoneFlag);
            hash.Add(ConnectedBone);
            hash.Add(PositionOffset);
            hash.Add(AttatchParent);
            hash.Add(AttatchRatio);
            hash.Add(AxisVec);
            hash.Add(XAxisVec);
            hash.Add(ZAxisVec);
            hash.Add(Key);
            hash.Add(IKTarget);
            hash.Add(IterCount);
            hash.Add(MaxRadianPerIter);
            hash.Add(IKLinks);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("Morph (Name={Name})")]
    public readonly struct Morph : IEquatable<Morph>
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

        public override bool Equals(object? obj) => obj is Morph morph && Equals(morph);

        public bool Equals(Morph other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   MorphTarget == other.MorphTarget &&
                   MorphType == other.MorphType &&
                   GroupMorphElements.Equals(other.GroupMorphElements) &&
                   VertexMorphElements.Equals(other.VertexMorphElements) &&
                   BoneMorphElements.Equals(other.BoneMorphElements) &&
                   UVMorphElements.Equals(other.UVMorphElements) &&
                   MaterialMorphElements.Equals(other.MaterialMorphElements) &&
                   FlipMorphElements.Equals(other.FlipMorphElements) &&
                   ImpulseMorphElements.Equals(other.ImpulseMorphElements);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(NameEnglish);
            hash.Add(MorphTarget);
            hash.Add(MorphType);
            hash.Add(GroupMorphElements);
            hash.Add(VertexMorphElements);
            hash.Add(BoneMorphElements);
            hash.Add(UVMorphElements);
            hash.Add(MaterialMorphElements);
            hash.Add(FlipMorphElements);
            hash.Add(ImpulseMorphElements);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("GroupMorphElement (TargetMorph={TargetMorph})")]
    public struct GroupMorphElement : IEquatable<GroupMorphElement>
    {
        public int TargetMorph;
        public float MorphRatio;

        public override bool Equals(object? obj) => obj is GroupMorphElement element && Equals(element);

        public bool Equals(GroupMorphElement other)
        {
            return TargetMorph == other.TargetMorph &&
                   MorphRatio == other.MorphRatio;
        }

        public override int GetHashCode() => HashCode.Combine(TargetMorph, MorphRatio);
    }

    [DebuggerDisplay("VertexMorphElement (TargetVertex={TargetVertex})")]
    public struct VertexMorphElement : IEquatable<VertexMorphElement>
    {
        public int TargetVertex;
        public Vector3 PosOffset;

        public override bool Equals(object? obj) => obj is VertexMorphElement element && Equals(element);

        public bool Equals(VertexMorphElement other)
        {
            return TargetVertex == other.TargetVertex &&
                   PosOffset.Equals(other.PosOffset);
        }

        public override int GetHashCode() => HashCode.Combine(TargetVertex, PosOffset);
    }

    [DebuggerDisplay("BoneMorphElement (TargetBone={TargetBone})")]
    public struct BoneMorphElement : IEquatable<BoneMorphElement>
    {
        public int TargetBone;
        public Vector3 Translate;
        public Vector4 Quaternion;

        public override bool Equals(object? obj) => obj is BoneMorphElement element && Equals(element);

        public bool Equals(BoneMorphElement other)
        {
            return TargetBone == other.TargetBone &&
                   Translate.Equals(other.Translate) &&
                   Quaternion.Equals(other.Quaternion);
        }

        public override int GetHashCode() => HashCode.Combine(TargetBone, Translate, Quaternion);
    }

    [DebuggerDisplay("UVMorphElement (TargetVertex={TargetVertex})")]
    public struct UVMorphElement : IEquatable<UVMorphElement>
    {
        public int TargetVertex;
        public Vector4 UVOffset;

        public override bool Equals(object? obj) => obj is UVMorphElement element && Equals(element);

        public bool Equals(UVMorphElement other)
        {
            return TargetVertex == other.TargetVertex &&
                   UVOffset.Equals(other.UVOffset);
        }

        public override int GetHashCode() => HashCode.Combine(TargetVertex, UVOffset);
    }

    [DebuggerDisplay("MaterialMorphElement (Material={Material})")]
    public struct MaterialMorphElement : IEquatable<MaterialMorphElement>
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

        public override bool Equals(object? obj) => obj is MaterialMorphElement element && Equals(element);

        public bool Equals(MaterialMorphElement other)
        {
            return Material == other.Material &&
                   IsAllMaterialTarget == other.IsAllMaterialTarget &&
                   CalcMode == other.CalcMode &&
                   Diffuse.Equals(other.Diffuse) &&
                   Specular.Equals(other.Specular) &&
                   Shininess == other.Shininess &&
                   Ambient.Equals(other.Ambient) &&
                   EdgeColor.Equals(other.EdgeColor) &&
                   EdgeSize == other.EdgeSize &&
                   TextureCoef.Equals(other.TextureCoef) &&
                   SphereTextureCoef.Equals(other.SphereTextureCoef) &&
                   ToonTextureCoef.Equals(other.ToonTextureCoef);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Material);
            hash.Add(IsAllMaterialTarget);
            hash.Add(CalcMode);
            hash.Add(Diffuse);
            hash.Add(Specular);
            hash.Add(Shininess);
            hash.Add(Ambient);
            hash.Add(EdgeColor);
            hash.Add(EdgeSize);
            hash.Add(TextureCoef);
            hash.Add(SphereTextureCoef);
            hash.Add(ToonTextureCoef);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("FlipMorphElement (TargetMorph={TargetMorph})")]
    public struct FlipMorphElement : IEquatable<FlipMorphElement>
    {
        public int TargetMorph;
        public float MorphRatio;

        public override bool Equals(object? obj) => obj is FlipMorphElement element && Equals(element);

        public bool Equals(FlipMorphElement other)
        {
            return TargetMorph == other.TargetMorph &&
                   MorphRatio == other.MorphRatio;
        }

        public override int GetHashCode() => HashCode.Combine(TargetMorph, MorphRatio);
    }

    [DebuggerDisplay("ImpulseMorphElement (TargetRigidBody={TargetRigidBody})")]
    public struct ImpulseMorphElement : IEquatable<ImpulseMorphElement>
    {
        public int TargetRigidBody;
        public bool IsLocal;
        public Vector3 Velocity;
        public Vector3 RotationTorque;

        public override bool Equals(object? obj) => obj is ImpulseMorphElement element && Equals(element);

        public bool Equals(ImpulseMorphElement other)
        {
            return TargetRigidBody == other.TargetRigidBody &&
                   IsLocal == other.IsLocal &&
                   Velocity.Equals(other.Velocity) &&
                   RotationTorque.Equals(other.RotationTorque);
        }

        public override int GetHashCode() => HashCode.Combine(TargetRigidBody, IsLocal, Velocity, RotationTorque);
    }
    
    [DebuggerDisplay("DisplayFrame (Name={Name})")]
    public readonly struct DisplayFrame : IEquatable<DisplayFrame>
    {
        public readonly ReadOnlyRawString Name;
        public readonly ReadOnlyRawString NameEnglish;
        public readonly DisplayFrameType Type;
        public readonly ReadOnlyRawArray<DisplayFrameElement> Elements;

        public override bool Equals(object? obj) => obj is DisplayFrame frame && Equals(frame);

        public bool Equals(DisplayFrame other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   Type == other.Type &&
                   Elements.Equals(other.Elements);
        }

        public override int GetHashCode() => HashCode.Combine(Name, NameEnglish, Type, Elements);
    }

    [DebuggerDisplay("RigidBody (Name={Name})")]
    public readonly struct RigidBody : IEquatable<RigidBody>
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

        public override bool Equals(object? obj) => obj is RigidBody body && Equals(body);

        public bool Equals(RigidBody other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   Bone == other.Bone &&
                   HasBone == other.HasBone &&
                   Group == other.Group &&
                   GroupTarget == other.GroupTarget &&
                   Shape == other.Shape &&
                   Size.Equals(other.Size) &&
                   Position.Equals(other.Position) &&
                   RotationRadian.Equals(other.RotationRadian) &&
                   Mass == other.Mass &&
                   TranslationAttenuation == other.TranslationAttenuation &&
                   RotationAttenuation == other.RotationAttenuation &&
                   Recoil == other.Recoil &&
                   Friction == other.Friction &&
                   PhysicsType == other.PhysicsType;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(NameEnglish);
            hash.Add(Bone);
            hash.Add(HasBone);
            hash.Add(Group);
            hash.Add(GroupTarget);
            hash.Add(Shape);
            hash.Add(Size);
            hash.Add(Position);
            hash.Add(RotationRadian);
            hash.Add(Mass);
            hash.Add(TranslationAttenuation);
            hash.Add(RotationAttenuation);
            hash.Add(Recoil);
            hash.Add(Friction);
            hash.Add(PhysicsType);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("Joint (Name={Name})")]
    public readonly struct Joint : IEquatable<Joint>
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

        public override bool Equals(object? obj) => obj is Joint joint && Equals(joint);

        public bool Equals(Joint other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   Type == other.Type &&
                   RigidBody1 == other.RigidBody1 &&
                   RigidBody2 == other.RigidBody2 &&
                   Position.Equals(other.Position) &&
                   RotationRadian.Equals(other.RotationRadian) &&
                   TranslationMinLimit.Equals(other.TranslationMinLimit) &&
                   TranslationMaxLimit.Equals(other.TranslationMaxLimit) &&
                   RotationRadianMinLimit.Equals(other.RotationRadianMinLimit) &&
                   RotationRadianMaxLimit.Equals(other.RotationRadianMaxLimit) &&
                   TranslationSpring.Equals(other.TranslationSpring) &&
                   RotationSpring.Equals(other.RotationSpring);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(NameEnglish);
            hash.Add(Type);
            hash.Add(RigidBody1);
            hash.Add(RigidBody2);
            hash.Add(Position);
            hash.Add(RotationRadian);
            hash.Add(TranslationMinLimit);
            hash.Add(TranslationMaxLimit);
            hash.Add(RotationRadianMinLimit);
            hash.Add(RotationRadianMaxLimit);
            hash.Add(TranslationSpring);
            hash.Add(RotationSpring);
            return hash.ToHashCode();
        }
    }

    [DebuggerDisplay("SoftBody (Name={Name})")]
    public readonly struct SoftBody : IEquatable<SoftBody>
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

        public override bool Equals(object? obj) => obj is SoftBody body && Equals(body);

        public bool Equals(SoftBody other)
        {
            return Name.Equals(other.Name) &&
                   NameEnglish.Equals(other.NameEnglish) &&
                   Shape == other.Shape &&
                   TargetMaterial == other.TargetMaterial &&
                   Group == other.Group &&
                   GroupTarget == other.GroupTarget &&
                   Mode == other.Mode &&
                   BLinkDistance == other.BLinkDistance &&
                   ClusterCount == other.ClusterCount &&
                   TotalMass == other.TotalMass &&
                   CollisionMargin == other.CollisionMargin &&
                   AeroModel == other.AeroModel &&
                   Config.Equals(other.Config) &&
                   Cluster.Equals(other.Cluster) &&
                   Iteration.Equals(other.Iteration) &&
                   Material.Equals(other.Material) &&
                   AnchorRigidBodies.Equals(other.AnchorRigidBodies) &&
                   PinnedVertex.Equals(other.PinnedVertex);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(NameEnglish);
            hash.Add(Shape);
            hash.Add(TargetMaterial);
            hash.Add(Group);
            hash.Add(GroupTarget);
            hash.Add(Mode);
            hash.Add(BLinkDistance);
            hash.Add(ClusterCount);
            hash.Add(TotalMass);
            hash.Add(CollisionMargin);
            hash.Add(AeroModel);
            hash.Add(Config);
            hash.Add(Cluster);
            hash.Add(Iteration);
            hash.Add(Material);
            hash.Add(AnchorRigidBodies);
            hash.Add(PinnedVertex);
            return hash.ToHashCode();
        }
    }

    public struct SoftBodyConfig : IEquatable<SoftBodyConfig>
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

        public override bool Equals(object? obj) => obj is SoftBodyConfig config && Equals(config);

        public bool Equals(SoftBodyConfig other)
        {
            return VCF == other.VCF &&
                   DP == other.DP &&
                   DG == other.DG &&
                   LF == other.LF &&
                   PR == other.PR &&
                   VC == other.VC &&
                   DF == other.DF &&
                   MT == other.MT &&
                   CHR == other.CHR &&
                   KHR == other.KHR &&
                   SHR == other.SHR &&
                   AHR == other.AHR;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(VCF);
            hash.Add(DP);
            hash.Add(DG);
            hash.Add(LF);
            hash.Add(PR);
            hash.Add(VC);
            hash.Add(DF);
            hash.Add(MT);
            hash.Add(CHR);
            hash.Add(KHR);
            hash.Add(SHR);
            hash.Add(AHR);
            return hash.ToHashCode();
        }
    }

    public struct SoftBodyCluster : IEquatable<SoftBodyCluster>
    {
        public float SRHR_CL;
        public float SKHR_CL;
        public float SSHR_CL;
        public float SR_SPLT_CL;
        public float SK_SPLT_CL;
        public float SS_SPLT_CL;

        public override bool Equals(object? obj) => obj is SoftBodyCluster cluster && Equals(cluster);

        public bool Equals(SoftBodyCluster other)
        {
            return SRHR_CL == other.SRHR_CL &&
                   SKHR_CL == other.SKHR_CL &&
                   SSHR_CL == other.SSHR_CL &&
                   SR_SPLT_CL == other.SR_SPLT_CL &&
                   SK_SPLT_CL == other.SK_SPLT_CL &&
                   SS_SPLT_CL == other.SS_SPLT_CL;
        }

        public override int GetHashCode() => HashCode.Combine(SRHR_CL, SKHR_CL, SSHR_CL, SR_SPLT_CL, SK_SPLT_CL, SS_SPLT_CL);
    }

    public struct SoftBodyIteration : IEquatable<SoftBodyIteration>
    {
        public int V_IT;
        public int P_IT;
        public int D_IT;
        public int C_IT;

        public override bool Equals(object? obj) => obj is SoftBodyIteration iteration && Equals(iteration);

        public bool Equals(SoftBodyIteration other)
        {
            return V_IT == other.V_IT &&
                   P_IT == other.P_IT &&
                   D_IT == other.D_IT &&
                   C_IT == other.C_IT;
        }

        public override int GetHashCode() => HashCode.Combine(V_IT, P_IT, D_IT, C_IT);
    }

    public struct SoftBodyMaterial : IEquatable<SoftBodyMaterial>
    {
        public float LST;
        public float AST;
        public float VST;

        public override bool Equals(object? obj) => obj is SoftBodyMaterial material && Equals(material);

        public bool Equals(SoftBodyMaterial other)
        {
            return LST == other.LST &&
                   AST == other.AST &&
                   VST == other.VST;
        }

        public override int GetHashCode() => HashCode.Combine(LST, AST, VST);
    }


    [DebuggerDisplay("IKLink (Bone={Bone})")]
    public struct IKLink : IEquatable<IKLink>
    {
        public int Bone;
        public bool IsEnableAngleLimited;
        public Vector3 MinLimit;
        public Vector3 MaxLimit;

        public override bool Equals(object? obj) => obj is IKLink link && Equals(link);

        public bool Equals(IKLink other)
        {
            return Bone == other.Bone &&
                   IsEnableAngleLimited == other.IsEnableAngleLimited &&
                   MinLimit.Equals(other.MinLimit) &&
                   MaxLimit.Equals(other.MaxLimit);
        }

        public override int GetHashCode() => HashCode.Combine(Bone, IsEnableAngleLimited, MinLimit, MaxLimit);
    }

    [DebuggerDisplay("DisplayFrameElement (TargetType={TargetType}, TargetIndex={TargetIndex})")]
    public struct DisplayFrameElement : IEquatable<DisplayFrameElement>
    {
        public DisplayFrameElementTarget TargetType;
        public int TargetIndex;

        public override bool Equals(object? obj) => obj is DisplayFrameElement element && Equals(element);

        public bool Equals(DisplayFrameElement other)
        {
            return TargetType == other.TargetType &&
                   TargetIndex == other.TargetIndex;
        }

        public override int GetHashCode() => HashCode.Combine(TargetType, TargetIndex);
    }

    [DebuggerDisplay("AnchorRigidBody (RigidBody={RigidBody}, Vertex={Vertex}, IsNearMode={IsNearMode})")]
    public struct AnchorRigidBody : IEquatable<AnchorRigidBody>
    {
        public int RigidBody;
        public int Vertex;
        public bool IsNearMode;

        public override bool Equals(object? obj) => obj is AnchorRigidBody body && Equals(body);

        public bool Equals(AnchorRigidBody other)
        {
            return RigidBody == other.RigidBody &&
                   Vertex == other.Vertex &&
                   IsNearMode == other.IsNearMode;
        }

        public override int GetHashCode() => HashCode.Combine(RigidBody, Vertex, IsNearMode);
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
