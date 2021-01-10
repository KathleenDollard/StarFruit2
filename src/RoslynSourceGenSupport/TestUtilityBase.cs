using Microsoft.CodeAnalysis;

namespace RoslynSourceGenSupport
{
    public abstract class TestUtilitiesBase
    {
        public abstract SyntaxTree ParseToSyntaxTree(string source);
    }
}
