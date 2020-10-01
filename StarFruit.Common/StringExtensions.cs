using System;
using System.Collections.Generic;
using System.Text;

namespace StarFruit.Common
{
    public static class StringExtensions
    {
        public static string AfterLast(this string input, string afterString)
        {
            var pos = input.LastIndexOf(afterString);
            return input[(pos + 1)..];
        }
    }
}
