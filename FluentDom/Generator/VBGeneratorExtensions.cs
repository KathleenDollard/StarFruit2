using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom.Generator
{
    public static class VBGeneratorExtensions
    {
        public static string VBString(this Scope scope)
            => scope.ToString();
    }
}