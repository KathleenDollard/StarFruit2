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

        protected override ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
                                                                        IPropertySymbol propertySymbol)
            // ** How to find syntax: var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
            => new ArgumentDescriptor(new ArgTypeInfoRoslyn(propertySymbol.Type), parent, propertySymbol.Name, propertySymbol)
            {
                Name = propertySymbol.Name,
                CliName = config.ArgumentNameToCliName(propertySymbol.Name),
                Description = config.GetDescription(propertySymbol) ?? "",
                Required = config.GetIsRequired(propertySymbol),
                IsHidden = config.GetIsHidden(propertySymbol),
            };

        protected override OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
                                                                   IPropertySymbol propertySymbol)
        {
            var option = new OptionDescriptor(parent, propertySymbol.Name, propertySymbol)
            {
                Name = propertySymbol.Name,
                CliName = config.OptionNameToCliName(propertySymbol.Name),
                Description = config.GetDescription(propertySymbol) ?? ""
            };

            option.Arguments.Add(CreateOptionArgumentDescriptor(parent, propertySymbol));
            return option;
        }

        private ArgumentDescriptor CreateOptionArgumentDescriptor(ISymbolDescriptor parent,
                                                                  IPropertySymbol propertySymbol)
        => new ArgumentDescriptor(new ArgTypeInfoRoslyn(propertySymbol.Type),
                                  parent,
                                  propertySymbol.Name,
                                  propertySymbol)
        {
            Name = propertySymbol.Name,
            CliName = config.OptionArgumentNameToCliName(propertySymbol.Name),
            Description = config.GetDescription(propertySymbol) ?? ""
        };

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
