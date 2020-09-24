using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.CommandLine.Parsing;
using System.Linq;

namespace Starfruit2_B
{
    public class DescriptorMakerBase
    {
        // Create disembodied list of commands. These commands may or may not have their children attached. 
        // Arrange and check for circularity

        protected internal string SourceToOptionName(string sourceName)
        => $"--{sourceName.ToKebabCase()}";

        protected internal string SourceToArgumentName(string sourceName)
        {
            if (sourceName.EndsWith("Arg"))
            {
                sourceName = sourceName[..^3];
            }
            return sourceName.ToKebabCase();
        }

        protected internal string SourceToCommandName(string sourceName)
            => sourceName.ToKebabCase();
    }

    public abstract class DescriptorMakerBase<TCommandSource, TOptionArgSource> : DescriptorMakerBase
    {
        protected internal abstract CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent, TCommandSource source);
        //protected abstract OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor? parent, TOptionArgSource parameterDeclaration);
        protected internal abstract bool IsArgument(TOptionArgSource source);
        protected internal bool IsOption(TOptionArgSource source)
        => !IsArgument(source);

    }

    public static class RoslyDescriptorMakerFactory
    {
        public static CommandDescriptor CreateCommandDescriptor<T>(T source)
            where T : Microsoft.CodeAnalysis.SyntaxNode
        => source switch
        {
            ClassDeclarationSyntax classDeclaration => new CommandMakerClassSyntax().CreateCommandDescriptor (null, classDeclaration),
            MethodDeclarationSyntax methodDeclaration => new CommandMakerMethodSyntax().CreateCommandDescriptor(null, methodDeclaration),
            _ => throw new InvalidOperationException("Unexpected syntax type")
        };

        public static CommandDescriptor CreateCommandDescriptor(SyntaxNode source, Compilation compilation)
        {
            return compilation switch
            {
                CSharpCompilation c => CreateCommandDescriptor(source, c),
                _ => throw new NotImplementedException()
            };
        }

     private static CommandDescriptor CreateCommandDescriptor(SyntaxNode source, CSharpCompilation compilation)
        {
            return source switch
            {
                ClassDeclarationSyntax classDeclaration => CreateCommandDescriptor(null, classDeclaration, compilation),
                MethodDeclarationSyntax methodDeclaration => CreateCommandDescriptor(null, methodDeclaration, compilation),
                CompilationUnitSyntax compUnitSyntax => CreateCommandDescriptor(null, compUnitSyntax, compilation),
                _ => throw new InvalidOperationException("Unexpected syntax type")
            };
        }

        private static CommandDescriptor CreateCommandDescriptor(object p, CompilationUnitSyntax source, CSharpCompilation compilation)
        {
            SemanticModel symbol = compilation.GetSemanticModel(source.SyntaxTree);
            throw new NotImplementedException();
        }

        private static CommandDescriptor CreateCommandDescriptor(object p, MethodDeclarationSyntax source, CSharpCompilation compilation)
        {
            SemanticModel symbol = compilation.GetSemanticModel (source.SyntaxTree );
            throw new NotImplementedException();
        }

        private static CommandDescriptor CreateCommandDescriptor(object p, ClassDeclarationSyntax source, CSharpCompilation compilation)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RoslynDescriptorMaker<TCommandSource, TOptionArgSource> : DescriptorMakerBase<TCommandSource, TOptionArgSource>
        where TCommandSource : CSharpSyntaxNode
        where TOptionArgSource : CSharpSyntaxNode

    { }

    public class CommandMakerMethodSyntax : DescriptorMakerBase<MethodDeclarationSyntax, ParameterSyntax>
    {
        protected internal override CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent, MethodDeclarationSyntax source)
        {
            var command = new CommandDescriptor(parent, source.Identifier.ToString(), source)
            {
                Name = SourceToCommandName(source.Identifier.ToString()),
            };
            command.AddArguments(source.ParameterList.Parameters
                                    .Where(p => IsArgument(p))
                                    .Select(p => CreateArgumentDescriptor(parent, p)));
            command.AddOptions(source.ParameterList.Parameters
                                    .Where(p => IsOption(p))
                                    .Select(p => CreateOptionDescriptor(parent, p)));
            return command;
        }

        protected OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor? parent, ParameterSyntax parameterDeclaration)
        {
            var option = new OptionDescriptor(parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
            {
                Name = SourceToOptionName(parameterDeclaration.Identifier.ToString())
            };
            option.Arguments.Add(CreateOptionArgumentDescriptor(parent, parameterDeclaration));
            return option;
        }

        private ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor? parent, ParameterSyntax parameterDeclaration)
        => new ArgumentDescriptor(new ArgTypeInfo(parameterDeclaration.Type), parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
        {
            Name = SourceToArgumentName(parameterDeclaration.Identifier.ToString())
        };

        private ArgumentDescriptor CreateOptionArgumentDescriptor(ISymbolDescriptor? parent, ParameterSyntax parameterDeclaration)
        => new ArgumentDescriptor(new ArgTypeInfo(parameterDeclaration.Type), parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
        {
            Name = SourceToArgumentName(parameterDeclaration.Identifier.ToString())
        };

        protected internal override bool IsArgument(ParameterSyntax parameter)
        => parameter.Identifier.ToString().EndsWith("Arg")
            || parameter.HasAttribute(typeof(ArgumentAttribute));
    }

    public class CommandMakerClassSyntax : DescriptorMakerBase<ClassDeclarationSyntax, PropertyDeclarationSyntax>
    {
        protected internal override CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent, ClassDeclarationSyntax source)
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
