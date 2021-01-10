using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynSourceGenSupport
{
    public class CSharpTestUtilities : TestUtilitiesBase
    {
        public override SyntaxTree ParseToSyntaxTree(string source)
          => CSharpSyntaxTree.ParseText(source);
    }
}
