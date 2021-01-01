using Microsoft.CodeAnalysis;
using Starfruit2;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Generate
{
    public static class RoslynCSharpDescriptorFactory
    {
        public static CliDescriptor GetCliDescriptor(string languageName, ISymbol symbol, SemanticModel semanticModel)
        {
            var maker = new RoslynDescriptionMaker(semanticModel);
            return maker.CreateCliDescriptor(null, symbol);
        }
    }
}
