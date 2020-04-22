#nullable enable
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Buffers;

namespace MMDTools
{
    /// <summary>PMX data parser class. This class is thread-safe.</summary>
    public static class PMXParser
    {
        /// <summary>Get PMX data version from specified file. This method is thread-safe.</summary>
        /// <param name="fileName">file name of PMX data</param>
        /// <returns>PMX file version</returns>
        public static PMXVersion GetVersion(string fileName)
        {
            if(!File.Exists(fileName)) { throw new FileNotFoundException("The file is not found", fileName); }
            using(var stream = File.OpenRead(fileName)) {
                return GetVersion(stream);
            }
        }

        /// <summary>Get PMX data version from specified stream. This method is thread-safe.</summary>
        /// <param name="stream">stream of PMX data</param>
        /// <returns>PMX file version</returns>
        public static PMXVersion GetVersion(Stream stream)
        {
            try {
                stream.NextBytes(4, out var magicWord);
                PMXValidator.ValidateMagicWord(magicWord);

                var version = (PMXVersion)(int)(stream.NextSingle() * 10);
                PMXValidator.ValidateVersion(version);
                return version;
            }
            finally {
                StreamHelper.ReleaseBuffer();
            }
        }

        /// <summary>Parse PMX data from specified file. This method is thread-safe.</summary>
        /// <param name="fileName">file name of PMX data</param>
        /// <returns>PMX object</returns>
        public static PMXObject Parse(string fileName)
        {
            if(!File.Exists(fileName)) { throw new FileNotFoundException("The file is not found", fileName); }
            using(var stream = File.OpenRead(fileName)) {
                return Parse(stream);
            }
        }

        /// <summary>Parse PMX data from specified stream. This method is thread-safe.</summary>
        /// <param name="stream">stream of PMX data</param>
        /// <returns>PMX object</returns>
        public static PMXObject Parse(Stream stream)
        {
            try {
                var pmx = new PMXObject();
                ParseHeader(stream, out var localInfo, pmx);
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
            finally {
                StreamHelper.ReleaseBuffer();
            }
        }

        private static void ParseHeader(Stream stream, out ParserLocalInfo localInfo, PMXObject pmx)
        {
            stream.NextBytes(4, out var magicWord);
            PMXValidator.ValidateMagicWord(magicWord);

            var version = (PMXVersion)(int)(stream.NextSingle() * 10);
            PMXValidator.ValidateVersion(version);

            var infoLen = stream.NextByte();
            stream.NextBytes(infoLen, out var info);
            PMXValidator.ValidateHeaderInfo(info);
            localInfo = new ParserLocalInfo(version, info);
            pmx.Version = localInfo.Version;
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
            var vertexArray = new Vertex[vertexCount];
            pmx.VertexList = vertexArray;
            for(int i = 0; i < vertexCount; i++) {
                var vertex = new Vertex();
                vertexArray[i] = vertex;
                vertex.Position = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                vertex.Normal = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                vertex.UV = new Vector2(stream.NextSingle(), stream.NextSingle());
                vertex.AdditionalUVCount = localInfo.AdditionalUVCount;
                if(localInfo.AdditionalUVCount > 0) { vertex.AdditionalUV1 = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle()); }
                if(localInfo.AdditionalUVCount > 1) { vertex.AdditionalUV2 = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle()); }
                if(localInfo.AdditionalUVCount > 2) { vertex.AdditionalUV3 = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle()); }
                if(localInfo.AdditionalUVCount > 3) { vertex.AdditionalUV4 = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle()); }
                vertex.WeightTransformType = (WeightTransformType)stream.NextByte();
                switch(vertex.WeightTransformType) {
                    case WeightTransformType.BDEF1: {
                        vertex.BoneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        break;
                    }
                    case WeightTransformType.BDEF2: {
                        vertex.BoneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.Weight1 = stream.NextSingle();
                        vertex.Weight2 = 1f - vertex.Weight1;
                        break;
                    }
                    case WeightTransformType.BDEF4: {
                        vertex.BoneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex3 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex4 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.Weight1 = stream.NextSingle();
                        vertex.Weight2 = stream.NextSingle();
                        vertex.Weight3 = stream.NextSingle();
                        vertex.Weight4 = stream.NextSingle();
                        break;
                    }
                    case WeightTransformType.SDEF: {
                        vertex.BoneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.Weight1 = stream.NextSingle();
                        vertex.Weight2 = 1f - vertex.Weight1;
                        vertex.C = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        vertex.R0 = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        vertex.R1 = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        break;
                    }
                    case WeightTransformType.QDEF: {
                        if(localInfo.Version < PMXVersion.V21) { throw new FormatException(); }
                        vertex.BoneIndex1 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex2 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex3 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.BoneIndex4 = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        vertex.Weight1 = stream.NextSingle();
                        vertex.Weight2 = stream.NextSingle();
                        vertex.Weight3 = stream.NextSingle();
                        vertex.Weight4 = stream.NextSingle();
                        break;
                    }
                    default:
                        throw new FormatException("Invalid weight transform type");
                }
                vertex.EdgeRatio = stream.NextSingle();
            }
        }

        private static void ParseSurface(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var count = stream.NextInt32();
            if(count % 3 != 0) { throw new FormatException(); }
            var surfaceArray = new Surface[count / 3];
            pmx.SurfaceList = surfaceArray;
            for(int i = 0; i < surfaceArray.Length; i++) {
                surfaceArray[i].V1 = stream.NextDataOfSize(localInfo.VertexIndexSize);
                surfaceArray[i].V2 = stream.NextDataOfSize(localInfo.VertexIndexSize);
                surfaceArray[i].V3 = stream.NextDataOfSize(localInfo.VertexIndexSize);
            }
        }

        private static void ParseTexture(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var textureCount = stream.NextInt32();
            var textureArray = new string[textureCount];
            pmx.TextureList = textureArray;
            for(int i = 0; i < textureArray.Length; i++) {
                textureArray[i] = stream.NextString(stream.NextInt32(), localInfo.Encoding);
            }
        }

        private static void ParseMaterial(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var materialCount = stream.NextInt32();
            var materialArray = new Material[materialCount];
            pmx.MaterialList = materialArray;
            for(int i = 0; i < materialCount; i++) {
                var material = new Material();
                materialArray[i] = material;
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

                material.Memo = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                material.VertexCount = stream.NextInt32();   // always can be devided by three
            }
        }

        private static void ParseBone(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var boneCount = stream.NextInt32();
            var boneArray = new Bone[boneCount];
            pmx.BoneList = boneArray;
            for(int i = 0; i < boneCount; i++) {
                var bone = new Bone();
                boneArray[i] = bone;
                bone.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                bone.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                bone.Position = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                bone.ParentBone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                bone.TransformDepth = stream.NextInt32();
                bone.BoneFlag = (BoneFlag)stream.NextInt16();
                if(bone.BoneFlag.Has(BoneFlag.ConnectionDestination)) {
                    bone.ConnectedBone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                }
                else {
                    bone.PositionOffset = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                }
                if(bone.BoneFlag.Has(BoneFlag.RotationAttach) || bone.BoneFlag.Has(BoneFlag.TranslationAttach)) {
                    bone.AttatchParent = stream.NextDataOfSize(localInfo.BoneIndexSize);
                    bone.AttatchRatio = stream.NextSingle();
                }
                if(bone.BoneFlag.Has(BoneFlag.FixedAxis)) {
                    bone.AxisVec = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                }
                if(bone.BoneFlag.Has(BoneFlag.LocalAxis)) {
                    bone.XAxisVec = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                    bone.ZAxisVec = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                }
                if(bone.BoneFlag.Has(BoneFlag.ExternalParentTransform)) {
                    bone.Key = stream.NextInt32();
                }
                if(bone.BoneFlag.Has(BoneFlag.IK)) {
                    bone.IKTarget = stream.NextDataOfSize(localInfo.BoneIndexSize);
                    bone.IterCount = stream.NextInt32();
                    bone.MaxRadianPerIter = stream.NextSingle();
                    bone.IKLinkCount = stream.NextInt32();
                    var ikLinks = new IKLink[bone.IKLinkCount];
                    bone.IKLinks = ikLinks;
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
            var morphArray = new Morph[morphCount];
            pmx.MorphList = morphArray;
            for(int i = 0; i < morphCount; i++) {
                var morph = new Morph();
                morphArray[i] = morph;
                morph.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                morph.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                morph.MorphTarget = (MorphTarget)stream.NextByte();
                morph.MorphType = (MorphType)stream.NextByte();
                var elementCount = stream.NextInt32();
                switch(morph.MorphType) {
                    case MorphType.Group: {
                        var groupMorphElements = new GroupMorphElement[elementCount];
                        morph.GroupMorphElements = groupMorphElements;
                        for(int j = 0; j < groupMorphElements.Length; j++) {
                            groupMorphElements[j] = new GroupMorphElement();
                            groupMorphElements[j].TargetMorph = stream.NextDataOfSize(localInfo.MorphIndexSize);
                            groupMorphElements[j].MorphRatio = stream.NextSingle();
                        }
                        break;
                    }
                    case MorphType.Vertex: {
                        var vertexMorphElements = new VertexMorphElement[elementCount];
                        morph.VertexMorphElements = vertexMorphElements;
                        for(int j = 0; j < vertexMorphElements.Length; j++) {
                            vertexMorphElements[j] = new VertexMorphElement();
                            vertexMorphElements[j].TargetVertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            vertexMorphElements[j].PosOffset = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Bone: {
                        var boneMorphElements = new BoneMorphElement[elementCount];
                        morph.BoneMorphElements = boneMorphElements;
                        for(int j = 0; j < boneMorphElements.Length; j++) {
                            boneMorphElements[j] = new BoneMorphElement();
                            boneMorphElements[j].TargetBone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                            boneMorphElements[j].Translate = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            boneMorphElements[j].Quaternion = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.UV:
                    case MorphType.AdditionalUV1:
                    case MorphType.AdditionalUV2:
                    case MorphType.AdditionalUV3:
                    case MorphType.AdditionalUV4: {
                        var uvMorphElements = new UVMorphElement[elementCount];
                        morph.UVMorphElements = uvMorphElements;
                        for(int j = 0; j < uvMorphElements.Length; j++) {
                            uvMorphElements[j] = new UVMorphElement();
                            uvMorphElements[j].TargetVertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            uvMorphElements[j].UVOffset = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Material: {
                        var materialMorphElements = new MaterialMorphElement[elementCount];
                        morph.MaterialMorphElements = materialMorphElements;
                        for(int j = 0; j < materialMorphElements.Length; j++) {
                            materialMorphElements[j] = new MaterialMorphElement();
                            materialMorphElements[j].Material = stream.NextDataOfSize(localInfo.MaterialIndexSize);
                            materialMorphElements[j].CalcMode = (MaterialMorphCalcMode)stream.NextByte();
                            materialMorphElements[j].Diffuse = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            materialMorphElements[j].Specular = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            materialMorphElements[j].Shininess = stream.NextSingle();
                            materialMorphElements[j].Ambient = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            materialMorphElements[j].EdgeColor = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            materialMorphElements[j].EdgeSize = stream.NextSingle();
                            materialMorphElements[j].TextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            materialMorphElements[j].SphereTextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            materialMorphElements[j].ToonTextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Flip: {
                        if(localInfo.Version < PMXVersion.V21) { throw new FormatException(); }
                        var flipMorphElements = new FlipMorphElement[elementCount];
                        morph.FlipMorphElements = flipMorphElements;
                        for(int j = 0; j < flipMorphElements.Length; j++) {
                            flipMorphElements[j] = new FlipMorphElement();
                            flipMorphElements[j].TargetMorph = stream.NextDataOfSize(localInfo.MorphIndexSize);
                            flipMorphElements[j].MorphRatio = stream.NextSingle();
                        }
                        break;
                    }
                    case MorphType.Impulse: {
                        if(localInfo.Version < PMXVersion.V21) { throw new FormatException(); }
                        var impulseMorphElements = new ImpulseMorphElement[elementCount];
                        morph.ImpulseMorphElements = impulseMorphElements;
                        for(int j = 0; j < impulseMorphElements.Length; j++) {
                            impulseMorphElements[j] = new ImpulseMorphElement();
                            impulseMorphElements[j].TargetRigidBody = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                            impulseMorphElements[j].IsLocal = stream.NextByte() != 0;
                            impulseMorphElements[j].Velocity = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            impulseMorphElements[j].RotationTorque = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    default:
                        throw new FormatException($"Invalid morph type : {morph.MorphType}");
                }
            }
        }

        private static void ParseDisplayFrame(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var displayFrameCount = stream.NextInt32();
            var displayFrameArray = new DisplayFrame[displayFrameCount];
            pmx.DisplayFrameList = displayFrameArray;
            for(int i = 0; i < displayFrameCount; i++) {
                var displayFrame = new DisplayFrame();
                displayFrameArray[i] = displayFrame;
                displayFrame.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                displayFrame.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                displayFrame.Type = (DisplayFrameType)stream.NextByte();
                var elementCount = stream.NextInt32();
                var elements = new DisplayFrameElement[elementCount];
                displayFrame.Elements = elements;
                for(int j = 0; j < elements.Length; j++) {
                    elements[j] = new DisplayFrameElement();
                    elements[j].TargetType = (DisplayFrameElementTarget)stream.NextByte();
                    elements[j].TargetIndex = elements[j].TargetType switch
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
            var rigidBodyArray = new RigidBody[rigidBodyCount];
            pmx.RigidBodyList = rigidBodyArray;
            for(int i = 0; i < rigidBodyArray.Length; i++) {
                var rigidBody = new RigidBody();
                rigidBodyArray[i] = rigidBody;
                rigidBody.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                rigidBody.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                rigidBody.Bone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                rigidBody.Group = stream.NextByte();
                rigidBody.GroupTarget = stream.NextUint16();
                rigidBody.Shape = (RigidBodyShape)stream.NextByte();
                rigidBody.Size = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                rigidBody.Position = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                rigidBody.RotationRadian = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                rigidBody.Mass = stream.NextSingle();
                rigidBody.TranslationAttenuation = stream.NextSingle();
                rigidBody.RotationAttenuation = stream.NextSingle();
                rigidBody.Recoil = stream.NextSingle();
                rigidBody.Friction = stream.NextSingle();
                rigidBody.PhysicsType = (RigidBodyPhysicsType)stream.NextByte();
            }
        }

        private static void ParseJoint(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var jointCount = stream.NextInt32();
            var jointArray = new Joint[jointCount];
            pmx.JointList = jointArray;
            for(int i = 0; i < jointArray.Length; i++) {
                var joint = new Joint();
                jointArray[i] = joint;
                joint.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                joint.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                joint.Type = (JointType)stream.NextByte();

                // Joint type of PMX Ver 2.0 is always Spring6DOF.
                if(localInfo.Version < PMXVersion.V21 && joint.Type != JointType.Spring6DOF) { throw new FormatException(); }

                joint.RigidBody1 = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                joint.RigidBody2 = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                joint.Position = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.RotationRadian = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.TranslationMinLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.TranslationMaxLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.RotationRadianMinLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.RotationRadianMaxLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.TranslationSpring = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                joint.RotationSpring = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
            }
        }

        private static void ParseSoftBody(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            if(localInfo.Version < PMXVersion.V21) {
                pmx.SoftBodyList = ReadOnlyMemory<SoftBody>.Empty;
                return;
            }
            var softBodyCount = stream.NextInt32();
            var softBodyArray = new SoftBody[softBodyCount];
            pmx.SoftBodyList = softBodyArray;
            for(int i = 0; i < softBodyArray.Length; i++) {
                var softBody = new SoftBody();
                softBodyArray[i] = softBody;
                softBody.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                softBody.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                softBody.Shape = (SoftBodyShape)stream.NextByte();
                softBody.TargetMaterial = stream.NextDataOfSize(localInfo.MaterialIndexSize);
                softBody.Group = stream.NextByte();
                softBody.GroupTarget = stream.NextUint16();
                softBody.Mode = (SoftBodyModeFlag)stream.NextByte();
                softBody.BLinkDistance = stream.NextInt32();
                softBody.ClusterCount = stream.NextInt32();
                softBody.TotalMass = stream.NextSingle();
                softBody.CollisionMargin = stream.NextSingle();
                softBody.AeroModel = (SoftBodyAeroModel)stream.NextInt32();

                softBody.Config = new SoftBodyConfig()
                {
                    VCF = stream.NextSingle(),
                    DP  = stream.NextSingle(),
                    DG  = stream.NextSingle(),
                    LF  = stream.NextSingle(),
                    PR  = stream.NextSingle(),
                    VC  = stream.NextSingle(),
                    DF  = stream.NextSingle(),
                    MT  = stream.NextSingle(),
                    CHR = stream.NextSingle(),
                    KHR = stream.NextSingle(),
                    SHR = stream.NextSingle(),
                    AHR = stream.NextSingle(),
                };

                softBody.Cluster = new SoftBodyCluster()
                {
                    SRHR_CL    = stream.NextSingle(),
                    SKHR_CL    = stream.NextSingle(),
                    SSHR_CL    = stream.NextSingle(),
                    SR_SPLT_CL = stream.NextSingle(),
                    SK_SPLT_CL = stream.NextSingle(),
                    SS_SPLT_CL = stream.NextSingle(),
                };
                softBody.Iteration = new SoftBodyIteration()
                {
                    V_IT = stream.NextInt32(),
                    P_IT = stream.NextInt32(),
                    D_IT = stream.NextInt32(),
                    C_IT = stream.NextInt32(),
                };
                softBody.Material = new SoftBodyMaterial()
                {
                    LST = stream.NextSingle(),
                    AST = stream.NextSingle(),
                    VST = stream.NextSingle(),
                };

                var anchorRigidBodyCount = stream.NextInt32();
                var anchors = new AnchorRigidBody[anchorRigidBodyCount];
                softBody.AnchorRigidBodies = anchors;
                for(int j = 0; j < anchors.Length; j++) {
                    anchors[j].RigidBody = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                    anchors[j].Vertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                    anchors[j].IsNearMode = stream.NextByte() != 0;
                }

                var pinnedVertexCount = stream.NextInt32();
                var pinnedVertex = new int[pinnedVertexCount];
                softBody.PinnedVertex = pinnedVertex;
                for(int j = 0; j < pinnedVertex.Length; j++) {
                    pinnedVertex[j] = stream.NextDataOfSize(localInfo.VertexIndexSize);
                }
            }
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

            public ParserLocalInfo(PMXVersion version, ReadOnlySpan<byte> info)
            {
                Version = version;
                Encoding = info[0] switch
                {
                    0 => Encoding.Unicode,
                    1 => Encoding.UTF8,
                    _ => throw new Exception("Hey! The value is not validated!? How'd I get here?"),
                };
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

    internal static class PMXValidator
    {
        public static void ValidateMagicWord(ReadOnlySpan<byte> magicWord)
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

        public static void ValidateHeaderInfo(ReadOnlySpan<byte> info)
        {
            Assert(info[0] == 0 || info[0] == 1, "Invalid encode type");
            Assert(info[1] >= 0 || info[1] <= 4, "Invalid additional UV count");
            Assert(info[2] == 1 || info[2] == 2 || info[2] == 4, "Invalid vertex index size");
            Assert(info[3] == 1 || info[3] == 2 || info[3] == 4, "Invalid texture index size");
            Assert(info[4] == 1 || info[4] == 2 || info[4] == 4, "Invalid material index size");
            Assert(info[5] == 1 || info[5] == 2 || info[5] == 4, "Invalid bone index size");
            Assert(info[6] == 1 || info[6] == 2 || info[6] == 4, "Invalid morph index size");
            Assert(info[7] == 1 || info[7] == 2 || info[7] == 4, "Invalid rigid body index size");
        }

        private static void Assert(bool condition, string message)
        {
            if(!condition) { throw new FormatException(message); }
        }
    }

    internal static class StreamHelper
    {
        [ThreadStatic]
        private static byte[]? _tlsBuffer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] GetTLSBuffer(int minLength)
        {
            var tlsBuffer = _tlsBuffer;
            if(tlsBuffer != null) {
                if(tlsBuffer.Length >= minLength) {
                    return tlsBuffer;
                }
                else {
                    ArrayPool<byte>.Shared.Return(tlsBuffer);
                }
            }
            return _tlsBuffer = ArrayPool<byte>.Shared.Rent(minLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReleaseBuffer()
        {
            if(_tlsBuffer != null) {
                ArrayPool<byte>.Shared.Return(_tlsBuffer);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string NextString(this Stream source, int byteSize, Encoding encoding)
        {
            if(byteSize == 0) { return string.Empty; }
            Read(source, byteSize, out var result);
            fixed(byte* ptr = result) {
                return encoding.GetString(ptr, result.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextInt32(this Stream source)
        {
            Read(source, sizeof(int), out var result);
            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short NextInt16(this Stream source)
        {
            Read(source, sizeof(short), out var result);
            return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort NextUint16(this Stream source)
        {
            Read(source, sizeof(ushort), out var result);
            return Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextSingle(this Stream source)
        {
            Read(source, sizeof(float), out var result);
            return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(result));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte NextByte(this Stream source)
        {
            Read(source, sizeof(byte), out var result);
            return result[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NextBytes(this Stream source, int byteSize, out ReadOnlySpan<byte> bytes)
        {
            Read(source, byteSize, out var result);
            bytes = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextDataOfSize(this Stream source, byte byteSize)
        {
            // byteSize must be [1 <= byteSize <= 4]

            Read(source, byteSize, out var buf);
            return byteSize switch
            {
                1 => (int)(sbyte)buf[0],
                2 => (int)Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(buf)),
                4 => Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(buf)),
                _ => throw new InvalidOperationException("Invalid byte size. Byte size must be 1, 2, or 4."),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Read(Stream stream, int length, out Span<byte> result)
        {
            var buf = GetTLSBuffer(length);
            if(stream.Read(buf, 0, length) != length) { throw new EndOfStreamException(); }
            result = buf.AsSpan(0, length);
        }
    }
}
