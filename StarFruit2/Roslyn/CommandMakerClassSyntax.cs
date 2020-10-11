using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starfruit2_B
{

    public class ClassSyntaxCommandMaker : DescriptorMakerBase<INamedTypeSymbol, IPropertySymbol>
    {

        public ClassSyntaxCommandMaker( MakerConfigurationBase config, SemanticModel semanticModel)
            : base (config, semanticModel )
        { }

        // GetSubCommands places the restriction that all subcommands must be in the same namespace
        private IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent,
                                                              ITypeSymbol typeSymbol) 
            => typeSymbol.ContainingNamespace.GetTypeMembers()
                                             .Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, typeSymbol))
                                             .Select(x => CreateCommandDescriptor(parent, x));

        private CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                          IMethodSymbol methodSymbol)
        {
            var propertyDeclaration = methodSymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
            Assert.NotNull(propertyDeclaration);
            var command = new CommandDescriptor(parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
            {
                Name = SourceToOptionName(propertyDeclaration.Identifier.ToString())
            };
            return command;
        }

        protected  override ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
                                                                        IPropertySymbol propertySymbol)
        {
            // I think for properties this is always a single value
            var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
            Assert.NotNull(propertyDeclaration);
            return new ArgumentDescriptor(new ArgTypeInfo(propertyDeclaration.Type), parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
            {
                Name = SourceToArgumentName(propertyDeclaration.Identifier.ToString())
            };
        }

        protected override OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
                                                                   IPropertySymbol propertySymbol)
        {
            // I think for properties this is always a single value
            var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
            Assert.NotNull(propertyDeclaration);
            var option = new OptionDescriptor(parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
            {
                Name = SourceToOptionName(propertyDeclaration.Identifier.ToString())
            };
            option.Arguments.Add(CreateOptionArgumentDescriptor(parent, propertyDeclaration));
            return option;
        }

        private ArgumentDescriptor CreateOptionArgumentDescriptor(ISymbolDescriptor? parent, PropertyDeclarationSyntax propertyDeclaration)
        => new ArgumentDescriptor(new ArgTypeInfo(propertyDeclaration.Type), parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
        {
            Name = SourceToArgumentName(propertyDeclaration.Identifier.ToString())
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
