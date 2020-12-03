using System;

namespace FluentDom
{
    public class Using
    {
        public Using(string usingNamespace, string alias = null, bool usingStatic = false)
        {
            UsingNamespace = usingNamespace;
            Alias = alias;
            UsingStatic = usingStatic;
        }

        public string UsingNamespace { get; }
        public string Alias { get; }
        public bool UsingStatic { get; } // VB ignores this since just importing the type has the same effect as C#

        public static implicit operator Using(string s) => new Using(s);
    }
}