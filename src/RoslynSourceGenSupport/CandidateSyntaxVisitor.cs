using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoslynSourceGenSupport
{
    public abstract class CandidateSyntaxVisitor : ISyntaxReceiver
    {
        public List<string> Usings { get; } = new();
        public List<SyntaxNode> Candidates { get; } = new();

        public abstract void OnVisitSyntaxNode(SyntaxNode syntaxNode);

    }
}