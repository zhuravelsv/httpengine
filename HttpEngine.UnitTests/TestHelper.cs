using HttpEngine.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpEngine.UnitTests
{
    internal static class TestHelper
    {
        public static string CreateFakeString(string targetString = null)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(targetString ?? "hello worD");
            string str = MemoryHelpers.MakeString(bytes);
            return str;
        }
        public static string CreateFakeString(byte[] bytes)
        {
            string str = MemoryHelpers.MakeString(bytes);
            return str;
        }
    }
}
