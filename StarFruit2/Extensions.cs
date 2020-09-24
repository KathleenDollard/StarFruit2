using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarFruit2
{
    public static class StarfruitExtensions
    {
        public static bool HasAttribute(this PropertyDeclarationSyntax prop, Type attributeType)
             => prop.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == attributeType.Name));
        public static bool HasAttribute(this ParameterSyntax  prop, Type attributeType)
            => prop.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == attributeType.Name));
    }
}
