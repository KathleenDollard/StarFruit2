using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace StarFruit.FluentDomSourceGen.Tests
{


    public class RoslynCSharpCompilationAssertions : ReferenceTypeAssertions<Compilation, RoslynCSharpCompilationAssertions>
    {
        public RoslynCSharpCompilationAssertions(Compilation instance)
            : base(instance)
        { }

        protected override string Identifier => "compileroutput";

        public AndConstraint<RoslynCSharpCompilationAssertions> NotHaveErrors()
        {
            Subject.Should().NotBeNull();
            if (Subject is not null)
            {
                var diagnostics = Subject.GetDiagnostics();
                // CS0246 occurs naturally as we test  a single file and its references are not available
                var filtered = diagnostics.Where(x => x.Id != "CS0246");
                filtered.Should().NotHaveErrors();
            }

            return new AndConstraint<RoslynCSharpCompilationAssertions>(this);

        }

    }
}
