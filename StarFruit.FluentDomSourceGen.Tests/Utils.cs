using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarFruit.FluentDomSourceGen.Tests
{
    internal class Utils
    {
        internal static string NormalizeLineEndings(string value)
        {
            // normalize unicode, combining characters, diacritics
            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}
