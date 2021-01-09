using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public static class AssertionExtensions
    {
        public static RoslynCompilationAssertions Should(this Compilation instance)
           => new RoslynCompilationAssertions(instance);

        public static RoslynGenerationDiagnosticAssertions Should(this IEnumerable<Diagnostic> diagnostics)
          => new RoslynGenerationDiagnosticAssertions(diagnostics);
    }
}
