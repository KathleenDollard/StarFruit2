using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace StarFruit2.Generate
{
    [Generator]
    public class CSharpPingGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new PingCSharpSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not PingCSharpSyntaxReceiver receiver)
                return;
            
            var source = $"\npublic class TempPing4{{}}\n";
            source += $"// {receiver.CandidateCliTypes.Count()}";
            context.AddSource("generated.cs", source);
        }
    }

    public class PingCSharpSyntaxReceiver : ISyntaxReceiver
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
