using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using CSharpSyntax = Microsoft.CodeAnalysis.CSharp.Syntax;
using VBSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace RoslynSourceGenSupport.Tests
{
    public class SyntaxListNodeAssertions : SelfReferencingCollectionAssertions<IEnumerable<SyntaxNode>, SyntaxNode, SyntaxListNodeAssertions>
    {
        public SyntaxListNodeAssertions(IEnumerable<SyntaxNode> instance)
            : base(instance)
        { }

        protected override string Identifier => "commanddesc2sample";

        public AndConstraint<SyntaxListNodeAssertions> AllBeClasses()
        {
            using var _ = new AssertionScope();
            foreach (var syntaxNode in Subject)
            {
                syntaxNode.Should().BeAClass();
            }
            return new AndConstraint<SyntaxListNodeAssertions>(this);
        }

        public AndConstraint<SyntaxListNodeAssertions> AllBeMethods()
        {
            using var _ = new AssertionScope();
            foreach (var syntaxNode in Subject)
            {
                syntaxNode.Should().BeAMethod();
            }
            return new AndConstraint<SyntaxListNodeAssertions>(this);
        }

        public AndConstraint<SyntaxListNodeAssertions> AllBeMethodInvocations()
        {
            using var _ = new AssertionScope();
            foreach (var syntaxNode in Subject)
            {
                syntaxNode.Should().BeAnInvocation();
            }
            return new AndConstraint<SyntaxListNodeAssertions>(this);
        }

        public AndConstraint<SyntaxListNodeAssertions> AllBeTypeUsage()
        {
            using var _ = new AssertionScope();
            foreach (var syntaxNode in Subject)
            {
                syntaxNode.Should().BeTypeUsage();
            }
            return new AndConstraint<SyntaxListNodeAssertions>(this);
        }

    }
}

