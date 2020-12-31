using Microsoft.CodeAnalysis;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;

namespace StarFruit2
{
    public static class RoslynExtensions
    {

        private static RawInfoBase RawInfoTypeFromSymbol(this ISymbol symbol,  bool isParentAMethod)
        {
            return symbol switch
            {

                null => (RawInfoBase)new RawInfoForNull(),
                ITypeSymbol x => new RawInfoForType(x, x.ContainingNamespace.Name, x.ContainingType?.Name),
                IMethodSymbol x => new RawInfoForMethod(x, x.IsStatic, x.ContainingNamespace.Name, x.ContainingType.Name),
                IPropertySymbol x => new RawInfoForProperty(x, x.ContainingNamespace.Name, x.ContainingType.Name),
                IParameterSymbol x => isParentAMethod 
                                            ? new RawInfoForMethodParameter(x, x.ContainingNamespace.Name, x.ContainingType.Name, x.ContainingSymbol.Name)
                                            : new RawInfoForCtorParameter(x, x.ContainingNamespace.Name, x.ContainingType.Name),
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
