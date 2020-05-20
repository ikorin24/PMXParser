#nullable enable
using Xunit;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Buffers;
using System.Linq;

namespace Test
{
    public class PMXParser_UMTest
    {
#if !NETCOREAPP3_0
        const string FILES = "../../../../Test/Files/";
#else
        const string FILES = "../../../Files/";
#endif
        const string HIDDEN = FILES + "Hidden/";

        [Theory(DisplayName = "PMX Ver 2.0")]
        [InlineData(FILES + "Alicia/Alicia_blade.pmx")]
        [InlineData(FILES + "Alicia/Alicia_solid.pmx")]
        [InlineData(FILES + "Appearance Miku/Appearance Miku.pmx")]
        [InlineData(FILES + "Appearance Miku/Appearance Miku_BDEF.pmx")]


        // Following cases of test are passed, but thier PMX does not exist in the repository because of their license.
        // (Redistribution does not allowed.)
#if false
        [InlineData(HIDDEN + "Mirai_Akari_v1.0/MiraiAkari_v1.0.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "初音ミクver.2.1/初音ミクver.2.1.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "癒月ちょこ公式mmd_ver1.0/癒月ちょこ.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "紫咲シオン公式mmd_ver1.01/紫咲シオン.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "くむ式キバナ01/くむ式キバナ01.pmx", PMXVersion.V20)]
#endif
        public unsafe void PMXParseVer20(string fileName)
        {
            using(var stream = File.OpenRead(fileName)) {
                using var pmxUm = MMDTools.Unmanaged.PMXParser.Parse(stream);
                Assert.Equal(stream.Length, stream.Position);
                stream.Position = 0;
                var pmx = MMDTools.PMXParser.Parse(stream);
                Assert.Equal(stream.Length, stream.Position);


                Assert.True(StructEqual(pmx.Version, pmxUm.Version));
                Assert.Equal(pmx.Name, pmxUm.Name.ToString());
                Assert.Equal(pmx.NameEnglish, pmxUm.NameEnglish.ToString());
                Assert.Equal(pmx.Comment, pmxUm.Comment.ToString());
                Assert.Equal(pmx.CommentEnglish, pmxUm.CommentEnglish.ToString());
                AssertEqual(pmx.VertexList, pmxUm.VertexList);
                AssertEqual(pmx.SurfaceList, pmxUm.SurfaceList);
                AssertEqual(pmx.TextureList, pmxUm.TextureList);
                AssertEqual(pmx.MaterialList, pmxUm.MaterialList);
                AssertEqual(pmx.BoneList, pmxUm.BoneList);
                AssertEqual(pmx.MorphList, pmxUm.MorphList);
                AssertEqual(pmx.DisplayFrameList, pmxUm.DisplayFrameList);
                AssertEqual(pmx.RigidBodyList, pmxUm.RigidBodyList);
                AssertEqual(pmx.JointList, pmxUm.JointList);
                AssertEqual(pmx.SoftBodyList, pmxUm.SoftBodyList);
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.SoftBody> softBodyList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.SoftBody> softbodyList2)
        {
            var sbl1 = softBodyList1.Span;
            var sbl2 = softbodyList2.AsSpan();
            Assert.Equal(sbl1.Length, sbl2.Length);
            for(int i = 0; i < sbl1.Length; i++) {
                Assert.True(StructEqual(sbl1[i].AeroModel, sbl2[i].AeroModel));

                var arb1 = sbl1[i].AnchorRigidBodies.Span;
                var arb2 = sbl2[i].AnchorRigidBodies.AsSpan();
                Assert.Equal(arb1.Length, arb2.Length);
                for(int j = 0; j < arb1.Length; j++) {
                    Assert.True(StructEqual(arb1[j], arb2[j]));
                }
                Assert.True(StructEqual(sbl1[i].BLinkDistance, sbl2[i].BLinkDistance));
                Assert.True(StructEqual(sbl1[i].Cluster.SKHR_CL, sbl2[i].Cluster.SKHR_CL));
                Assert.True(StructEqual(sbl1[i].Cluster.SK_SPLT_CL, sbl2[i].Cluster.SK_SPLT_CL));
                Assert.True(StructEqual(sbl1[i].Cluster.SRHR_CL, sbl2[i].Cluster.SRHR_CL));
                Assert.True(StructEqual(sbl1[i].Cluster.SR_SPLT_CL, sbl2[i].Cluster.SR_SPLT_CL));
                Assert.True(StructEqual(sbl1[i].Cluster.SSHR_CL, sbl2[i].Cluster.SSHR_CL));
                Assert.True(StructEqual(sbl1[i].Cluster.SS_SPLT_CL, sbl2[i].Cluster.SS_SPLT_CL));
                Assert.True(StructEqual(sbl1[i].ClusterCount, sbl2[i].ClusterCount));
                Assert.True(StructEqual(sbl1[i].CollisionMargin, sbl2[i].CollisionMargin));
                Assert.True(StructEqual(sbl1[i].Config.AHR, sbl2[i].Config.AHR));
                Assert.True(StructEqual(sbl1[i].Config.CHR, sbl2[i].Config.CHR));
                Assert.True(StructEqual(sbl1[i].Config.DF, sbl2[i].Config.DF));
                Assert.True(StructEqual(sbl1[i].Config.DG, sbl2[i].Config.DG));
                Assert.True(StructEqual(sbl1[i].Config.DP, sbl2[i].Config.DP));
                Assert.True(StructEqual(sbl1[i].Config.KHR, sbl2[i].Config.KHR));
                Assert.True(StructEqual(sbl1[i].Config.LF, sbl2[i].Config.LF));
                Assert.True(StructEqual(sbl1[i].Config.MT, sbl2[i].Config.MT));
                Assert.True(StructEqual(sbl1[i].Config.PR, sbl2[i].Config.PR));
                Assert.True(StructEqual(sbl1[i].Config.SHR, sbl2[i].Config.SHR));
                Assert.True(StructEqual(sbl1[i].Config.VC, sbl2[i].Config.VC));
                Assert.True(StructEqual(sbl1[i].Config.VCF, sbl2[i].Config.VCF));
                Assert.True(StructEqual(sbl1[i].Group, sbl2[i].Group));
                Assert.True(StructEqual(sbl1[i].GroupTarget, sbl2[i].GroupTarget));
                Assert.True(StructEqual(sbl1[i].Iteration.C_IT, sbl2[i].Iteration.C_IT));
                Assert.True(StructEqual(sbl1[i].Iteration.D_IT, sbl2[i].Iteration.D_IT));
                Assert.True(StructEqual(sbl1[i].Iteration.P_IT, sbl2[i].Iteration.P_IT));
                Assert.True(StructEqual(sbl1[i].Iteration.V_IT, sbl2[i].Iteration.V_IT));
                Assert.True(StructEqual(sbl1[i].Material.AST, sbl2[i].Material.AST));
                Assert.True(StructEqual(sbl1[i].Material.LST, sbl2[i].Material.LST));
                Assert.True(StructEqual(sbl1[i].Material.VST, sbl2[i].Material.VST));
                Assert.True(StructEqual(sbl1[i].Mode, sbl2[i].Mode));
                Assert.Equal(sbl1[i].Name, sbl2[i].Name.ToString());
                Assert.Equal(sbl1[i].NameEnglish, sbl2[i].NameEnglish.ToString());
                Assert.True(sbl1[i].PinnedVertex.Span.SequenceEqual(sbl2[i].PinnedVertex.AsSpan()));
                Assert.True(StructEqual(sbl1[i].Shape, sbl2[i].Shape));
                Assert.True(StructEqual(sbl1[i].TargetMaterial, sbl2[i].TargetMaterial));
                Assert.True(StructEqual(sbl1[i].TotalMass, sbl2[i].TotalMass));
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.Joint> jointList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.Joint> jointList2)
        {
            var jl1 = jointList1.Span;
            var jl2 = jointList2.AsSpan();
            Assert.Equal(jl1.Length, jl2.Length);
            for(int i = 0; i < jl1.Length; i++) {
                Assert.Equal(jl1[i].Name, jl2[i].Name.ToString());
                Assert.Equal(jl1[i].NameEnglish, jl2[i].NameEnglish.ToString());
                Assert.True(StructEqual(jl1[i].Position, jl2[i].Position));
                Assert.True(StructEqual(jl1[i].RigidBody1, jl2[i].RigidBody1));
                Assert.True(StructEqual(jl1[i].RigidBody2, jl2[i].RigidBody2));
                Assert.True(StructEqual(jl1[i].RotationRadian, jl2[i].RotationRadian));
                Assert.True(StructEqual(jl1[i].RotationRadianMaxLimit, jl2[i].RotationRadianMaxLimit));
                Assert.True(StructEqual(jl1[i].RotationRadianMinLimit, jl2[i].RotationRadianMinLimit));
                Assert.True(StructEqual(jl1[i].RotationSpring, jl2[i].RotationSpring));
                Assert.True(StructEqual(jl1[i].TranslationMaxLimit, jl2[i].TranslationMaxLimit));
                Assert.True(StructEqual(jl1[i].TranslationMinLimit, jl2[i].TranslationMinLimit));
                Assert.True(StructEqual(jl1[i].TranslationSpring, jl2[i].TranslationSpring));
                Assert.True(StructEqual(jl1[i].Type, jl2[i].Type));
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.RigidBody> rigidBodyList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.RigidBody> rigidBodyList2)
        {
            var rbl1 = rigidBodyList1.Span;
            var rbl2 = rigidBodyList2.AsSpan();
            Assert.Equal(rbl1.Length, rbl2.Length);
            for(int i = 0; i < rbl1.Length; i++) {
                Assert.True(StructEqual(rbl1[i].Bone, rbl2[i].Bone));
                Assert.True(StructEqual(rbl1[i].Friction, rbl2[i].Friction));
                Assert.True(StructEqual(rbl1[i].Group, rbl2[i].Group));
                Assert.True(StructEqual(rbl1[i].GroupTarget, rbl2[i].GroupTarget));
                Assert.True(StructEqual(rbl1[i].HasBone, rbl2[i].HasBone));
                Assert.True(StructEqual(rbl1[i].Mass, rbl2[i].Mass));
                Assert.Equal(rbl1[i].Name, rbl2[i].Name.ToString());
                Assert.Equal(rbl1[i].NameEnglish, rbl2[i].NameEnglish.ToString());
                Assert.True(StructEqual(rbl1[i].PhysicsType, rbl2[i].PhysicsType));
                Assert.True(StructEqual(rbl1[i].Position, rbl2[i].Position));
                Assert.True(StructEqual(rbl1[i].Recoil, rbl2[i].Recoil));
                Assert.True(StructEqual(rbl1[i].RotationAttenuation, rbl2[i].RotationAttenuation));
                Assert.True(StructEqual(rbl1[i].RotationRadian, rbl2[i].RotationRadian));
                Assert.True(StructEqual(rbl1[i].Shape, rbl2[i].Shape));
                Assert.True(StructEqual(rbl1[i].Size, rbl2[i].Size));
                Assert.True(StructEqual(rbl1[i].TranslationAttenuation, rbl2[i].TranslationAttenuation));
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.DisplayFrame> displayFrameList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.DisplayFrame> displayFrameList2)
        {
            var dfl1 = displayFrameList1.Span;
            var dfl2 = displayFrameList2.AsSpan();
            Assert.Equal(dfl1.Length, dfl2.Length);
            for(int i = 0; i < dfl1.Length; i++) {
                var e1 = dfl1[i].Elements.Span;
                var e2 = dfl2[i].Elements.AsSpan();
                Assert.Equal(e1.Length, e2.Length);
                for(int j = 0; j < e1.Length; j++) {
                    Assert.True(StructEqual(e1[j].TargetIndex, e2[j].TargetIndex));
                    Assert.True(StructEqual(e1[j].TargetType, e2[j].TargetType));
                }
                Assert.Equal(dfl1[i].Name, dfl2[i].Name.ToString());
                Assert.Equal(dfl1[i].NameEnglish, dfl2[i].NameEnglish.ToString());
                Assert.True(StructEqual(dfl1[i].Type, dfl2[i].Type));
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.Morph> morphList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.Morph> morphList2)
        {
            var ml1 = morphList1.Span;
            var ml2 = morphList2.AsSpan();
            Assert.Equal(ml1.Length, ml2.Length);
            for(int i = 0; i < ml1.Length; i++) {

                var bme1 = ml1[i].BoneMorphElements.Span;
                var bme2 = ml2[i].BoneMorphElements.AsSpan();
                Assert.Equal(bme1.Length, bme2.Length);
                for(int j = 0; j < bme1.Length; j++) {
                    Assert.True(StructEqual(bme1[j].Quaternion, bme2[j].Quaternion));
                    Assert.True(StructEqual(bme1[j].TargetBone, bme2[j].TargetBone));
                    Assert.True(StructEqual(bme1[j].Translate, bme2[j].Translate));
                }

                var fme1 = ml1[i].FlipMorphElements.Span;
                var fme2 = ml2[i].FlipMorphElements.AsSpan();
                Assert.Equal(fme1.Length, fme2.Length);
                for(int j = 0; j < fme1.Length; j++) {
                    Assert.True(StructEqual(fme1[j].MorphRatio, fme2[j].MorphRatio));
                    Assert.True(StructEqual(fme1[j].TargetMorph, fme2[j].TargetMorph));
                }

                var gme1 = ml1[i].GroupMorphElements.Span;
                var gme2 = ml2[i].GroupMorphElements.AsSpan();
                Assert.Equal(gme1.Length, gme2.Length);
                for(int j = 0; j < gme1.Length; j++) {
                    Assert.True(StructEqual(gme1[j].MorphRatio, gme2[j].MorphRatio));
                    Assert.True(StructEqual(gme1[j].TargetMorph, gme2[j].TargetMorph));
                }

                var ime1 = ml1[i].ImpulseMorphElements.Span;
                var ime2 = ml2[i].ImpulseMorphElements.AsSpan();
                Assert.Equal(ime1.Length, ime2.Length);
                for(int j = 0; j < ime1.Length; j++) {
                    Assert.True(StructEqual(ime1[j].IsLocal, ime2[j].IsLocal));
                    Assert.True(StructEqual(ime1[j].RotationTorque, ime2[j].RotationTorque));
                    Assert.True(StructEqual(ime1[j].TargetRigidBody, ime2[j].TargetRigidBody));
                    Assert.True(StructEqual(ime1[j].Velocity, ime2[j].Velocity));
                }

                var mme1 = ml1[i].MaterialMorphElements.Span;
                var mme2 = ml2[i].MaterialMorphElements.AsSpan();
                Assert.Equal(mme1.Length, mme2.Length);
                for(int j = 0; j < mme1.Length; j++) {
                    Assert.True(StructEqual(mme1[j].Ambient, mme1[j].Ambient));
                    Assert.True(StructEqual(mme1[j].CalcMode, mme1[j].CalcMode));
                    Assert.True(StructEqual(mme1[j].Diffuse, mme1[j].Diffuse));
                    Assert.True(StructEqual(mme1[j].EdgeColor, mme1[j].EdgeColor));
                    Assert.True(StructEqual(mme1[j].EdgeSize, mme1[j].EdgeSize));
                    Assert.True(StructEqual(mme1[j].IsAllMaterialTarget, mme1[j].IsAllMaterialTarget));
                    Assert.True(StructEqual(mme1[j].Material, mme1[j].Material));
                    Assert.True(StructEqual(mme1[j].Shininess, mme1[j].Shininess));
                    Assert.True(StructEqual(mme1[j].Specular, mme1[j].Specular));
                    Assert.True(StructEqual(mme1[j].SphereTextureCoef, mme1[j].SphereTextureCoef));
                    Assert.True(StructEqual(mme1[j].TextureCoef, mme1[j].TextureCoef));
                    Assert.True(StructEqual(mme1[j].ToonTextureCoef, mme1[j].ToonTextureCoef));
                }

                Assert.True(StructEqual(ml1[i].MorphTarget, ml2[i].MorphTarget));
                Assert.True(StructEqual(ml1[i].MorphType, ml2[i].MorphType));
                Assert.Equal(ml1[i].Name, ml2[i].Name.ToString());
                Assert.Equal(ml1[i].NameEnglish, ml2[i].NameEnglish.ToString());

                var uvme1 = ml1[i].UVMorphElements.Span;
                var uvme2 = ml2[i].UVMorphElements.AsSpan();
                Assert.Equal(uvme1.Length, uvme2.Length);
                for(int j = 0; j < uvme1.Length; j++) {
                    Assert.True(StructEqual(uvme1[j].TargetVertex, uvme2[j].TargetVertex));
                    Assert.True(StructEqual(uvme1[j].UVOffset, uvme2[j].UVOffset));
                }

                var vme1 = ml1[i].VertexMorphElements.Span;
                var vme2 = ml2[i].VertexMorphElements.AsSpan();
                Assert.Equal(vme1.Length, vme2.Length);
                for(int j = 0; j < vme1.Length; j++) {
                    Assert.True(StructEqual(vme1[j].PosOffset, vme2[j].PosOffset));
                    Assert.True(StructEqual(vme1[j].TargetVertex, vme2[j].TargetVertex));
                }
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.Bone> boneList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.Bone> boneList2)
        {
            var bl1 = boneList1.Span;
            var bl2 = boneList2.AsSpan();
            Assert.Equal(bl1.Length, bl2.Length);
            for(int i = 0; i < bl1.Length; i++) {
                Assert.True(StructEqual(bl1[i].AttatchParent, bl2[i].AttatchParent));
                Assert.True(StructEqual(bl1[i].AttatchRatio, bl2[i].AttatchRatio));
                Assert.True(StructEqual(bl1[i].AxisVec, bl2[i].AxisVec));
                Assert.True(StructEqual(bl1[i].BoneFlag, bl2[i].BoneFlag));
                Assert.True(StructEqual(bl1[i].ConnectedBone, bl2[i].ConnectedBone));
                Assert.True(StructEqual(bl1[i].IKLinkCount, bl2[i].IKLinks.Length));
                Assert.True(StructEqual(bl1[i].IKLinks.Length, bl2[i].IKLinks.Length));
                Assert.True(bl1[i].IKLinks.ToArray().Zip(bl2[i].IKLinks.AsSpan().ToArray(), (a, b) => StructEqual(a, b)).All(a => a));
                Assert.True(StructEqual(bl1[i].IKTarget, bl2[i].IKTarget));
                Assert.True(StructEqual(bl1[i].IterCount, bl2[i].IterCount));
                Assert.True(StructEqual(bl1[i].Key, bl2[i].Key));
                Assert.True(StructEqual(bl1[i].MaxRadianPerIter, bl2[i].MaxRadianPerIter));
                Assert.Equal(bl1[i].Name, bl2[i].Name.ToString());
                Assert.Equal(bl1[i].NameEnglish, bl2[i].NameEnglish.ToString());
                Assert.True(StructEqual(bl1[i].ParentBone, bl2[i].ParentBone));
                Assert.True(StructEqual(bl1[i].Position, bl2[i].Position));
                Assert.True(StructEqual(bl1[i].PositionOffset, bl2[i].PositionOffset));
                Assert.True(StructEqual(bl1[i].TransformDepth, bl2[i].TransformDepth));
                Assert.True(StructEqual(bl1[i].XAxisVec, bl2[i].XAxisVec));
                Assert.True(StructEqual(bl1[i].ZAxisVec, bl2[i].ZAxisVec));
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.Material> materialList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.Material> materialList2)
        {
            var ml1 = materialList1.Span;
            var ml2 = materialList2.AsSpan();
            Assert.Equal(ml1.Length, ml2.Length);
            for(int i = 0; i < ml1.Length; i++) {
                Assert.True(StructEqual(ml1[i].Ambient, ml2[i].Ambient));
                Assert.True(StructEqual(ml1[i].Diffuse, ml2[i].Diffuse));
                Assert.True(StructEqual(ml1[i].DrawFlag, ml2[i].DrawFlag));
                Assert.True(StructEqual(ml1[i].EdgeColor, ml2[i].EdgeColor));
                Assert.True(StructEqual(ml1[i].EdgeSize, ml2[i].EdgeSize));
                Assert.Equal(ml1[i].Memo, ml2[i].Memo.ToString());
                Assert.Equal(ml1[i].Name, ml1[i].Name.ToString());
                Assert.Equal(ml1[i].NameEnglish, ml1[i].NameEnglish.ToString());
                Assert.True(StructEqual(ml1[i].SharedToonMode, ml1[i].SharedToonMode));
                Assert.True(StructEqual(ml1[i].Shininess, ml1[i].Shininess));
                Assert.True(StructEqual(ml1[i].Specular, ml1[i].Specular));
                Assert.True(StructEqual(ml1[i].SphereTextre, ml1[i].SphereTextre));
                Assert.True(StructEqual(ml1[i].SphereTextureMode, ml1[i].SphereTextureMode));
                Assert.True(StructEqual(ml1[i].Texture, ml1[i].Texture));
                Assert.True(StructEqual(ml1[i].ToonTexture, ml1[i].ToonTexture));
                Assert.True(StructEqual(ml1[i].VertexCount, ml1[i].VertexCount));
            }
        }


        private void AssertEqual(ReadOnlyMemory<string> textureList1, MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.ReadOnlyRawString> textureList2)
        {
            var span1 = textureList1.Span;
            var span2 = textureList2.AsSpan();
            Assert.Equal(span1.Length, span2.Length);
            for(int i = 0; i < span1.Length; i++) {
                Assert.Equal(span1[i].Length, span2[i].GetCharCount());
                char[] buffer = null!;
                try {
                    buffer = ArrayPool<char>.Shared.Rent(span1[i].Length);
                    var str1 = span1[i].AsSpan();
                    var str2 = span2[i].ToString(buffer.AsSpan());                  // ReadOnlySpan<char> への変換のテスト
                    Assert.True(str1.Equals(str1, StringComparison.Ordinal));
                }
                finally {
                    if(buffer != null) {
                        ArrayPool<char>.Shared.Return(buffer);
                    }
                }
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.Vertex> vertexList1,
                                 MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.Vertex> vertexList2)
        {
            Assert.Equal(vertexList1.Length, vertexList2.Length);
            var span1 = vertexList1.Span;
            var span2 = vertexList2.AsSpan();

            Assert.Equal(span1.Length, span2.Length);
            for(int i = 0; i < span1.Length; i++) {
                var v1 = span1[i];
                var v2 = span2[i];
                Assert.True(StructEqual(v1.AdditionalUV1, v2.AdditionalUV1));
                Assert.True(StructEqual(v1.AdditionalUV2, v2.AdditionalUV2));
                Assert.True(StructEqual(v1.AdditionalUV3, v2.AdditionalUV3));
                Assert.True(StructEqual(v1.AdditionalUV4, v2.AdditionalUV4));
                Assert.True(StructEqual(v1.AdditionalUVCount, v2.AdditionalUVCount));
                Assert.True(StructEqual(v1.BoneIndex1, v2.BoneIndex1));
                Assert.True(StructEqual(v1.BoneIndex2, v2.BoneIndex2));
                Assert.True(StructEqual(v1.BoneIndex3, v2.BoneIndex3));
                Assert.True(StructEqual(v1.BoneIndex4, v2.BoneIndex4));
                Assert.True(StructEqual(v1.C, v2.C));
                Assert.True(StructEqual(v1.EdgeRatio, v2.EdgeRatio));
                Assert.True(StructEqual(v1.Normal, v2.Normal));
                Assert.True(StructEqual(v1.Position, v2.Position));
                Assert.True(StructEqual(v1.R0, v2.R0));
                Assert.True(StructEqual(v1.R1, v2.R1));
                Assert.True(StructEqual(v1.UV, v2.UV));
                Assert.True(StructEqual(v1.Weight1, v2.Weight1));
                Assert.True(StructEqual(v1.Weight2, v2.Weight2));
                Assert.True(StructEqual(v1.Weight3, v2.Weight3));
                Assert.True(StructEqual(v1.Weight4, v2.Weight4));
                Assert.True(StructEqual(v1.WeightTransformType, v2.WeightTransformType));
            }
        }

        private void AssertEqual(ReadOnlyMemory<MMDTools.Surface> surfaceList1,
                                 MMDTools.Unmanaged.ReadOnlyRawArray<MMDTools.Unmanaged.Surface> surfaceList2)
        {
            var s1 = surfaceList1.Span;
            for(int i = 0; i < s1.Length; i++) {
                Assert.True(StructEqual(s1[i], surfaceList2[i]));
            }
        }


        private static unsafe bool StructEqual<T1, T2>(T1 v1, T2 v2) where T1 : unmanaged
                                                                     where T2 : unmanaged
        {
            if(sizeof(T1) != sizeof(T2)) { return false; }
            var s1 = new Span<byte>(&v1, sizeof(T1));
            var s2 = new Span<byte>(&v2, sizeof(T2));
            return s1.SequenceEqual(s2);
        }
    }
}
