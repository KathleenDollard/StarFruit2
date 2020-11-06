using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorSupport
{
    public enum Scope
    {
        Public,
        Private,
        Internal,
        Protected
    }

    static class ScopeExtensions
    {
        public static string CSharpString(this Scope scope)
            => scope.ToString().ToLower();

        public static string VBString(this Scope scope)
            => scope.ToString();
    }

}
