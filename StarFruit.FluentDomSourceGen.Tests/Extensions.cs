using System;
using System.Linq;
using System.Text;

namespace StarFruit.FluentDomSourceGen.Tests
{
    internal static class Extensions
    {
        internal static string NormalizeLineEndings(this string value)
        {
            // normalize unicode, combining characters, diacritics
            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        internal static string NormalizeWhitespace(this string value)
        {
            if (value is null)
            {
                throw new InvalidOperationException("You cannot normalize strings that are null");
            }
            // normalize unicode, combining characters, diacritics
            value = value.Normalize(NormalizationForm.FormC);
            value = value.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = value.Split("\n");
            value = string.Join("\n", lines.Select(x => x.Trim()));
            value = ReplaceWithRepeating(value, "\n\n", "\n");
            value = ReplaceWithRepeating(value, "  ", " ");

            return value.Trim();
        }

        private static string ReplaceWithRepeating(string value, string oldValue, string newValue)
        {
            var len = value.Length + 1; // plus one forces this to run at least once
            while (len != value.Length)
            {
                len = value.Length;
                value = value.Replace(oldValue, newValue);
            }

            return value;
        }

    }
}
