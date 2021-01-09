using FluentDom;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace StarFruit2.Generate
{
    public abstract class SyntaxReceiverBase : ISyntaxReceiver
    {
        public List<Using> Usings { get; } = new();
        public List<SyntaxNode> Candidates { get; } = new();

        public abstract void OnVisitSyntaxNode(SyntaxNode syntaxNode);
     
    }
}
