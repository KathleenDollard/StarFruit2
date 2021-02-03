using Microsoft.CodeAnalysis;

namespace RoslynSourceGenSupport.Tests
{
    public static  class RoslynSourceGenExtensions
    {
        public static SyntaxTree CheckedTree(this SyntaxTree syntaxTree)
        {
            syntaxTree.GetDiagnostics().Should().NotHaveErrors();
            return syntaxTree;
        }
    }
}
