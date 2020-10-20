using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public static bool BoolAttributeValue(this ISymbol symbol, Type attributeType)
        {
            var attribute = symbol.GetAttributes()
                                          .Where(x => symbol.GetAttributes().First().AttributeClass?.Name == attributeType.Name)
                                                   //|| symbol.GetAttributes().First().AttributeClass?.Name + "Attribute" == attributeType.Name)
                                          .FirstOrDefault();
            if (attribute is null)
            {
                return false;
            }

            var attributeArgs = attribute.ConstructorArguments;
            if (attributeArgs.Any() && !attributeArgs.First().IsNull)
            {
                var value = attributeArgs.First().Value;
                return value is null
                    ? false
                    : (bool)value;
            }

            return true; // attribute is present, lacks a value, which indicates true
        }

        public static bool BoolAttributeValue<TAttribute>(this ISymbol symbol)
        {
            return symbol.BoolAttributeValue(typeof(TAttribute));
        }

        public static IEnumerable<T> AttributeValueForList<T>(this ISymbol symbol, Type attributeType)
        {
            var attribute = symbol.GetAttributes()
                                          .Where(x => symbol.GetAttributes().First().AttributeClass?.Name == attributeType.Name)
                                          //|| symbol.GetAttributes().First().AttributeClass?.Name + "Attribute" == attributeType.Name)
                                          .FirstOrDefault();
            if (attribute is null)
            {
                return Enumerable.Empty<T>();
            }

            var attributeArgs = attribute.ConstructorArguments;
            if (attributeArgs.Any() && !attributeArgs.First().IsNull)
            {
                return attributeArgs.Where(x=>x.Kind == TypedConstantKind.Array )
                                    .SelectMany(x => x.Values)
                                    .Select(x=>x.Value)
                                    .OfType<T>();
            }

            return Enumerable.Empty<T>(); 
        }

        public static IEnumerable<T> AttributeValueForList<TAttribute,T>(this ISymbol symbol)
        {
            return symbol.AttributeValueForList<T>(typeof(TAttribute));
        }
    }
}
