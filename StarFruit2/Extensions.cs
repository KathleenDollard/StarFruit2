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

        public static bool HasAttribute(this ISymbol prop, Type attributeType)
            => prop.GetAttributes()
                   .Any(x => x.AttributeClass?.Name == attributeType.Name);
        public static bool HasAttribute<TAttribute>(this ISymbol prop)
            => prop.GetAttributes()
                   .Any(x => x.AttributeClass?.Name == typeof(TAttribute).Name);

        public static object? AttributeValue<TAttribute>(ISymbol symbol)
        {
            var attribute = symbol.GetAttributes()
                                  .Where(x => x is TAttribute)
                                  .FirstOrDefault();
            if (attribute is null)
            {
                return default;
            }
            var valueConst = attribute.ConstructorArguments.FirstOrDefault();
            return valueConst.Value;
        }
    }
}
