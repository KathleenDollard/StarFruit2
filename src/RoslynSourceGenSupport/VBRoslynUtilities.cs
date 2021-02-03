using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace RoslynSourceGenSupport
{
    public class VBRoslynUtilities : RoslynUtilitiesBase
    {
 
        public override SyntaxTree ParseToSyntaxTree(string source)
          => CSharpSyntaxTree.ParseText(source);

        public override IEnumerable<SyntaxToken> TokensWithName(SyntaxNode syntaxNode, string name)
        {
            return syntaxNode.DescendantTokens()
                             .Where(x => x.IsKind(SyntaxKind.IdentifierToken) && x.ToString() == name);

        }

        public override IEnumerable<SyntaxNode> ClassesWithName(SyntaxNode? syntaxNode, string className)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> MethodsWithName(SyntaxNode? syntaxNode, string className)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> ClassesWithBaseOrInterfaceNamed(SyntaxNode? syntaxNode, string baseOrInterfaceName)
        {
            throw new System.NotImplementedException();
        }

        public override string? TypeName(SyntaxNode syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<string> GetUsingNames(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> GetGenericArguments(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override SyntaxNode? GetReturnType(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsClass(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsMethod(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsMethodInvocation(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsTypeUsage(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> GetUsings(SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<SyntaxNode> MethodArguments( SyntaxNode? syntaxNode)
        {
            throw new System.NotImplementedException();
        }
    }
}
