#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MMDTools;

namespace RunTest
{
    static class Program
    {
        static void Main()
        {
            var parser = new PMXPerser();
            var pmx = parser.Parse("test.pmx");
        }
    }
}
