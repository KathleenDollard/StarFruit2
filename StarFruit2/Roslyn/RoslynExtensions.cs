using Microsoft.CodeAnalysis;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;
using System;

namespace StarFruit2
{
    public static class RoslynExtensions
    {

        private static RawInfoBase RawInfoTypeFromSymbol(this ISymbol symbol,  bool isParentAMethod)
        {
            return symbol switch
            {

                null => (RawInfoBase)new RawInfoForNull(),
                ITypeSymbol x => new RawInfoForType(x),
                IMethodSymbol x => new RawInfoForMethod(x, x.IsStatic),
                IPropertySymbol x => new RawInfoForProperty(x),
                IParameterSymbol x => isParentAMethod 
                                            ? new RawInfoForMethodParameter(x)
                                            : new RawInfoForCtorParameter(x),
                _ => new RawInfoForUnknown(),
            };
        }

        public static RawInfoBase RawInfoTypeFromSymbol(this ISymbol symbol, ISymbol parent)
            => RawInfoTypeFromSymbol(symbol, parent is IMethodSymbol );
        public static RawInfoBase RawInfoTypeFromSymbol(this ISymbol symbol, ISymbolDescriptor parent)
            => RawInfoTypeFromSymbol(symbol, parent?.RawInfo is RawInfoForMethod);
        public static RawInfoBase RawInfoTypeFromSymbol(this ISymbol symbol)
            => RawInfoTypeFromSymbol(symbol, false);
    }
}
