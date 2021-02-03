using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynSourceGenSupport
{
    public class CSharpRoslynUtilities : RoslynUtilitiesBase
    {
        public override SyntaxTree ParseToSyntaxTree(string source)
          => CSharpSyntaxTree.ParseText(source);

        public override IEnumerable<SyntaxToken> TokensWithName(SyntaxNode syntaxNode, string name)
            => syntaxNode.DescendantTokens()
                         .Where(x => x.IsKind(SyntaxKind.IdentifierToken) && x.ValueText == name);

        public override IEnumerable<SyntaxNode> ClassesWithName(SyntaxNode? syntaxNode, string className)
            => syntaxNode is null
                ? Enumerable.Empty<SyntaxNode>()
                : syntaxNode.DescendantNodesAndSelf()
                            .OfType<ClassDeclarationSyntax>()
                            .Where(x => x.Identifier.ValueText == className);

        public override IEnumerable<SyntaxNode> MethodsWithName(SyntaxNode? syntaxNode, string className)
            => syntaxNode is null
                ? Enumerable.Empty<SyntaxNode>()
                : syntaxNode.DescendantNodesAndSelf()
                            .OfType<MethodDeclarationSyntax>()
                            .Where(x => x.Identifier.ValueText == className);

        public override IEnumerable<SyntaxNode> ClassesWithBaseOrInterfaceNamed(SyntaxNode? syntaxNode, string baseOrInterfaceName)
        {
            var classes = syntaxNode is null
                                        ? Enumerable.Empty<ClassDeclarationSyntax>()
                                        : syntaxNode.DescendantNodesAndSelf()
                                                    .OfType<ClassDeclarationSyntax>();
            return classes.Where(x => x.BaseList is not null
                                        && x.BaseList.Types
                                                     .Where(y => TypeName(y) == baseOrInterfaceName)
                                                     .Any());
        }

        public override string? TypeName(SyntaxNode syntaxNode)
            => syntaxNode switch
            {
                TypeSyntax type => type switch
                {
                    SimpleNameSyntax syntax => syntax.Identifier.ValueText,
                    _ => throw new NotImplementedException()
                },
                ClassDeclarationSyntax syntax => syntax.Identifier.ValueText,
                SimpleBaseTypeSyntax syntax => syntax.Type.ToString(),
                _ => null
            };

        public override IEnumerable<string> GetUsingNames(SyntaxNode? syntaxNode)
            => GetUsingsInternal(syntaxNode).Select(x => x.Name.ToString());
        public override IEnumerable<SyntaxNode> GetUsings(SyntaxNode? syntaxNode)
            => GetUsingsInternal(syntaxNode);
        private IEnumerable<UsingDirectiveSyntax> GetUsingsInternal(SyntaxNode? syntaxNode)
        {
            if (syntaxNode is null)
                return Enumerable.Empty<UsingDirectiveSyntax>();

            var compilationUnit = syntaxNode.AncestorsAndSelf()
                                            .OfType<CompilationUnitSyntax>()
                                            .FirstOrDefault();
            return compilationUnit.Usings;
        }

        public override IEnumerable<SyntaxNode> GetGenericArguments(SyntaxNode? syntaxNode)
             => syntaxNode switch
             {
                 GenericNameSyntax syntax => syntax.TypeArgumentList.Arguments.ToList(),
                 MemberAccessExpressionSyntax syntax => GetGenericArguments(syntax.Name),
                 InvocationExpressionSyntax syntax => GetGenericArguments(syntax.Expression),
                 _ => Enumerable.Empty<SyntaxNode>()
             };


        public override SyntaxNode? GetReturnType(SyntaxNode? syntaxNode)
             => syntaxNode is MethodDeclarationSyntax methodSyntax
                     ? methodSyntax.ReturnType
                     : null;

        public override IEnumerable<SyntaxNode> MethodArguments(SyntaxNode? syntaxNode)
        {
            return syntaxNode switch
            {
                MemberAccessExpressionSyntax x => ArgumentsOnMethod(x),
                InvocationExpressionSyntax x => ArgumentsOnMethod(x),
                _ => new ArgumentSyntax[] { }
            };
        }
        private IEnumerable<ArgumentSyntax> ArgumentsOnMethod(MemberAccessExpressionSyntax? syntaxNode)
        {
            var invocation = syntaxNode?.Ancestors()
                                       .OfType<InvocationExpressionSyntax>()
                                       .FirstOrDefault();
            return invocation is not null
                ? ArgumentsOnMethod(invocation)
                : new ArgumentSyntax[] { };
        }

        private IEnumerable<ArgumentSyntax> ArgumentsOnMethod(InvocationExpressionSyntax? invocation)
           => invocation is null
                  ? new ArgumentSyntax[] { }
                  : invocation.ArgumentList.Arguments.ToList();

        public override bool IsClass(SyntaxNode? syntaxNode)
                => syntaxNode is ClassDeclarationSyntax;

        public override bool IsMethod(SyntaxNode? syntaxNode)
            => syntaxNode is MethodDeclarationSyntax;

        public override bool IsMethodInvocation(SyntaxNode? syntaxNode)
            => syntaxNode is InvocationExpressionSyntax;

        public override bool IsTypeUsage(SyntaxNode? syntaxNode)
            => syntaxNode is TypeSyntax;
    }
}
