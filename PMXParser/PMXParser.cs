#nullable enable
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.ObjectModel;

namespace MMDTools
{
    /// <summary>PMX data parser class</summary>
    public static class PMXParser
    {
        /// <summary>Get PMX data version from specified file</summary>
        /// <param name="fileName">file name of PMX data</param>
        /// <returns>PMX file version</returns>
        public static PMXVersion GetVersion(string fileName)
        {
            if(!File.Exists(fileName)) { throw new FileNotFoundException("The file is not found", fileName); }
            using(var stream = File.OpenRead(fileName)) {
                return GetVersion(stream);
            }
        }

        /// <summary>Get PMX data version from specified stream</summary>
        /// <param name="stream">stream of PMX data</param>
        /// <returns>PMX file version</returns>
        public static PMXVersion GetVersion(Stream stream)
        {
            Span<byte> magicWord = stackalloc byte[4];
            stream.NextBytes(ref magicWord);
            PMXValidator.ValidateMagicWord(ref magicWord);

            var version = (PMXVersion)(int)(stream.NextSingle() * 10);
            PMXValidator.ValidateVersion(version);
            return version;
        }

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

        private static void ParseHeader(Stream stream, out ParserLocalInfo localInfo, PMXObject pmx)
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
            localInfo = new ParserLocalInfo(version, ref info);
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
            pmx.VertexList = new ReadOnlyCollection<Vertex>(vertexArray);
            for(int i = 0; i < vertexCount; i++) {
                var vertex = new Vertex();
                vertexArray[i] = vertex;
                vertex.Posision = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
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
            pmx.SurfaceList = new ReadOnlyCollection<Surface>(surfaceArray);
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
            pmx.TextureList = new ReadOnlyCollection<string>(textureArray);
            for(int i = 0; i < textureArray.Length; i++) {
                textureArray[i] = stream.NextString(stream.NextInt32(), localInfo.Encoding);
            }
        }

        private static void ParseMaterial(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var materialCount = stream.NextInt32();
            var materialArray = new Material[materialCount];
            pmx.MaterialList = new ReadOnlyCollection<Material>(materialArray);
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
            var boneArray = new Bone[boneCount];
            pmx.BoneList = new ReadOnlyCollection<Bone>(boneArray);
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
                    bone.IKLinks = new ReadOnlyCollection<IKLink>(ikLinks);
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
            pmx.MorphList = new ReadOnlyCollection<Morph>(morphArray);
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
                        morph.GroupMorphElements = new ReadOnlyCollection<GroupMorphElement>(groupMorphElements);
                        for(int j = 0; j < groupMorphElements.Length; j++) {
                            groupMorphElements[j] = new GroupMorphElement();
                            groupMorphElements[j].TargetMorph = stream.NextDataOfSize(localInfo.MorphIndexSize);
                            groupMorphElements[j].MorphRatio = stream.NextSingle();
                        }
                        break;
                    }
                    case MorphType.Vertex: {
                        var vertexMorphElements = new VertexMorphElement[elementCount];
                        morph.VertexMorphElements = new ReadOnlyCollection<VertexMorphElement>(vertexMorphElements);
                        for(int j = 0; j < vertexMorphElements.Length; j++) {
                            vertexMorphElements[j] = new VertexMorphElement();
                            vertexMorphElements[j].TargetVertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            vertexMorphElements[j].PosOffset = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Bone: {
                        var boneMorphElements = new BoneMorphElement[elementCount];
                        morph.BoneMorphElements = new ReadOnlyCollection<BoneMorphElement>(boneMorphElements);
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
                        morph.UVMorphElements = new ReadOnlyCollection<UVMorphElement>(uvMorphElements);
                        for(int j = 0; j < uvMorphElements.Length; j++) {
                            uvMorphElements[j] = new UVMorphElement();
                            uvMorphElements[j].TargetVertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            uvMorphElements[j].UVOffset = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Material: {
                        var materialMorphElements = new MaterialMorphElement[elementCount];
                        morph.MaterialMorphElements = new ReadOnlyCollection<MaterialMorphElement>(materialMorphElements);
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
                        morph.FlipMorphElements = new ReadOnlyCollection<FlipMorphElement>(flipMorphElements);
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
                        morph.ImpulseMorphElements = new ReadOnlyCollection<ImpulseMorphElement>(impulseMorphElements);
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
                morph.GroupMorphElements    ??= new ReadOnlyCollection<GroupMorphElement>(Array.Empty<GroupMorphElement>());
                morph.VertexMorphElements   ??= new ReadOnlyCollection<VertexMorphElement>(Array.Empty<VertexMorphElement>());
                morph.BoneMorphElements     ??= new ReadOnlyCollection<BoneMorphElement>(Array.Empty<BoneMorphElement>());
                morph.UVMorphElements       ??= new ReadOnlyCollection<UVMorphElement>(Array.Empty<UVMorphElement>());
                morph.MaterialMorphElements ??= new ReadOnlyCollection<MaterialMorphElement>(Array.Empty<MaterialMorphElement>());
                morph.FlipMorphElements     ??= new ReadOnlyCollection<FlipMorphElement>(Array.Empty<FlipMorphElement>());
                morph.ImpulseMorphElements  ??= new ReadOnlyCollection<ImpulseMorphElement>(Array.Empty<ImpulseMorphElement>());
            }
        }

        private static void ParseDisplayFrame(Stream stream, ref ParserLocalInfo localInfo, PMXObject pmx)
        {
            var displayFrameCount = stream.NextInt32();
            var displayFrameArray = new DisplayFrame[displayFrameCount];
            pmx.DisplayFrameList = new ReadOnlyCollection<DisplayFrame>(displayFrameArray);
            for(int i = 0; i < displayFrameCount; i++) {
                var displayFrame = new DisplayFrame();
                displayFrameArray[i] = displayFrame;
                displayFrame.Name = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                displayFrame.NameEnglish = stream.NextString(stream.NextInt32(), localInfo.Encoding);
                displayFrame.Type = (DisplayFrameType)stream.NextByte();
                var elementCount = stream.NextInt32();
                var elements = new DisplayFrameElement[elementCount];
                displayFrame.Elements = new ReadOnlyCollection<DisplayFrameElement>(elements);
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
            pmx.RigidBodyList = new ReadOnlyCollection<RigidBody>(rigidBodyArray);
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
            pmx.JointList = new ReadOnlyCollection<Joint>(jointArray);
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
                pmx.SoftBodyList = new ReadOnlyCollection<SoftBody>(Array.Empty<SoftBody>());
                return;
            }
            var softBodyCount = stream.NextInt32();
            var softBodyArray = new SoftBody[softBodyCount];
            pmx.SoftBodyList = new ReadOnlyCollection<SoftBody>(softBodyArray);
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
                softBody.AnchorRigidBodies = new ReadOnlyCollection<AnchorRigidBody>(anchors);
                for(int j = 0; j < anchors.Length; j++) {
                    anchors[j].RigidBody = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                    anchors[j].Vertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                    anchors[j].IsNearMode = stream.NextByte() != 0;
                }

                var pinnedVertexCount = stream.NextInt32();
                var pinnedVertex = new int[pinnedVertexCount];
                softBody.PinnedVertex = new ReadOnlyCollection<int>(pinnedVertex);
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

            public ParserLocalInfo(PMXVersion version, ref Span<byte> info)
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

        public unsafe static string NextString(this Stream source, int byteSize, Encoding encoding)
        {
            if(byteSize <= 128) {
                Span<byte> buf = stackalloc byte[byteSize];
                Read(source, ref buf);
                return encoding.GetString(buf);
            }
            else {
                var ptr = Marshal.AllocHGlobal(byteSize);
                try {
                    var buf = new Span<byte>((byte*)ptr, byteSize);
                    Read(source, ref buf);
                    return encoding.GetString(buf);
                }
                finally {
                    Marshal.FreeHGlobal(ptr);
                }
            }
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
