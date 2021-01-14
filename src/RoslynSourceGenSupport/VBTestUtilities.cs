using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace RoslynSourceGenSupport
{
    public class VBTestUtilities : TestUtilitiesBase
    {
        public override SyntaxTree ParseToSyntaxTree(string source)
          => CSharpSyntaxTree.ParseText(source);

        public override IEnumerable<SyntaxToken> TokensWithName(SyntaxNode syntaxNode, string name)
        {
            return syntaxNode.DescendantTokens()
                             .Where(x => x.IsKind(SyntaxKind.IdentifierToken) && x.ToString() == name);

        }

    }
}
