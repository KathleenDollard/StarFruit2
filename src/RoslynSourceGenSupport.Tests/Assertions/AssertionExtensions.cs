using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoslynSourceGenSupport.Tests
{
    public static class AssertionExtensions
    {
        public static DiagnosticAssertions Should(this IEnumerable<Diagnostic> instance)
            => new(instance);

        public static SyntaxNodeAssertions Should(this SyntaxNode instance)
            => new(instance);

        public static SyntaxListNodeAssertions Should(this IEnumerable<SyntaxNode> instance)
            => new(instance);
    }
}
