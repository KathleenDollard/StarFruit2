using Microsoft.CodeAnalysis;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;

namespace StarFruit2
{
    public static class RoslynExtensions
    {

        public static string OriginalElementTypeFromSymbol(this ISymbol symbol, ISymbolDescriptor? parent)
        {
            return symbol switch
            {

                null => OriginalElementType.Null,
                ITypeSymbol => OriginalElementType.Class,
                IMethodSymbol => OriginalElementType.Method,
                IPropertySymbol => OriginalElementType.Property,
                IParameterSymbol => parent is null
                                       ? OriginalElementType.Null
                                       : parent.OriginalElementType == OriginalElementType.Method 
                                            ? OriginalElementType.MethodParameter
                                            : OriginalElementType.CtorParameter ,
                _ => OriginalElementType.Null,
            };
        }

    }
}
