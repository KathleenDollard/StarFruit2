using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace StarFruit.FluentDomSourceGen.Tests
{

    public class RoslynGenerationDiagnosticAssertions : ReferenceTypeAssertions<IEnumerable <Diagnostic>, RoslynGenerationDiagnosticAssertions>
    {
        public RoslynGenerationDiagnosticAssertions(IEnumerable<Diagnostic> instance)
            : base(instance)
        { }

        protected override string Identifier => "roslyndiagnostics";

        public AndConstraint<RoslynGenerationDiagnosticAssertions> NotHaveErrors(string compilationId)
        {

            using var _ = new AssertionScope();
            var diagnostics = Subject;

            foreach (var diagnostic in diagnostics.Where(x => x.Severity.HasFlag(DiagnosticSeverity.Error)))
            {
                Execute.Assertion
                     .ForCondition(!diagnostic.Severity.HasFlag(DiagnosticSeverity.Warning))
                     .FailWith(@$"
Compilation issue ({compilationId}): 
   {diagnostic}");

            }
            return new AndConstraint<RoslynGenerationDiagnosticAssertions>(this);

     

        }

    }
}
