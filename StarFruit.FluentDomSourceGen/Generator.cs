using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Starfruit2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentDom;
using System.Diagnostics;
using FluentDom.Generator;

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
            try
            {
                //Debugger.Launch();
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0001", "Generator entered", "", "KDGenerator", DiagnosticSeverity.Info, true),
                    null));

                if (context.SyntaxReceiver is not SyntaxReceiver receiver)
                    return;

                var source = "";
                foreach (var declaration in receiver.CandidateCliTypes)
                {
                    var cliDescriptor = RoslyDescriptorMakerFactory.CreateCliDescriptor(declaration, context.Compilation as CSharpCompilation);
                    //var tempSource += $"\npublic class Temp{cliDescriptor.CommandDescriptor.OriginalName}{{}}\n";
                    //OutputCode(tempSource, context, $"Temp.generated.cs");
                    OutputCode(new GenerateCommandSource().CreateCode(cliDescriptor), context, $"{cliDescriptor.CommandDescriptor.OriginalName}CommandSource.generated.cs");
                    OutputCode(new GenerateCommandSourceResult().CreateCode(cliDescriptor), context, $"{cliDescriptor.CommandDescriptor.OriginalName}CommandSourceResult.generated.cs");
                }

                if (source != null)
                {
                    //Debugger.Launch();
                    context.AddSource("generated.cs", source);
                }

             }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0002", "Generator failed", ex.ToString(), "KDGenerator", DiagnosticSeverity.Error, true),
                    null));
            }

            static void OutputCode(Code code, GeneratorExecutionContext context, string fileName)
            {
                var source = new CSharpGenerator().Generate(code);
                if (source is not null)
                {
                    context.AddSource(fileName, source);
                }
            }
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
            try
            {
                if (syntaxNode is InvocationExpressionSyntax invocation
                    && invocation.Expression is MemberAccessExpressionSyntax memberAccess
                    && memberAccess.Expression is IdentifierNameSyntax classIdentifier
                    && classIdentifier.Identifier.ValueText == "CommandSource"
                    && memberAccess.Name is GenericNameSyntax methodName
                    && methodName.Identifier is SyntaxToken identifier
                    && identifier.ValueText == "Run")
                {
                    var cliRootName = methodName.TypeArgumentList.Arguments.FirstOrDefault();
                    if (cliRootName is not null
                        && !CandidateCliTypeferences.Contains(cliRootName))
                    {
                        CandidateCliTypeferences.Add(cliRootName);
                    }
                }

                if (syntaxNode is ClassDeclarationSyntax classDeclaration
                    && classDeclaration.BaseList is not null
                    && classDeclaration.BaseList.DescendantNodes()
                                                .OfType<IdentifierNameSyntax>()
                                                .Any(x => x.Identifier.ValueText == "ICliRoot"))
                {
                    if (!CandidateCliTypes.Contains(classDeclaration))
                    {
                        CandidateCliTypes.Add(classDeclaration);
                    }
                }
            }
            catch
            {
                Debugger.Launch();
            }
        }
    }
}
