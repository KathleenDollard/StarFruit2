﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Starfruit2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentDom;

namespace StarFruit2.Generate
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //System.Diagnostics.Debugger.Launch();
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
                return;

            var source = "";
            foreach (var declaration in receiver.CandidateCliTypes)
            {
                var cliDescriptor = RoslyDescriptorMakerFactory.CreateCliDescriptor(declaration, context.Compilation as CSharpCompilation);
                //source += $"\npublic class Temp{cliDescriptor.CommandDescriptor.OriginalName}{{}}\n";
                source += OutputCode(new GenerateCommandSource().CreateCode(cliDescriptor));
                source += OutputCode(new GenerateCommandSourceResult().CreateCode(cliDescriptor));
            }

            if (source != null)
            {
                context.AddSource("generated.cs", source);
            }

            static string OutputCode(Code code) 
                => new FluentDom.Generator.CSharpGenerator().Generate(code);
        }


    }

    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeSyntax> CandidateCliTypeferences { get; } = new();
        public List<ClassDeclarationSyntax> CandidateCliTypes { get; } = new();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InvocationExpressionSyntax invocation
                && invocation.Expression is MemberAccessExpressionSyntax memberAccess
                && memberAccess.Expression is IdentifierNameSyntax classIdentifier
                && classIdentifier.Identifier.ValueText == "CommandSource"
                && memberAccess.Name is GenericNameSyntax methodName
                && (string)methodName.Identifier.Value == "Run")
            {
                var cliRootName = methodName.TypeArgumentList.Arguments.FirstOrDefault();
                if (cliRootName is not null
                    && !CandidateCliTypeferences.Contains (cliRootName))
                {
                    CandidateCliTypeferences.Add(cliRootName);
                }
            }

            if (syntaxNode is ClassDeclarationSyntax  classDeclaration
                && classDeclaration.BaseList.DescendantNodes()
                                            .OfType<IdentifierNameSyntax>()
                                            .Any(x=>x.Identifier.ValueText == "ICliRoot"))
            {
                if (!CandidateCliTypes.Contains(classDeclaration))
                {
                    CandidateCliTypes.Add(classDeclaration);
                }
            }
        }
    }
}
