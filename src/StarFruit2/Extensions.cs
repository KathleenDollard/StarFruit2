using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2
{
    public static class StarfruitExtensions
    {
        public static bool HasAttribute(this ISymbol prop, Type attributeType)
            => prop.GetAttributes()
                   .Any(x => x.AttributeClass?.Name == attributeType.Name);
        public static bool HasAttribute<TAttribute>(this ISymbol prop)
            => prop.GetAttributes()
                   .Any(x => x.AttributeClass?.Name == typeof(TAttribute).Name);

        public static bool BoolAttributeValue(this ISymbol symbol, Type attributeType, bool defaultValue = false)
        {
            var attribute = symbol.GetAttributes()
                                          .Where(x => symbol.GetAttributes().First().AttributeClass?.Name == attributeType.Name)
                                          //|| symbol.GetAttributes().First().AttributeClass?.Name + "Attribute" == attributeType.Name)
                                          .FirstOrDefault();
            if (attribute is null)
            {
                return defaultValue;
            }

            //if (attribute is BoolAttribute boolAttribute)
            //{ }

            var attributeArgs = attribute.ConstructorArguments;
            if (attributeArgs.Any() && !attributeArgs.First().IsNull)
            {
                var value = attributeArgs.First().Value;
                return value is null
                    ? defaultValue
                    : (bool)value;
            }

            return true; // attribute is present, lacks a value, which indicates true
        }

        public static bool BoolAttributeValue<TAttribute>(this ISymbol symbol, bool defaultValue = false)
        {
                return symbol.BoolAttributeValue(typeof(TAttribute), defaultValue);
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
                return attributeArgs.Where(x => x.Kind == TypedConstantKind.Array)
                                    .SelectMany(x => x.Values)
                                    .Select(x => x.Value)
                                    .OfType<T>();
            }

            return Enumerable.Empty<T>();
        }

        public static IEnumerable<T> AttributeValueForList<TAttribute, T>(this ISymbol symbol)
        {
            return symbol.AttributeValueForList<T>(typeof(TAttribute));
        }

        // This does not yet handle snake case
        public static string CamelCase(this string input)
        {
            var arr = input.ToCharArray();
            arr[0] = char.ToLowerInvariant(arr[0]);
            return new string(arr);
        }

        public static string RemoveLeadingTrailingQuotes(this string input)
        {
            var withoutLeading = input.StartsWith(@"""")
                                    ? input.Substring(1)
                                    : input;
            return withoutLeading.EndsWith(@"""")
                                    ? input.Substring(0, input.Length - 1)
                                    : input;

        }
    }
}
