using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace StarFruit.FluentDomSourceGen.Tests
{
   public static class AssertionExtensions
    {
        public static RoslynCSharpCompilationAssertions Should(this Compilation instance)
           => new RoslynCSharpCompilationAssertions(instance);

        //public static RoslynGenerationDiagnosticAssertions Should(this ImmutableArray<Diagnostic> diagnostics)
        //  => new RoslynGenerationDiagnosticAssertions(diagnostics);

        public static RoslynGenerationDiagnosticAssertions Should(this IEnumerable<Diagnostic> diagnostics)
          => new RoslynGenerationDiagnosticAssertions(diagnostics);
    }
}
