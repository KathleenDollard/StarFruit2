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

    public class ClassSyntaxCommandMaker : DescriptorMakerBase<ClassDeclarationSyntax, PropertyDeclarationSyntax, CSharpCompilation>
    {
        private readonly ISymbolDescriptor? parent;
        private readonly MakerConfigurationBase config;
        private readonly ClassDeclarationSyntax source;
        private readonly CSharpCompilation? compilation;

        public ClassSyntaxCommandMaker(ISymbolDescriptor? parent, MakerConfigurationBase config, ClassDeclarationSyntax source, CSharpCompilation? compilation = null)
        {
            this.parent = parent;
            this.config = config;
            this.source = source;
            this.compilation = compilation;
        }

        protected internal override CliDescriptor CreateCliDescriptor()
        {
            var semanticModel = GetSemanticModel(source, compilation);
            var classSymbol = semanticModel.GetDeclaredSymbol(source);
            if (classSymbol is null)
            {
                throw new InvalidOperationException("Symbol not found");
            }

            var cliDesriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = classSymbol.ContainingNamespace.ToString(),
                CommandDescriptor = CreateCommandDescriptor(parent, source, semanticModel)
            };
            return cliDesriptor;
        }

        protected CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                            ClassDeclarationSyntax source,
                                                            SemanticModel semanticModel)
        {
            var typeSymbol = semanticModel.GetDeclaredSymbol(source);
            return CreateCommandDescriptor(parent, null, source, typeSymbol, semanticModel);
        }

        private IEnumerable<OptionDescriptor> GetOptions(ISymbolDescriptor parent,
                                                         ClassDeclarationSyntax source,
                                                         INamedTypeSymbol typeSymbol,
                                                         SemanticModel semanticModel)
            => typeSymbol.GetMembers()
                         .OfType<IPropertySymbol>()
                         .Where(p => config.IsOption(p.Name, p.GetAttributes()))
                         .Select(o => CreateOptionDescriptor(parent, o, source, typeSymbol, semanticModel));

        private IEnumerable<ArgumentDescriptor> GetArguments(ISymbolDescriptor parent,
                                                             ClassDeclarationSyntax source,
                                                             ITypeSymbol typeSymbol,
                                                             SemanticModel semanticModel)
         => typeSymbol.GetMembers()
                      .OfType<IPropertySymbol>()
                      .Select(m => CreateArgumentDescriptor(parent, m, source, typeSymbol, semanticModel));

        private IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent,
                                                              ClassDeclarationSyntax source,
                                                              ITypeSymbol typeSymbol,
                                                              SemanticModel semanticModel)
        {
            var derivedTypes = compilation is null
                               ? new List<ClassDeclarationSyntax> { }
                               : compilation.SyntaxTrees
                               .SelectMany(t => t.GetRoot()
                                                 .ChildNodes()
                                                 .OfType<ClassDeclarationSyntax>()
                                                 .Where(c => (c.BaseList?.Contains(source)).GetValueOrDefault()));
            return typeSymbol.GetMembers()
                                    .OfType<IMethodSymbol>()
                                    .Select(c => CreateCommandDescriptor(parent, typeSymbol, source, c, semanticModel))
                                    .Union(derivedTypes.Select(t => semanticModel.GetDeclaredSymbol(source))
                                                       .OfType<INamedTypeSymbol>()
                                                       .Select(t => CreateCommandDescriptor(parent, typeSymbol, source, t, semanticModel)));

        }

        private CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                          ITypeSymbol? parentTypeSymbol,
                                                          ClassDeclarationSyntax parentSource,
                                                          INamedTypeSymbol typeSymbol,
                                                          SemanticModel semanticModel)
        {
            Assert.NotNull(typeSymbol);
            var command = new CommandDescriptor(parent, source.Identifier.ToString(), source)
            {
                Name = SourceToCommandName(source.Identifier.ToString()),
            };
            command.AddArguments(GetArguments(command, source, typeSymbol, semanticModel));
            command.AddOptions(GetOptions(command, source, typeSymbol, semanticModel));
            command.AddCommands(GetSubCommands(command, source, typeSymbol, semanticModel));
            return command;
        }

        private CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                          ITypeSymbol? symbol,
                                                          ClassDeclarationSyntax parentSource,
                                                          IMethodSymbol methodSymbol,
                                                          SemanticModel semanticModel)
        {
            var propertyDeclaration = methodSymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
            Assert.NotNull(propertyDeclaration);
            var command = new CommandDescriptor(parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
            {
                Name = SourceToOptionName(propertyDeclaration.Identifier.ToString())
            };
            return command;
        }

        private ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
                                                            IPropertySymbol propertySymbol,
                                                            ClassDeclarationSyntax source,
                                                            ITypeSymbol typeSymbol,
                                                            SemanticModel semanticModel)
        {
            // I think for properties this is always a single value
            var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
            Assert.NotNull(propertyDeclaration);
            return new ArgumentDescriptor(new ArgTypeInfo(propertyDeclaration.Type), parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
            {
                Name = SourceToArgumentName(propertyDeclaration.Identifier.ToString())
            };
        }

        private OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
                                                        IPropertySymbol propertySymbol,
                                                        ClassDeclarationSyntax source,
                                                        ITypeSymbol typeSymbol,
                                                        SemanticModel semanticModel)
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

        protected internal override bool IsArgument(PropertyDeclarationSyntax property)
         => property.Identifier.ToString().EndsWith("Arg")
             || property.HasAttribute(typeof(ArgumentAttribute));

    }
}
