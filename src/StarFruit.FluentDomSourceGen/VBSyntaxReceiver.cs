using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;
using FluentDom;
using System;

namespace StarFruit2.Generate
{
    public class VBSyntaxReceiver : SyntaxReceiverBase
    {

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public override void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!AddPocoTypes(syntaxNode, Candidates, Usings))
                if (!AddInterfaceMarkedTypes(syntaxNode, Candidates, Usings))
                    AddCalledMethods(syntaxNode, Candidates, Usings);


            static IEnumerable<Using> CheckUsings(SyntaxNode syntaxNode)
            {
                var compilationUnit = syntaxNode.Ancestors()
                                                .OfType<CompilationUnitSyntax>()
                                                .FirstOrDefault();
                var importClauses = compilationUnit.Imports.SelectMany(x => x.ImportsClauses).OfType<SimpleImportsClauseSyntax>();
                return importClauses.Select(y => new Using(y.Name.ToString(), y.Alias?.ToString()));
            }

            static bool AddPocoTypes(SyntaxNode syntaxNode, List<SyntaxNode> candidatePocoTypes, List<Using> usings)
            {
                // This allows specifying a simple POCO CLI root class as the CLI root
                // by stating hte generic in CommandSource static methods
                if (syntaxNode is InvocationExpressionSyntax invocation)
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                        if (memberAccess.Expression is IdentifierNameSyntax className)
                            if (className.Identifier.ValueText == "CommandSource")
                                if (memberAccess.Name is GenericNameSyntax genericName)
                                {
                                    var cliRoot = genericName.TypeArgumentList.Arguments.FirstOrDefault();
                                    if (cliRoot is not null
                                        && !candidatePocoTypes.Contains(cliRoot))
                                    {
                                        candidatePocoTypes.Add(cliRoot);
                                        usings.AddRange(CheckUsings(invocation));
                                        return true;
                                    }
                                }
                return false;

            }

            static bool AddInterfaceMarkedTypes(SyntaxNode syntaxNode, List<SyntaxNode> candidateCliTypes, List<Using> usings)
            {
                // This allows specifying the CLI root via a marker interface
                // It is not clear that this is still needed.
                if (syntaxNode is ClassBlockSyntax classBlock)
                {
                    var inheritsStatement = classBlock.ChildNodes().OfType<InheritsStatementSyntax>();
                    var implementedTypes = classBlock.ChildNodes().OfType<ImplementsStatementSyntax>().FirstOrDefault()?.Types.ToList();
                    if (implementedTypes is not null && implementedTypes.Any(x => true)) // is CliRoot
                    {
                        {
                            usings.AddRange(CheckUsings(classBlock));
                            candidateCliTypes.Add(classBlock);
                            return true;
                        }
                    }
                }
                return false;
            }

            static bool AddCalledMethods(SyntaxNode syntaxNode, List<SyntaxNode> candidatesFromMethods, List<Using> usings)
            {
                // This allows specifying a simple POCO CLI root class as the CLI root
                // by stating hte generic in CommandSource static methods
                if (syntaxNode is InvocationExpressionSyntax invocation)
                    if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                        if (memberAccess.Expression is IdentifierNameSyntax className)
                            if (className.Identifier.ValueText == "CommandSource")
                                if (memberAccess.Name is IdentifierNameSyntax) // any name, but not a generic
                                    if (invocation.ArgumentList.Arguments.Any())
                                    {
                                        switch (invocation.ArgumentList.Arguments.First().GetExpression())
                                        {
                                            case NameOfExpressionSyntax s:
                                                var rootIdentifier = s.Argument switch
                                                {
                                                    IdentifierNameSyntax r => (SyntaxNode)r,
                                                    MemberAccessExpressionSyntax r => r,
                                                    _ => throw new NotImplementedException(),
                                                };
                                                usings.AddRange(CheckUsings(s));
                                                candidatesFromMethods.Add(rootIdentifier);
                                                return true;

                                            case MemberAccessExpressionSyntax s:
                                                usings.AddRange(CheckUsings(s));
                                                candidatesFromMethods.Add(s);
                                                return true;

                                            default:
                                                return false;

                                        }
                                    }

                return false;

            }
        }
    }
}
