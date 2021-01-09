using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using FluentDom;

namespace StarFruit2.Generate
{
    public class CSharpSyntaxReceiver : SyntaxReceiverBase
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
                return compilationUnit.Usings.Select(y => new Using(y.Name.ToString(), y.Alias?.ToString()));
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
                if (syntaxNode is ClassDeclarationSyntax classDeclaration
                    && classDeclaration.BaseList is not null
                    && classDeclaration.BaseList.DescendantNodes()
                                                .OfType<IdentifierNameSyntax>()
                                                .Any(x => x.Identifier.ValueText == "ICliRoot"))
                {
                    if (!candidateCliTypes.Any(x => x == classDeclaration))
                    {
                        usings.AddRange(CheckUsings(classDeclaration));
                        candidateCliTypes.Add(classDeclaration);
                        return true;
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
                                    if (invocation.ArgumentList.Arguments.Count() == 1
                                        && invocation.ArgumentList.Arguments.First().Expression is InvocationExpressionSyntax nameofInvocation
                                        && nameofInvocation.Expression is IdentifierNameSyntax nameIdentifier
                                        && nameIdentifier.Identifier.ValueText == "nameof")
                                    {
                                        var methodId = nameofInvocation.ArgumentList.Arguments.First().Expression;
                                        if (methodId is MemberAccessExpressionSyntax s)
                                        {
                                            usings.AddRange(CheckUsings(s));
                                            candidatesFromMethods.Add(s);
                                            return true;
                                        }
                                        if (methodId is IdentifierNameSyntax n)
                                        {
                                            usings.AddRange(CheckUsings(n));
                                            candidatesFromMethods.Add(n); 
                                            return true;
                                        }
                                        return false;
                                    }
            return false;

        }
    }
}
}
