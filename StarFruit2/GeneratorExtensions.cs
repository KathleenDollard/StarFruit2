using System;
using System.Collections.Generic;
using System.Text;

namespace StarFruit2.Generator
{
    public static class GeneratorExtensions
    {
        public static string EndStatement(this string expression)
            => expression.EndsWith(";") ? expression : $"{expression};";

        public static List<string> EndStatement(this List<string> expression)
        {
            //// re-assign the final string to be the final string with a semicolon
            //expression[^1] = EndStatement(expression[^1]);
            return expression;
        }
    }
}
