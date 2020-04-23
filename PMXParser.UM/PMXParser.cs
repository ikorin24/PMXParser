#nullable enable
using System;
using System.IO;
using System.Text;
using MMDTools;

namespace MMDTools.Unmanaged
{
    public unsafe static class PMXParser
    {
        public static PMXObject Parse(string fileName)
        {
            if(!File.Exists(fileName)) { throw new FileNotFoundException("The file is not found", fileName); }
            using(var stream = File.OpenRead(fileName)) {
                return Parse(stream);
            }
        }

        public static PMXObject Parse(Stream stream)
        {
            PMXObject pmx = default;
            try {
                pmx = PMXObject.New();
                ParseHeader(stream, out var localInfo, pmx.Entity);
                ParseModelInfo(stream, in localInfo, pmx.Entity);
                //ParseVertex(stream, ref localInfo, pmx);
                //ParseSurface(stream, ref localInfo, pmx);
                //ParseTexture(stream, ref localInfo, pmx);
                //ParseMaterial(stream, ref localInfo, pmx);
                //ParseBone(stream, ref localInfo, pmx);
                //ParseMorph(stream, ref localInfo, pmx);
                //ParseDisplayFrame(stream, ref localInfo, pmx);
                //ParseRigidBody(stream, ref localInfo, pmx);
                //ParseJoint(stream, ref localInfo, pmx);
                //ParseSoftBody(stream, ref localInfo, pmx);
                return pmx;
            }
            catch(Exception ex) {
                pmx.Dispose();
                throw ex;
            }
            finally {
                StreamHelper.ReleaseBuffer();
            }
        }

        private static void ParseHeader(Stream stream, out ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            stream.NextBytes(4, out var magicWord);
            PMXValidator.ValidateMagicWord(magicWord);

            var version = (int)(stream.NextSingle() * 10);
            PMXValidator.ValidateVersion(version);

            stream.NextBytes(stream.NextByte(), out var info);
            PMXValidator.ValidateHeaderInfo(info);
            localInfo = new ParserLocalInfo((PMXVersion)version, info);
            pmx->Version = localInfo.Version;
        }

        private static void ParseModelInfo(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            stream.NextRawString(stream.NextInt32(), localInfo.Encoding, &pmx->Name);
            stream.NextRawString(stream.NextInt32(), localInfo.Encoding, &pmx->NameEnglish);
            stream.NextRawString(stream.NextInt32(), localInfo.Encoding, &pmx->Comment);
            stream.NextRawString(stream.NextInt32(), localInfo.Encoding, &pmx->CommentEnglish);
        }

        private struct ParserLocalInfo
        {
            public PMXVersion Version { get; private set; }
            public StringEncoding Encoding { get; private set; }
            public int AdditionalUVCount { get; private set; }
            public byte VertexIndexSize { get; private set; }
            public byte TextureIndexSize { get; private set; }
            public byte MaterialIndexSize { get; private set; }
            public byte BoneIndexSize { get; private set; }
            public byte MorphIndexSize { get; private set; }
            public byte RigidBodyIndexSize { get; private set; }

            public ParserLocalInfo(PMXVersion version, ReadOnlySpan<byte> info)
            {
                Version = version;
                Encoding = (StringEncoding)info[0];
                AdditionalUVCount = info[1];
                VertexIndexSize = info[2];
                TextureIndexSize = info[3];
                MaterialIndexSize = info[4];
                BoneIndexSize = info[5];
                MorphIndexSize = info[6];
                RigidBodyIndexSize = info[7];
            }
        }
    }
}
