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
                ParseModelInfo(stream, localInfo, pmx.Entity);
                ParseVertex(stream, localInfo, pmx.Entity);
                ParseSurface(stream, localInfo, pmx.Entity);
                ParseTexture(stream, localInfo, pmx.Entity);
                ParseMaterial(stream, localInfo, pmx.Entity);
                ParseBone(stream, localInfo, pmx.Entity);
                ParseMorph(stream, localInfo, pmx.Entity);
                ParseDisplayFrame(stream, localInfo, pmx.Entity);
                ParseRigidBody(stream, localInfo, pmx.Entity);
                ParseJoint(stream, localInfo, pmx.Entity);
                ParseSoftBody(stream, localInfo, pmx.Entity);
                return pmx;
            }
            catch(Exception ex) {
                pmx.Dispose();
                throw new Exception("File parsing failed.", ex);
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
            pmx->Name =           stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
            pmx->NameEnglish =    stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
            pmx->Comment =        stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
            pmx->CommentEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
        }

        private static void ParseVertex(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var vertexCount = stream.NextInt32();
            pmx->VertexList = new RawArray<Vertex>(vertexCount);
            for(int i = 0; i < vertexCount; i++) {
                ref var vertex = ref pmx->VertexList[i];
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

        private static void ParseSurface(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var count = stream.NextInt32();
            if(count % 3 != 0) { throw new FormatException(); }
            var surfaceArray = new RawArray<Surface>(count / 3);
            pmx->SurfaceList = surfaceArray;
            for(int i = 0; i < surfaceArray.Length; i++) {
                surfaceArray[i].V1 = stream.NextDataOfSize(localInfo.VertexIndexSize);
                surfaceArray[i].V2 = stream.NextDataOfSize(localInfo.VertexIndexSize);
                surfaceArray[i].V3 = stream.NextDataOfSize(localInfo.VertexIndexSize);
            }
        }

        private static void ParseTexture(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var textureCount = stream.NextInt32();
            pmx->TextureList = new DisposableRawArray<RawString>(textureCount);
            for(int i = 0; i < textureCount; i++) {
                pmx-> TextureList[i] = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
            }
        }

        private static void ParseMaterial(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var materialCount = stream.NextInt32();
            pmx->MaterialList = new DisposableRawArray<Material>(materialCount);
            for(int i = 0; i < materialCount; i++) {
                ref var material = ref pmx->MaterialList[i];
                material.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                material.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
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

                material.Memo = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                material.VertexCount = stream.NextInt32();   // always can be devided by three
            }
        }

        private static void ParseBone(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var boneCount = stream.NextInt32();
            pmx->BoneList = new DisposableRawArray<Bone>(boneCount);
            for(int i = 0; i < boneCount; i++) {
                ref var bone = ref pmx->BoneList[i];
                bone.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                bone.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
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
                    var ikLinkCount = stream.NextInt32();
                    bone.IKLinks = new RawArray<IKLink>(ikLinkCount);
                    for(int j = 0; j < bone.IKLinks.Length; j++) {
                        bone.IKLinks[j].Bone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                        bone.IKLinks[j].IsEnableAngleLimited = stream.NextByte() switch
                        {
                            0 => false,
                            1 => true,
                            _ => throw new FormatException("Invalid value"),
                        };
                        if(bone.IKLinks[j].IsEnableAngleLimited) {
                            bone.IKLinks[j].MinLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            bone.IKLinks[j].MaxLimit = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                    }
                }
            }
        }

        private static void ParseMorph(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var morphCount = stream.NextInt32();
            pmx->MorphList = new DisposableRawArray<Morph>(morphCount);
            for(int i = 0; i < morphCount; i++) {
                ref var morph = ref pmx->MorphList[i];
                morph.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                morph.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                morph.MorphTarget = (MorphTarget)stream.NextByte();
                morph.MorphType = (MorphType)stream.NextByte();
                var elementCount = stream.NextInt32();
                switch(morph.MorphType) {
                    case MorphType.Group: {
                        morph.GroupMorphElements = new RawArray<GroupMorphElement>(elementCount);
                        for(int j = 0; j < morph.GroupMorphElements.Length; j++) {
                            ref var element = ref morph.GroupMorphElements[j];
                            element.TargetMorph = stream.NextDataOfSize(localInfo.MorphIndexSize);
                            element.MorphRatio = stream.NextSingle();
                        }
                        break;
                    }
                    case MorphType.Vertex: {
                        morph.VertexMorphElements = new RawArray<VertexMorphElement>(elementCount);
                        for(int j = 0; j < morph.VertexMorphElements.Length; j++) {
                            ref var element = ref morph.VertexMorphElements[j];
                            element.TargetVertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            element.PosOffset = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Bone: {
                        morph.BoneMorphElements = new RawArray<BoneMorphElement>(elementCount);
                        for(int j = 0; j < morph.BoneMorphElements.Length; j++) {
                            ref var element = ref morph.BoneMorphElements[j];
                            element.TargetBone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                            element.Translate = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.Quaternion = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.UV:
                    case MorphType.AdditionalUV1:
                    case MorphType.AdditionalUV2:
                    case MorphType.AdditionalUV3:
                    case MorphType.AdditionalUV4: {
                        morph.UVMorphElements = new RawArray<UVMorphElement>(elementCount);
                        for(int j = 0; j < morph.UVMorphElements.Length; j++) {
                            ref var element = ref morph.UVMorphElements[j];
                            element.TargetVertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                            element.UVOffset = new Vector4(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Material: {
                        morph.MaterialMorphElements = new RawArray<MaterialMorphElement>(elementCount);
                        for(int j = 0; j < morph.MaterialMorphElements.Length; j++) {
                            ref var element = ref morph.MaterialMorphElements[j];
                            element.Material = stream.NextDataOfSize(localInfo.MaterialIndexSize);
                            element.CalcMode = (MaterialMorphCalcMode)stream.NextByte();
                            element.Diffuse = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.Specular = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.Shininess = stream.NextSingle();
                            element.Ambient = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.EdgeColor = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.EdgeSize = stream.NextSingle();
                            element.TextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.SphereTextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.ToonTextureCoef = new Color(stream.NextSingle(), stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    case MorphType.Flip: {
                        if(localInfo.Version < PMXVersion.V21) { throw new FormatException(); }
                        morph.FlipMorphElements = new RawArray<FlipMorphElement>(elementCount);
                        for(int j = 0; j < morph.FlipMorphElements.Length; j++) {
                            ref var element = ref morph.FlipMorphElements[j];
                            element.TargetMorph = stream.NextDataOfSize(localInfo.MorphIndexSize);
                            element.MorphRatio = stream.NextSingle();
                        }
                        break;
                    }
                    case MorphType.Impulse: {
                        if(localInfo.Version < PMXVersion.V21) { throw new FormatException(); }
                        morph.ImpulseMorphElements = new RawArray<ImpulseMorphElement>(elementCount);
                        for(int j = 0; j < morph.ImpulseMorphElements.Length; j++) {
                            ref var element = ref morph.ImpulseMorphElements[j];
                            element.TargetRigidBody = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                            element.IsLocal = stream.NextByte() != 0;
                            element.Velocity = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                            element.RotationTorque = new Vector3(stream.NextSingle(), stream.NextSingle(), stream.NextSingle());
                        }
                        break;
                    }
                    default:
                        throw new FormatException($"Invalid morph type : {morph.MorphType}");
                }
            }
        }

        private static void ParseDisplayFrame(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var displayFrameCount = stream.NextInt32();
            pmx->DisplayFrameList = new DisposableRawArray<DisplayFrame>(displayFrameCount);
            for(int i = 0; i < displayFrameCount; i++) {
                ref var displayFrame = ref pmx->DisplayFrameList[i];
                displayFrame.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                displayFrame.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                displayFrame.Type = (DisplayFrameType)stream.NextByte();
                var elementCount = stream.NextInt32();
                displayFrame.Elements = new RawArray<DisplayFrameElement>(elementCount);
                for(int j = 0; j < displayFrame.Elements.Length; j++) {
                    ref var element = ref displayFrame.Elements[j];
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

        private static void ParseRigidBody(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var rigidBodyCount = stream.NextInt32();
            pmx->RigidBodyList = new DisposableRawArray<RigidBody>(rigidBodyCount);
            for(int i = 0; i < pmx->RigidBodyList.Length; i++) {
                ref var rigidBody = ref pmx->RigidBodyList[i];
                rigidBody.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                rigidBody.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                rigidBody.Bone = stream.NextDataOfSize(localInfo.BoneIndexSize);
                rigidBody.HasBone = (rigidBody.Bone >= 0);
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

        private static void ParseJoint(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            var jointCount = stream.NextInt32();
            pmx->JointList = new DisposableRawArray<Joint>(jointCount);
            for(int i = 0; i < pmx->JointList.Length; i++) {
                ref var joint = ref pmx->JointList[i];
                joint.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                joint.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
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

        private static void ParseSoftBody(Stream stream, in ParserLocalInfo localInfo, PMXObject_* pmx)
        {
            if(localInfo.Version < PMXVersion.V21) { return; }
            var softBodyCount = stream.NextInt32();
            pmx->SoftBodyList = new DisposableRawArray<SoftBody>(softBodyCount);
            for(int i = 0; i < pmx->SoftBodyList.Length; i++) {
                ref var softBody = ref pmx->SoftBodyList[i];
                softBody.Name = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
                softBody.NameEnglish = stream.NextRawString(stream.NextInt32(), localInfo.Encoding);
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
                    DP = stream.NextSingle(),
                    DG = stream.NextSingle(),
                    LF = stream.NextSingle(),
                    PR = stream.NextSingle(),
                    VC = stream.NextSingle(),
                    DF = stream.NextSingle(),
                    MT = stream.NextSingle(),
                    CHR = stream.NextSingle(),
                    KHR = stream.NextSingle(),
                    SHR = stream.NextSingle(),
                    AHR = stream.NextSingle(),
                };

                softBody.Cluster = new SoftBodyCluster()
                {
                    SRHR_CL = stream.NextSingle(),
                    SKHR_CL = stream.NextSingle(),
                    SSHR_CL = stream.NextSingle(),
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
                softBody.AnchorRigidBodies = new RawArray<AnchorRigidBody>(anchorRigidBodyCount);
                for(int j = 0; j < softBody.AnchorRigidBodies.Length; j++) {
                    ref var anchor = ref softBody.AnchorRigidBodies[j];
                    anchor.RigidBody = stream.NextDataOfSize(localInfo.RigidBodyIndexSize);
                    anchor.Vertex = stream.NextDataOfSize(localInfo.VertexIndexSize);
                    anchor.IsNearMode = stream.NextByte() != 0;
                }
                var pinnedVertexCount = stream.NextInt32();
                softBody.PinnedVertex = new RawArray<int>(pinnedVertexCount);
                for(int j = 0; j < softBody.PinnedVertex.Length; j++) {
                    softBody.PinnedVertex[j] = stream.NextDataOfSize(localInfo.VertexIndexSize);
                }
            }
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
