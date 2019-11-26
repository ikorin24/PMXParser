#nullable enable
using System;
using System.IO;
using System.Text;

namespace MMDTools
{
    /// <summary>PMX data parser class</summary>
    public class PMXPerser
    {
        /// <summary>Parse PMX data from specified file</summary>
        /// <param name="fileName">file name of PMX data</param>
        /// <returns>PMX object</returns>
        public PMXObject Parse(string fileName)
        {
            if(!File.Exists(fileName)) { throw new FileNotFoundException("The file is not found", fileName); }
            using(var stream = File.OpenRead(fileName)) {
                return Parse(stream);
            }
        }

        /// <summary>Parse PMX data from specified stream</summary>
        /// <param name="stream">stream of PMX data</param>
        /// <returns>PMX object</returns>
        public PMXObject Parse(Stream stream)
        {
            var pmx = new PMXObject();
            ParseHeader(stream, out var localInfo);
            ParseModelInfo(stream, localInfo.Encoding, pmx);
            ParseVertex(stream, localInfo.AdditionalUVCount, localInfo.BoneIndexSize);
            ParseSurface(stream, localInfo.VertexIndexSize);
            ParseTexture(stream, localInfo.Encoding);
            ParseMaterial(stream, localInfo.Encoding);
            ParseBone(stream);
            ParseMorph(stream);
            ParseDisplayFrame(stream);
            ParseRigidBody(stream);
            ParseJoint(stream);
            if(localInfo.Version == FormatVersion.V20) { return pmx; }

            // PMX version >= 2.1
            ParseSoftBody(stream);
            return pmx;
        }

        private void ParseHeader(Stream stream, out PerserLocalInfo localInfo)
        {
            Span<byte> magicWord = stackalloc byte[4];
            stream.NextBytes(ref magicWord);
            PMXValidator.ValidateMagicWord(ref magicWord);

            localInfo = new PerserLocalInfo();

            var version = (int)(stream.NextSingle() * 10);
            PMXValidator.ValidateVersion(version);
            localInfo.Version = (FormatVersion)version;

            var infoLen = stream.NextByte();
            Span<byte> info = stackalloc byte[infoLen];
            stream.NextBytes(ref info);
            PMXValidator.ValidateHeaderInfo(ref info);
            localInfo.Encoding = info[0] switch
            {
                0 => Encoding.Unicode,
                1 => Encoding.UTF8,
                _ => throw new Exception("Hey! The value is not validated!? How'd I get here?"),
            };
            localInfo.AdditionalUVCount = info[1];
            localInfo.VertexIndexSize =    (IndexSize)info[2];
            localInfo.TextureIndexSize =   (IndexSize)info[3];
            localInfo.MaterialIndexSize =  (IndexSize)info[4];
            localInfo.BoneIndexSize =      (IndexSize)info[5];
            localInfo.MorphIndexSize =     (IndexSize)info[6];
            localInfo.RigidBodyIndexSize = (IndexSize)info[7];
        }

        private void ParseModelInfo(Stream stream, Encoding encoding, PMXObject pmx)
        {
            pmx.Name =           stream.NextString(stream.NextInt32(), encoding);
            pmx.NameEnglish =    stream.NextString(stream.NextInt32(), encoding);
            pmx.Comment =        stream.NextString(stream.NextInt32(), encoding);
            pmx.CommentEnglish = stream.NextString(stream.NextInt32(), encoding);
        }

        private void ParseVertex(Stream stream, int additionalUVCount, IndexSize boneIndexSize)
        {
            var vertexCount = stream.NextInt32();
            for(int i = 0; i < vertexCount; i++) {
                var pos = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                var normal = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                var uv = new Vector2(stream.NextSingle(), stream.NextSingle());
                Span<Vector4> additionalUV = stackalloc Vector4[additionalUVCount];
                var weightType = (WeightTransformType)stream.NextByte();
                switch(weightType) {
                    case WeightTransformType.BDEF1: {
                        var boneIndex1 = stream.NextDataOfSize((byte)boneIndexSize);
                        break;
                    }
                    case WeightTransformType.BDEF2: {
                        var boneIndex1 = stream.NextDataOfSize((byte)boneIndexSize);
                        var boneIndex2 = stream.NextDataOfSize((byte)boneIndexSize);
                        var weight1 = stream.NextSingle();
                        var weight2 = 1f - weight1;
                        break;
                    }
                    case WeightTransformType.BDEF4: {
                        var boneIndex1 = stream.NextDataOfSize((byte)boneIndexSize);
                        var boneIndex2 = stream.NextDataOfSize((byte)boneIndexSize);
                        var boneIndex3 = stream.NextDataOfSize((byte)boneIndexSize);
                        var boneIndex4 = stream.NextDataOfSize((byte)boneIndexSize);
                        var weight1 = stream.NextSingle();
                        var weight2 = stream.NextSingle();
                        var weight3 = stream.NextSingle();
                        var weight4 = stream.NextSingle();
                        break;
                    }
                    case WeightTransformType.SDEF: {
                        var boneIndex1 = stream.NextDataOfSize((byte)boneIndexSize);
                        var boneIndex2 = stream.NextDataOfSize((byte)boneIndexSize);
                        var weight1 = stream.NextSingle();
                        var weight2 = 1f - weight1;
                        var c = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        var r0 = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        var r1 = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        break;
                    }
                    default:
                        throw new FormatException("Invalid weight transform type");
                }
                var edgeRatio = stream.NextSingle();
            }
        }

        private void ParseSurface(Stream stream, IndexSize vertexIndexSize)
        {
            var surfaceCount = stream.NextInt32();
            for(int i = 0; i < surfaceCount; i++) {
                var surface0 = stream.NextDataOfSize((byte)vertexIndexSize);
                var surface1 = stream.NextDataOfSize((byte)vertexIndexSize);
                var surface2 = stream.NextDataOfSize((byte)vertexIndexSize);
            }
        }

        private void ParseTexture(Stream stream, Encoding encoding)
        {
            var textureCount = stream.NextInt32();
            for(int i = 0; i < textureCount; i++) {
                var path = stream.NextString(stream.NextInt32(), encoding);
            }
        }

        private void ParseMaterial(Stream stream, Encoding encoding)
        {
            var materialCount = stream.NextInt32();
            return;
            throw new NotImplementedException();
            for(int i = 0; i < materialCount; i++) {
                var name = stream.NextString(stream.NextInt32(), encoding);
                var nameEn = stream.NextString(stream.NextInt32(), encoding);
                var diffuce = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());        // Color(r, g, b, a) 
                var specular = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());        // Color(r, g, b)
                var shininess = stream.NextSingle();
                var ambient = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());        // Color(r, g, b)
            }
        }

        private void ParseBone(Stream stream)
        {

        }

        private void ParseMorph(Stream stream)
        {

        }

        private void ParseDisplayFrame(Stream stream)
        {

        }

        private void ParseRigidBody(Stream stream)
        {

        }

        private void ParseJoint(Stream stream)
        {

        }

        private void ParseSoftBody(Stream stream)
        {

        }

        private struct PerserLocalInfo
        {
            public FormatVersion Version { get; set; }
            public Encoding Encoding { get; set; }
            public int AdditionalUVCount { get; set; }
            public IndexSize VertexIndexSize { get; set; }
            public IndexSize TextureIndexSize { get; set; }
            public IndexSize MaterialIndexSize { get; set; }
            public IndexSize BoneIndexSize { get; set; }
            public IndexSize MorphIndexSize { get; set; }
            public IndexSize RigidBodyIndexSize { get; set; }
        }

        private enum FormatVersion
        {
            V20 = 20,
            V21 = 21,
        }

        private enum IndexSize : byte
        {
            Size1 = 1,
            Size2 = 2,
            Size4 = 4,
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

        public static void ValidateVersion(int version)
        {
            Assert(version == 20 || version == 21, "Invalid or not supported version");
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
