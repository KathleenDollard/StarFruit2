using Microsoft.CodeAnalysis;

// From RoslynSource
namespace StarFruit.FluentDomSourceGen.Tests
{

    internal sealed class GeneratorSyntaxWalker : SyntaxWalker
    {
        private readonly ISyntaxReceiver _syntaxReceiver;

        internal GeneratorSyntaxWalker(ISyntaxReceiver syntaxReceiver)
        {
            _syntaxReceiver = syntaxReceiver;
        }

        public override void Visit(SyntaxNode node)
        {
            _syntaxReceiver.OnVisitSyntaxNode(node);
            base.Visit(node);
        }
    }
}
