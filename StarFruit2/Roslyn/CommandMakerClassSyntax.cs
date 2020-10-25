using Microsoft.CodeAnalysis;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace Starfruit2
{

    public class ClassSyntaxCommandMaker : DescriptorMakerBase<INamedTypeSymbol, IPropertySymbol>
    {
        public ClassSyntaxCommandMaker(MakerConfiguration config, SemanticModel semanticModel)
            : base(config, semanticModel)
        { }


        protected override IEnumerable<IPropertySymbol> GetMembers(INamedTypeSymbol parentSymbol)
            => parentSymbol.GetMembers().OfType<IPropertySymbol>();


        // GetSubCommands places the restriction that all subcommands must be in the same namespace
        protected override IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent,
                                                                INamedTypeSymbol parentSymbol)
            => parentSymbol.ContainingNamespace
                           .GetTypeMembers()
                           .Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, parentSymbol))
                           .Select(x => CreateCommandDescriptor(parent, x));


        // TODO: Union with Method commands

    }
}
