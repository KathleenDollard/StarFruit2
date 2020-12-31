using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StarFruit2.Common
{
    public class Assert
    {
        public static T NotNull<T>([NotNull] T input, string? name = null)
                where T : class?
        {
            if (input is null)
            {
                var valueName = "Value";
                throw new InvalidOperationException($"{name ?? valueName} cannot be null");
            }
            return input;
        }
    }
}
