#nullable enable
using Xunit;
using MMDTools;

namespace Test
{
    public class PMXParserTest
    {
        [Theory]
        [InlineData("../../../Files/Alicia/Alicia_blade.pmx")]
        [InlineData("../../../Files/Alicia/Alicia_solid.pmx")]
        public void PMXParse(string fileName)
        {
            var pmx = PMXParser.Parse(fileName);
        }
    }
}
