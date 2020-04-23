#nullable enable
using MMDTools;
using System;
using System.Runtime.CompilerServices;

namespace MMDTools
{
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

        public static void ValidateVersion(int version)
        {
            Assert(version == 20 || version == 21, "Invalid or not supported version");
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Assert(bool condition, string message)
        {
            if(!condition) { throw new FormatException(message); }
        }
    }
}
