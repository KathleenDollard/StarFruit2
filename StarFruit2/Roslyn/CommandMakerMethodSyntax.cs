using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit.Common;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Linq;

namespace Starfruit2_B
{
    public class CommandMakerMethodSyntax : DescriptorMakerBase<MethodDeclarationSyntax, ParameterSyntax, CSharpCompilation>
    {
        protected internal override CliDescriptor CreateCliDescriptor(ISymbolDescriptor? parent,
                                                                      MethodDeclarationSyntax source,
                                                                      CSharpCompilation? compilation = null)
        {
            var semanticModel = GetSemanticModel(source, compilation);
            var methodSymbol = semanticModel.GetDeclaredSymbol(source);
            if (methodSymbol is null)
            {
                throw new InvalidOperationException("Symbol not found");
            }
            var cliDesriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = methodSymbol.ContainingNamespace.ToString() ?? "",
                CommandDescriptor = CreateCommandDescriptor(parent, source, semanticModel)
            };
            return cliDesriptor;

        }

        protected CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                           MethodDeclarationSyntax methodDeclaration,
                                                           SemanticModel semanticModel)
        {
            var command = new CommandDescriptor(parent, methodDeclaration.Identifier.ToString(), methodDeclaration)
            {
                Name = SourceToCommandName(methodDeclaration.Identifier.ToString()),
            };
            command.AddArguments(methodDeclaration.ParameterList.Parameters
                                    .Where(p => IsArgument(p))
                                    .Select(p => CreateArgumentDescriptor(parent, p, semanticModel)));
            command.AddOptions(methodDeclaration.ParameterList.Parameters
                                    .Where(p => IsOption(p))
                                    .Select(p => CreateOptionDescriptor(parent, p, semanticModel)));
            return command;
        }

        protected OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor? parent,
                                                          ParameterSyntax parameterDeclaration,
                                                          SemanticModel semanticModel)
        {
            var option = new OptionDescriptor(parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
            {
                Name = SourceToOptionName(parameterDeclaration.Identifier.ToString())
            };
            option.Arguments.Add(CreateOptionArgumentDescriptor(parent, parameterDeclaration, semanticModel));
            return option;
        }

        private ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor? parent,
                                                            ParameterSyntax parameterDeclaration,
                                                            SemanticModel semanticModel)
        => new ArgumentDescriptor(new ArgTypeInfo(parameterDeclaration.Type), parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
        {
            Name = SourceToArgumentName(parameterDeclaration.Identifier.ToString())
        };

        private ArgumentDescriptor CreateOptionArgumentDescriptor(ISymbolDescriptor? parent,
                                                                  ParameterSyntax parameterDeclaration,
                                                                  SemanticModel semanticModel)
        => new ArgumentDescriptor(new ArgTypeInfo(parameterDeclaration.Type), parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
        {
            Name = SourceToArgumentName(parameterDeclaration.Identifier.ToString())
        };

        protected internal override bool IsArgument(ParameterSyntax parameter)
        => parameter.Identifier.ToString().EndsWith("Arg")
            || parameter.HasAttribute(typeof(ArgumentAttribute));
    }
}
