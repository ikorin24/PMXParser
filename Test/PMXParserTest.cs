#nullable enable
using Xunit;
using MMDTools;
using System.IO;

namespace Test
{
    public class PMXParserTest
    {
        const string FILES = "../../../Files/";
        const string HIDDEN = FILES +  "Hidden/";

        [Theory(DisplayName = "PMX Ver 2.0")]
        [InlineData(FILES + "Alicia/Alicia_blade.pmx", PMXVersion.V20)]
        [InlineData(FILES + "Alicia/Alicia_solid.pmx", PMXVersion.V20)]
        [InlineData(FILES + "Appearance Miku/Appearance Miku.pmx", PMXVersion.V20)]
        [InlineData(FILES + "Appearance Miku/Appearance Miku_BDEF.pmx", PMXVersion.V20)]


        // Following cases of test are passed, but thier PMX does not exist in the repository because of their license.
        // (Redistribution does not allowed.)
#if false
        [InlineData(HIDDEN + "Mirai_Akari_v1.0/MiraiAkari_v1.0.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "初音ミクver.2.1/初音ミクver.2.1.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "癒月ちょこ公式mmd_ver1.0/癒月ちょこ.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "紫咲シオン公式mmd_ver1.01/紫咲シオン.pmx", PMXVersion.V20)]
        [InlineData(HIDDEN + "くむ式キバナ01/くむ式キバナ01.pmx", PMXVersion.V20)]
#endif
        public void PMXParseVer20(string fileName, PMXVersion version)
        {
            PMXParser.GetVersion(fileName).Is(version);

            using(var stream = File.OpenRead(fileName)) {
                var pmx = PMXParser.Parse(stream);
                stream.Position.Is(stream.Length);
            }
        }



        // There are no Tests for PMX ver 2.1.
        // I have no PMX of ver 2.1, and can not find it in the Web. Why? Unmm...
    }
}
