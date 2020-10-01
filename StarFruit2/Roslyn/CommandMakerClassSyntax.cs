using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Linq;

namespace Starfruit2_B
{
    public class CommandMakerClassSyntax : DescriptorMakerBase<ClassDeclarationSyntax, PropertyDeclarationSyntax, CSharpCompilation>
    {
        protected internal override CliDescriptor CreateCliDescriptor(ISymbolDescriptor? parent,
                                                                      ClassDeclarationSyntax source,
                                                                      CSharpCompilation? compilation = null)
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
                CommandDescriptor = CreateCommandDescriptor (parent, source, semanticModel )
            };
            return cliDesriptor;
        }

        protected CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                          ClassDeclarationSyntax source,
                                                          SemanticModel semanticModel)
        {
            var command = new CommandDescriptor(parent, source.Identifier.ToString(), source)
            {
                Name = SourceToCommandName(source.Identifier.ToString()),
                // SubCommands = GetSubCommandDescriptorSet(classDeclaration)
            };
            command.AddArguments(source.ChildNodes()
                                  .OfType<PropertyDeclarationSyntax>()
                                  .Where(p => IsArgument(p))
                                  .Select(p => CreateArgumentDescriptor(parent, p)));
            command.AddOptions(source.ChildNodes()
                                       .OfType<PropertyDeclarationSyntax>()
                                       .Where(p => !IsArgument(p))
                                       .Select(p => CreateOptionDescriptor(parent, p)));
            return command;
        }

        private ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor? parent, PropertyDeclarationSyntax propertyDeclaration)
        => new ArgumentDescriptor(new ArgTypeInfo(propertyDeclaration.Type), parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
        {
            Name = SourceToArgumentName(propertyDeclaration.Identifier.ToString())
        };

        private OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor? parent, PropertyDeclarationSyntax propertyDeclaration)
        {
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
