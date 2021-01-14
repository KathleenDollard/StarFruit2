using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RoslynSourceGenSupport.CSharp
{
    public static class CSharpSourceGenExtensions
    {
        public static IEnumerable<UsingDirectiveSyntax> GetUsings(this SyntaxNode? syntaxNode)
        {
            if (syntaxNode is null)
                return new UsingDirectiveSyntax[] { };

            var compilationUnit = syntaxNode.Ancestors()
                                            .OfType<CompilationUnitSyntax>()
                                            .FirstOrDefault();
            return compilationUnit.Usings;
        }

        public static MemberAccessExpressionSyntax? IfCallToMethodOnClass(this SyntaxNode? syntaxNode, string className)
        {
            if (syntaxNode is InvocationExpressionSyntax invocationSyntax)
                if (invocationSyntax.Expression is MemberAccessExpressionSyntax memberAccessSyntax)
                    if (memberAccessSyntax.Expression is IdentifierNameSyntax classNameSyntax)
                        if (classNameSyntax.Identifier.ValueText == className)
                            return memberAccessSyntax;
            return null;
        }

        public static InvocationExpressionSyntax? IfCallToMethod(this SyntaxNode? syntaxNode, string methodName)
        {
            if (syntaxNode is InvocationExpressionSyntax invocationSyntax)
                if (invocationSyntax.Expression is IdentifierNameSyntax nameIdentifier)
                    if (nameIdentifier.Identifier.ValueText == methodName)
                        return invocationSyntax;
            return null;
        }

        public static IEnumerable<TypeSyntax> GenericArgumentsFromName(this SyntaxNode? syntaxNode)
            => syntaxNode is GenericNameSyntax genericName
                    ? genericName.TypeArgumentList.Arguments.ToList()
                    : (IEnumerable<TypeSyntax>)(new TypeSyntax[] { });

        public static ClassDeclarationSyntax? IfClassWithBaseOrInterface(this SyntaxNode syntaxNode, string name)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclaration
                               && classDeclaration.BaseList is not null
                               && classDeclaration.BaseList.DescendantNodes()
                                                           .OfType<IdentifierNameSyntax>()
                                                           .Any(x => x.Identifier.ValueText == name))
            {
                return classDeclaration;
            }
            return null;
        }

        public static IEnumerable<ArgumentSyntax> ArgumentsOnMethod(this SyntaxNode? syntaxNode)
        {
            return syntaxNode switch
            {
                MemberAccessExpressionSyntax x => ArgumentsOnMethod(x),
                InvocationExpressionSyntax x => ArgumentsOnMethod(x),
                _ => new ArgumentSyntax[] { }
            };
        }

        public static IEnumerable<ArgumentSyntax> ArgumentsOnMethod(this MemberAccessExpressionSyntax? syntaxNode)
        {
            var invocation = syntaxNode?.Ancestors()
                                       .OfType<InvocationExpressionSyntax>()
                                       .FirstOrDefault();
            return invocation is not null
                ? ArgumentsOnMethod(invocation)
                : new ArgumentSyntax[] { };
        }

        public static IEnumerable<ArgumentSyntax> ArgumentsOnMethod(this InvocationExpressionSyntax? invocation)
            => invocation is null
                   ? new ArgumentSyntax[] { }
                   : invocation.ArgumentList.Arguments.ToList();
    }
}
