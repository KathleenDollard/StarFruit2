using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StarFruit2.Tests
{
    internal class Utils
    {

        // From StackOverflow user11523568 with modification
        // This is a bit of a hack. 
        internal static string Normalize(string value)
        {
            if (value is null)
            {
                throw new InvalidOperationException("You cannot normalize strings that are null");
            }
            // normalize unicode, combining characters, diacritics
            value = value.Normalize(NormalizationForm.FormC);
            value = value.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = value.Split("\n");
            value = string.Join("\n", lines.Select(x=>x.Trim()));
            value = ReplaceWithRepeating(value, "\n\n", "\n");
            value = ReplaceWithRepeating(value, "  ", " ");

            return value.Trim();
        }

        private static string ReplaceWithRepeating(string value,  string oldValue, string newValue)
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
