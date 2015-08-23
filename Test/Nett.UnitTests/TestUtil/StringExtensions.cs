﻿using System.IO;

namespace Nett.UnitTests.TestUtil
{
    internal static class StringExtensions
    {
        public static Stream ToStream(this string s)
        {
            var ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(s);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}
