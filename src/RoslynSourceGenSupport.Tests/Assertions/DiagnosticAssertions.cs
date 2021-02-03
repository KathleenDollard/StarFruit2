using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RoslynSourceGenSupport.Tests
{
    public class DiagnosticAssertions : ReferenceTypeAssertions<IEnumerable<Diagnostic>, DiagnosticAssertions>
    {
        public DiagnosticAssertions(IEnumerable<Diagnostic> instance)
            : base(instance)
        { }

        protected override string Identifier => "commanddesc2sample";

        public AndConstraint<DiagnosticAssertions> NotHaveWarningsOrErrors()
        {
            ReportDiagnosticIssues(Subject,DiagnosticSeverity.Warning, DiagnosticSeverity.Error );
            return new AndConstraint<DiagnosticAssertions>(this);
        }

        public AndConstraint<DiagnosticAssertions> NotHaveErrors()
        {
            ReportDiagnosticIssues(Subject, DiagnosticSeverity.Error);
            return new AndConstraint<DiagnosticAssertions>(this);
        }

        private static void ReportDiagnosticIssues(IEnumerable<Diagnostic> diagnostics, params DiagnosticSeverity[] severityFlags)
        {
            using var _ = new AssertionScope();
            foreach (var diagnostic in diagnostics.Where(x => MatchesAnyFlag(x, severityFlags)))
            {
                Execute.Assertion
                     .ForCondition(!diagnostic.Severity.HasFlag(DiagnosticSeverity.Warning))
                     .FailWith($"Compilation issue: {Escape(diagnostic.ToString())}");

            }

            static string Escape(string input)
            {
                return input.Replace("{", "{{");
            }

            static bool MatchesAnyFlag(Diagnostic diagnostic, params DiagnosticSeverity[] severityFlags)
                => severityFlags.Any(x => diagnostic.Severity.HasFlag(x));
        }
    }
}

