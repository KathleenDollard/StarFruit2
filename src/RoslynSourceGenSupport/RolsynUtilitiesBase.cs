using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoslynSourceGenSupport
{
    public abstract class RoslynUtilitiesBase
    {
        public static CSharpRoslynUtilities CSharpRoslynUtilities { get; } = new();
        public static VBRoslynUtilities VBRoslynUtilities { get; } = new VBRoslynUtilities();

        public static RoslynUtilitiesBase Pick(bool useVB)
            => useVB
                    ? VBRoslynUtilities
                    : CSharpRoslynUtilities;


        public abstract SyntaxTree ParseToSyntaxTree(string source);

        public abstract IEnumerable<SyntaxToken> TokensWithName(SyntaxNode syntaxNode, string name);

        public abstract IEnumerable<SyntaxNode> ClassesWithName(SyntaxNode? syntaxNode, string className);

        public abstract IEnumerable<SyntaxNode> MethodsWithName(SyntaxNode? syntaxNode, string className);

        public abstract IEnumerable<SyntaxNode> ClassesWithBaseOrInterfaceNamed(SyntaxNode? syntaxNode, string baseOrInterfaceName);

        public abstract string? TypeName(SyntaxNode syntaxNode);

        public abstract IEnumerable<string> GetUsingNames(SyntaxNode? syntaxNode);

        public abstract IEnumerable<SyntaxNode> GetUsings(SyntaxNode? syntaxNode);

        public abstract IEnumerable<SyntaxNode> GetGenericArguments(SyntaxNode? syntaxNode);

        public abstract SyntaxNode? GetReturnType(SyntaxNode? syntaxNode);

        public abstract IEnumerable<SyntaxNode> MethodArguments( SyntaxNode? syntaxNode);

    public abstract bool IsClass(SyntaxNode? syntaxNode);
        public abstract bool IsMethod(SyntaxNode? syntaxNode);
        public abstract bool IsMethodInvocation(SyntaxNode? syntaxNode);
        public abstract bool IsTypeUsage(SyntaxNode? syntaxNode);


    }
}
