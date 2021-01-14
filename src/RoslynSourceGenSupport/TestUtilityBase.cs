using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoslynSourceGenSupport
{
    public abstract class TestUtilitiesBase
    {
        public abstract SyntaxTree ParseToSyntaxTree(string source);

        public abstract IEnumerable<SyntaxToken> TokensWithName(SyntaxNode syntaxNode, string name);

        public virtual IEnumerable<SyntaxToken> TokensWithName(SyntaxTree syntaxTree, string name)
            => TokensWithName(syntaxTree.GetRoot(), name);
    }
}
