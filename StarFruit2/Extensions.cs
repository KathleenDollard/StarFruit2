using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace StarFruit2
{
    public static class StarfruitExtensions
    {
        public static bool HasAttribute(this PropertyDeclarationSyntax prop, Type attributeType)
             => prop.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == attributeType.Name));
        public static bool HasAttribute(this ParameterSyntax param, Type attributeType)
            => param.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == attributeType.Name));
        public static bool HasAttribute<T>(this PropertyDeclarationSyntax prop)
            => prop.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == typeof(T).Name));
        public static bool HasAttribute<T>(this ParameterSyntax param)
            => param.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString() == typeof(T).Name));

        public static bool HasAttribute(this IPropertySymbol prop, Type attributeType)
            => prop.GetAttributes().Any(x => x.AttributeClass?.Name == attributeType.Name);
        public static bool HasAttribute(this IParameterSymbol param, Type attributeType)
            => param.GetAttributes().Any(x => x.AttributeClass?.Name == attributeType.Name);
        public static bool HasAttribute<T>(this IPropertySymbol prop)
            => prop.GetAttributes().Any(x => x.AttributeClass?.Name == typeof(T).Name);
        public static bool HasAttribute<T>(this IParameterSymbol param)
            => param.GetAttributes().Any(x => x.AttributeClass?.Name == typeof(T).Name);
    }
}
