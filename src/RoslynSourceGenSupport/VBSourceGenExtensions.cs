using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RoslynSourceGenSupport.VisualBasic
{
    public static class VBSourceGenExtensions
    {
        public static IEnumerable<SimpleImportsClauseSyntax> GetUsings(this SyntaxNode? syntaxNode)
        {
            if (syntaxNode is null)
                return new SimpleImportsClauseSyntax[] { };

            var compilationUnit = syntaxNode.Ancestors()
                                            .OfType<CompilationUnitSyntax>()
                                            .FirstOrDefault();
            return compilationUnit.Imports.SelectMany(x => x.ImportsClauses).OfType<SimpleImportsClauseSyntax>();
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

        public static ClassBlockSyntax? IfClassWithBaseOrInterface(this SyntaxNode syntaxNode, string name)
        {
            if (syntaxNode is ClassBlockSyntax classBlock)
               if ( classBlock.ChildNodes()     
                    .OfType<InheritsOrImplementsStatementSyntax>()
                    .Any(x => true))
            {
                return classBlock;
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
