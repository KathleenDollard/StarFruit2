using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class RoslynCompilationAssertions : ReferenceTypeAssertions<Compilation, RoslynCompilationAssertions>
    {
        public RoslynCompilationAssertions(Compilation instance)
            : base(instance)
        { }

        protected override string Identifier => "compileroutput";

        public AndConstraint<RoslynCompilationAssertions> NotHaveErrors(string compilationId)
        {
            Subject.Should().NotBeNull();
            if (Subject is not null)
            {
                var diagnostics = Subject.GetDiagnostics();
                // CS0246 occurs naturally as we test  a single file and its references are not available
                var filtered = diagnostics.Where(x => x.Id != "CS0246");
                filtered.Should().NotHaveErrors(compilationId);
            }

            return new AndConstraint<RoslynCompilationAssertions>(this);

        }

    }
}
