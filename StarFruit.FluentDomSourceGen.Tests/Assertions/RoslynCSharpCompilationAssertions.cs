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

            var diagnostics = Subject.GetDiagnostics();
            var filtered = diagnostics.Where(x => x.Id != "CS0246");
            filtered.Should().NotHaveErrors();

            return new AndConstraint<RoslynCSharpCompilationAssertions>(this);

        }

    }
}
