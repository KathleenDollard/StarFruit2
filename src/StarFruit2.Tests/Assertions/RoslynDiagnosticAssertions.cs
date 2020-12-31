using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2.Tests
{
    public static class RoslynExtensions
    {
        public static RoslynDiagnosticAssertions Should(this IEnumerable<Diagnostic> instance)
            => new RoslynDiagnosticAssertions(instance);
    }

    public class RoslynDiagnosticAssertions : ReferenceTypeAssertions<IEnumerable<Diagnostic>, RoslynDiagnosticAssertions>
    {
        public RoslynDiagnosticAssertions(IEnumerable<Diagnostic> instance)
            : base(instance)
        { }

        protected override string Identifier => "commanddesc2sample";

        public AndConstraint<RoslynDiagnosticAssertions> NotHaveWarningsOrErrors()
        {

            using var _ = new AssertionScope();
            foreach (var diagnostic in Subject.Where(x => x.Severity.HasFlag(DiagnosticSeverity.Warning) || x.Severity.HasFlag(DiagnosticSeverity.Error)))
            {
                Execute.Assertion
                     .ForCondition(!diagnostic.Severity.HasFlag(DiagnosticSeverity.Warning))
                     .FailWith($"Compilation issue: {diagnostic}");

            }
            return new AndConstraint<RoslynDiagnosticAssertions>(this);

        }

    }
}

