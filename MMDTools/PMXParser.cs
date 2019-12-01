#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MMDTools
{
    /// <summary>PMX data parser class</summary>
    public static class PMXParser
    {
        /// <summary>Parse PMX data from specified file</summary>
        /// <param name="fileName">file name of PMX data</param>
        /// <returns>PMX object</returns>
        public static PMXObject Parse(string fileName)
        {
            if(!File.Exists(fileName)) { throw new FileNotFoundException("The file is not found", fileName); }
            using(var stream = File.OpenRead(fileName)) {
                return Parse(stream);
            }
        }

        /// <summary>Parse PMX data from specified stream</summary>
        /// <param name="stream">stream of PMX data</param>
        /// <returns>PMX object</returns>
        public static PMXObject Parse(Stream stream)
        {
            var pmx = new PMXObject();
            ParseHeader(stream, out var localInfo);
            ParseModelInfo(stream, ref localInfo, pmx);
            ParseVertex(stream, ref localInfo, pmx);
            ParseSurface(stream, ref localInfo, pmx);
            ParseTexture(stream, ref localInfo, pmx);
            ParseMaterial(stream, ref localInfo, pmx);
            ParseBone(stream, ref localInfo, pmx);
            ParseMorph(stream, ref localInfo, pmx);
            ParseDisplayFrame(stream, ref localInfo, pmx);
            ParseRigidBody(stream, ref localInfo, pmx);
            ParseJoint(stream, ref localInfo, pmx);
            ParseSoftBody(stream, ref localInfo, pmx);
            return pmx;
        }

        private static void ParseHeader(Stream stream, out ParserLocalInfo localInfo)
        {
            Span<byte> magicWord = stackalloc byte[4];
            stream.NextBytes(ref magicWord);
            PMXValidator.ValidateMagicWord(ref magicWord);

            var version = (PMXVersion)(int)(stream.NextSingle() * 10);
            PMXValidator.ValidateVersion(version);

            var infoLen = stream.NextByte();
            Span<byte> info = stackalloc byte[infoLen];
            stream.NextBytes(ref info);
            PMXValidator.ValidateHeaderInfo(ref info);
            var encoding = info[0] switch
            {
                0 => Encoding.Unicode,
                1 => Encoding.UTF8,
                _ => throw new Exception("Hey! The value is not validated!? How'd I get here?"),
            };
            localInfo = new ParserLocalInfo(version, encoding, info[1], info[2], info[3], info[4], info[5], info[6], info[7]);
        }

        private static void ParseModelInfo(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            pmx.Name =           stream.NextString(stream.NextInt32(), localInfo.Encoding);
            pmx.NameEnglish =    stream.NextString(stream.NextInt32(), localInfo.Encoding);
            pmx.Comment =        stream.NextString(stream.NextInt32(), localInfo.Encoding);
            pmx.CommentEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
        }

        private static void ParseVertex(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var vertexCount = stream.NextInt32();
            var vertexList = new List<Vertex>(vertexCount);
            for(int i = 0; i < vertexCount; i++) {
                var vertex = new Vertex();
                vertex.Posision = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                vertex.Normal = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                vertex.UV = new Vector2(stream.NextSingle(), stream.NextSingle());
                vertex.AdditionalUVCount = localInfo.AdditionalUVCount;
                Span<Vector4> additionalUV = stackalloc Vector4[localInfo.AdditionalUVCount];
                vertex.WeightTransformType = (WeightTransformType)stream.NextByte();
                switch(vertex.WeightTransformType) {
                    case WeightTransformType.BDEF1: {
                        var boneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.SetBDEF1Params(boneIndex1);
                        break;
                    }
                    case WeightTransformType.BDEF2: {
                        var boneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var boneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var weight1 = stream.NextSingle();
                        var weight2 = 1f - weight1;
                        vertex.SetBDEF2Params(boneIndex1, boneIndex2, weight1, weight2);
                        break;
                    }
                    case WeightTransformType.BDEF4: {
                        var boneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var boneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var boneIndex3 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var boneIndex4 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var weight1 = stream.NextSingle();
                        var weight2 = stream.NextSingle();
                        var weight3 = stream.NextSingle();
                        var weight4 = stream.NextSingle();
                        vertex.SetBDEF4Params(boneIndex1, boneIndex2, boneIndex3, boneIndex4, weight1, weight2, weight3, weight4);
                        break;
                    }
                    case WeightTransformType.SDEF: {
                        var boneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var boneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        var weight1 = stream.NextSingle();
                        var weight2 = 1f - weight1;
                        var c = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        var r0 = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        var r1 = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        vertex.SetSDEFParams(boneIndex1, boneIndex2, weight1, weight2, c, r0, r1);
                        break;
                    }
                    default:
                        throw new FormatException("Invalid weight transform type");
                }
                vertex.EdgeRatio = stream.NextSingle();
            }
            pmx.VertexList = vertexList.AsReadOnly();
        }

        private static void ParseSurface(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var surfaceCount = stream.NextInt32();
            var surfaceList = new List<int>(surfaceCount);
            pmx.SurfaceList = surfaceList.AsReadOnly();
            for(int i = 0; i < surfaceCount; i++) {
                surfaceList.Add(stream.NextDataOfSize(localInfo.VertexIndexSize));
            }
        }

        private static void ParseTexture(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var textureCount = stream.NextInt32();
            var textureList = new List<string>(textureCount);
            pmx.TextureList = textureList.AsReadOnly();
            for(int i = 0; i < textureCount; i++) {
                textureList.Add(stream.NextString(stream.NextInt32(), localInfo.Encoding));
            }
        }

        private static void ParseMaterial(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var materialCount = stream.NextInt32();
            var materialList = new List<Material>(materialCount);
            pmx.MaterialList = materialList.AsReadOnly();
            for(int i = 0; i < materialCount; i++) {
                var material = new Material();
                material.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                material.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                material.Diffuse = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                material.Specular = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                material.Shininess = stream.NextSingle();
                material.Ambient = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                material.DrawFlag = (MaterialDrawFlag)stream.NextByte();
                material.EdgeColor = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                material.EdgeSize = stream.NextSingle();
                material.Texture = stream.NextDataOfSize(localInfo.TextureIndexSize);
                material.SphereTextre = stream.NextDataOfSize(localInfo.TextureIndexSize);
                material.SphereTextureMode = (SphereTextureMode)stream.NextByte();
                material.SharedToonMode = (SharedToonMode)stream.NextByte();
                switch(material.SharedToonMode) {
                    case SharedToonMode.TextureIndex: {
                        material.ToonTexture = stream.NextDataOfSize(localInfo.TextureIndexSize);
                        break;
                    }
                    case SharedToonMode.SharedToon: {
                        material.ToonTexture = stream.NextByte();
                        break;
                    }
                    default:
                        throw new FormatException("Invalid shared toon mode");
                }

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
                // If memo is not useful, use following code instead.
#if true
                material.Memo = stream.NextString(stream.NextInt32(), localInfo.Encoding);
#else
                stream.Skip(stream.NextInt32());
#endif
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
                material.VertexCount = stream.NextInt32();   // always can be devided by three
            }
        }

        private static void ParseBone(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var boneCount = stream.NextInt32();
            for(int i = 0; i < boneCount; i++) {
                var boneName = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var boneNameEn = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var position = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                var parentBone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                var transformDepth = stream.NextInt32();
                var boneflag = (BoneFlag)stream.NextInt16();
                if((boneflag & BoneFlag.ConnectionDestination) != BoneFlag.ConnectionDestination) {
                    var positionOffset = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                }
                else {
                    var connectedBone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                }
                if((boneflag & BoneFlag.RotationAttach) == BoneFlag.RotationAttach ||
                   (boneflag & BoneFlag.TranslationAttach) == BoneFlag.TranslationAttach) {
                    var attachParent = stream.NextDataOfSize(localInfo.BoneIndexSize);
                    var attachRatio = stream.NextSingle();
                }
                if((boneflag & BoneFlag.FixedAxis) == BoneFlag.FixedAxis) {
                    var axisVec = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                }
                if((boneflag & BoneFlag.LocalAxis) == BoneFlag.LocalAxis) {
                    var xAxisVec = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                    var zAxisVec = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                }
                if((boneflag & BoneFlag.ExternalParentTransform) == BoneFlag.ExternalParentTransform) {
                    var key = stream.NextInt32();
                }
                if((boneflag & BoneFlag.IK) == BoneFlag.IK) {
                    var ikTarget = stream.NextDataOfSize(localInfo.BoneIndexSize);
                    var iterCount = stream.NextInt32();
                    var maxRadianPerIter = stream.NextSingle();
                    var ikLinkCount = stream.NextInt32();

                    // Stack overflow maybe happen...
                    //Span<IKLink> ikLinks = stackalloc IKLink[ikLinkCount];
                    const int STACK_LIMIT = 1024 * 16;
                    Span<IKLink> ikLinks = ((uint)ikLinkCount <= STACK_LIMIT) ? stackalloc IKLink[ikLinkCount] : new IKLink[ikLinkCount];
                    for(int j = 0; j < ikLinks.Length; j++) {
                        ikLinks[j].Bone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        ikLinks[j].IsEnableAngleLimited = stream.NextByte() switch
                        {
                            0 => false,
                            1 => true,
                            _ => throw new FormatException("Invalid value"),
                        };
                        if(ikLinks[j].IsEnableAngleLimited) {
                            ikLinks[j].MinLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            ikLinks[j].MaxLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                    }

                }
            }
        }

        private static void ParseMorph(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var morphCount = stream.NextInt32();
            for(int i = 0; i < morphCount; i++) {
                var morphName = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var morphNameEn = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var target = (MorphTarget)stream.NextByte();
                var type = (MorphType)stream.NextByte();
                var offsetCount = stream.NextInt32();
                switch(type) {
                    case MorphType.Group: {
                        for(int j = 0; j < offsetCount; j++) {
                            var morph = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            var morphRatio = stream.NextSingle();
                        }
                        break;
                    }
                    case MorphType.Vertex: {
                        for(int j = 0; j < offsetCount; j++) {
                            var vertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            var posOffset = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Bone: {
                        for(int j = 0; j < offsetCount; j++) {
                            var bone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                            var translate = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var quaternion = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.UV:
                    case MorphType.AdditionalUV1:
                    case MorphType.AdditionalUV2:
                    case MorphType.AdditionalUV3:
                    case MorphType.AdditionalUV4: {
                        for(int j = 0; j < offsetCount; j++) {
                            var vertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            var uvOffset = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Material: {
                        for(int j = 0; j < offsetCount; j++) {
                            var material = stream.NextDataOfSize(localInfo.MaterialIndexSize);
                            var isAllMaterialTarget = material == -1;
                            var calcType = (MaterialMorphCalcMode)stream.NextByte();
                            var diffuse = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var specular = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var shininess = stream.NextSingle();
                            var ambient = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var edgeColor = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var edgeSize = stream.NextSingle();
                            var textureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var sphereTextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            var toonTextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    default:
                        break;
                }
            }
        }

        private static void ParseDisplayFrame(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var displayFrameCount = stream.NextInt32();
            for(int i = 0; i < displayFrameCount; i++) {
                var displayFrameName = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var displayFrameNameEn = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var type = (DisplayFrameType)stream.NextByte();
                var elementCount = stream.NextInt32();
                for(int j = 0; j < elementCount; j++) {
                    var element = new DisplayFrameElement();
                    element.TargetType = (DisplayFrameElementTarget)stream.NextByte();
                    element.TargetIndex = element.TargetType switch
                    {
                        DisplayFrameElementTarget.Bone => stream.NextDataOfSize(localInfo.BoneIndexSize),
                        DisplayFrameElementTarget.Morph => stream.NextDataOfSize(localInfo.MorphIndexSize),
                        _ => throw new FormatException("Invalid element type"),
                    };
                }
            }
        }

        private static void ParseRigidBody(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var rigidBodyCount = stream.NextInt32();
            for(int i = 0; i < rigidBodyCount; i++) {
                var rigidBodyName = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var rigidBodyNameEn = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                var bone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                var hasBone = bone != -1;
                var group = stream.NextByte();
                var collisionInvisibleFlag = stream.NextUint16();
                var shape = (RigidBodyShape)stream.NextByte();
                var size = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                var position = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                var rotationRadian = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                var mass = stream.NextSingle();
                var translationAttenuation = stream.NextSingle();
                var rotationAttenuation = stream.NextSingle();
                var repulsion = stream.NextSingle();
                var friction = stream.NextSingle();
                var physicsType = (RigidBodyPhysicsType)stream.NextByte();
            }
        }

        private static void ParseJoint(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {

        }

        private static void ParseSoftBody(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            if(localInfo.Version == PMXVersion.V20) { return; }
        }

        private struct ParserLocalInfo
        {
            public PMXVersion Version { get; private set; }
            public Encoding Encoding { get; private set; }
            public int AdditionalUVCount { get; private set; }
            public byte VertexIndexSize { get; private set; }
            public byte TextureIndexSize { get; private set; }
            public byte MaterialIndexSize { get; private set; }
            public byte BoneIndexSize { get; private set; }
            public byte MorphIndexSize { get; private set; }
            public byte RigidBodyIndexSize { get; private set; }

            public ParserLocalInfo(PMXVersion version, Encoding encoding, int additonalUVCount,
                                   byte vertexIndexSize, byte textureIndexSize, byte materialIndexSize,
                                   byte boneIndexSize, byte morphIndexSize, byte rigidBodyIndexSize)
            {
                Version = version;
                Encoding = encoding;
                AdditionalUVCount = additonalUVCount;
                VertexIndexSize = vertexIndexSize;
                TextureIndexSize = textureIndexSize;
                MaterialIndexSize = materialIndexSize;
                BoneIndexSize = boneIndexSize;
                MorphIndexSize = morphIndexSize;
                RigidBodyIndexSize = rigidBodyIndexSize;
            }
        }
    }

    internal static class PMXValidator
    {
        public static void ValidateMagicWord(ref Span<byte> magicWord)
        {
            // magic word : "PMX "
            Assert(magicWord[0] == 0x50 &&
                   magicWord[1] == 0x4d &&
                   magicWord[2] == 0x58 &&
                   magicWord[3] == 0x20,
                   $"Invalid magic word");
        }

        public static void ValidateVersion(PMXVersion version)
        {
            Assert(version == PMXVersion.V20 || version == PMXVersion.V21, "Invalid or not supported version");
        }

        public static void ValidateHeaderInfo(ref Span<byte> info)
        {
            Assert(info[0] == 0 || info[0] == 1, "Invalid encode type");
            Assert(info[1] >= 0 || info[1] <= 4, "Invalid additional UV count");
            Assert(info[2] == 1 || info[2] <= 2 || info[2] == 4, "Invalid vertex index size");
            Assert(info[3] == 1 || info[3] <= 2 || info[3] == 4, "Invalid texture index size");
            Assert(info[4] == 1 || info[4] <= 2 || info[4] == 4, "Invalid material index size");
            Assert(info[5] == 1 || info[5] <= 2 || info[5] == 4, "Invalid bone index size");
            Assert(info[6] == 1 || info[6] <= 2 || info[6] == 4, "Invalid morph index size");
            Assert(info[7] == 1 || info[7] <= 2 || info[7] == 4, "Invalid rigid body index size");
        }

        private static void Assert(bool condition, string message)
        {
            if(!condition) { throw new FormatException(message); }
        }
    }

    internal static class StreamExtension
    {
        // Do not rename or remove (Used in conditional compiled code)
        public static void Skip(this Stream source, int byteSize)
        {
            // This is bad because some types of Stream throws NotSupportedException. (e.g. NetworkStream)
            // source.Position += byteSize

            // Use following instead.
            Span<byte> buf = stackalloc byte[byteSize];
            source.NextBytes(ref buf);
        }

        public static string NextString(this Stream source, int byteSize, Encoding encoding)
        {
            Span<byte> buf = stackalloc byte[byteSize];
            Read(source, ref buf);
            return encoding.GetString(buf);
        }

        public static int NextInt32(this Stream source)
        {
            Span<byte> buf = stackalloc byte[sizeof(int)];
            Read(source, ref buf);
            return BitConverter.ToInt32(buf);
        }

        public static short NextInt16(this Stream source)
        {
            Span<byte> buf = stackalloc byte[sizeof(short)];
            Read(source, ref buf);
            return BitConverter.ToInt16(buf);
        }

        public static ushort NextUint16(this Stream source)
        {
            Span<byte> buf = stackalloc byte[sizeof(ushort)];
            Read(source, ref buf);
            return BitConverter.ToUInt16(buf);
        }

        public static uint NextUInt32(this Stream source)
        {
            Span<byte> buf = stackalloc byte[sizeof(uint)];
            Read(source, ref buf);
            return BitConverter.ToUInt32(buf);
        }

        public static float NextSingle(this Stream source)
        {
            Span<byte> buf = stackalloc byte[sizeof(float)];
            Read(source, ref buf);
            return BitConverter.ToSingle(buf);
        }

        public static byte NextByte(this Stream source)
        {
            Span<byte> buf = stackalloc byte[sizeof(byte)];
            Read(source, ref buf);
            return buf[0];
        }

        public static void NextBytes(this Stream source, ref Span<byte> buf)
        {
            Read(source, ref buf);
        }

        public static int NextDataOfSize(this Stream source, byte byteSize)
        {
            // byteSize must be [1 <= byteSize <= 4]

            Span<byte> buf = stackalloc byte[4];
            var sliced = buf.Slice(buf.Length - byteSize, byteSize);     // for little endian
            Read(source, ref sliced);
            return BitConverter.ToInt32(buf);
        }



        private static void Read(Stream stream, ref Span<byte> buf)
        {
            if(stream.Read(buf) != buf.Length) { throw new EndOfStreamException(); }
        }
    }
}
