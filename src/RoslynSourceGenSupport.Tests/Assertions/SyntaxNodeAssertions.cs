using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using CSharpSyntax = Microsoft.CodeAnalysis.CSharp.Syntax;
using VBSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace RoslynSourceGenSupport.Tests
{
    public class SyntaxNodeAssertions : ReferenceTypeAssertions<SyntaxNode, SyntaxNodeAssertions>
    {
        public SyntaxNodeAssertions(SyntaxNode instance)
            : base(instance)
        { }

        protected override string Identifier => "commanddesc2sample";

        public AndConstraint<SyntaxNodeAssertions> BeAClass()
        {
            if (Subject is not CSharpSyntax.ClassDeclarationSyntax &&
                Subject is not VBSyntax.ClassBlockSyntax)
            {

                Execute.Assertion
                     .FailWith($"SyntaxNode is not a class.");
            }
            return new AndConstraint<SyntaxNodeAssertions>(this);
        }

        public AndConstraint<SyntaxNodeAssertions> BeAMethod()
        {
            if (Subject is not CSharpSyntax.MethodDeclarationSyntax &&
                Subject is not VBSyntax.MethodBlockSyntax)
            {

                Execute.Assertion
                     .FailWith($"SyntaxNode is not a method.");
            }
            return new AndConstraint<SyntaxNodeAssertions>(this);
        }

        public AndConstraint<SyntaxNodeAssertions> BeAnInvocation()
        {
            if (Subject is not CSharpSyntax.InvocationExpressionSyntax &&
                Subject is not VBSyntax.InvocationExpressionSyntax)
            {

                Execute.Assertion
                     .FailWith($"SyntaxNode is not a method invocation.");
            }
            return new AndConstraint<SyntaxNodeAssertions>(this);
        }

        public AndConstraint<SyntaxNodeAssertions> BeTypeUsage()
        {
            if (Subject is not CSharpSyntax.TypeSyntax &&
                Subject is not VBSyntax.TypeSyntax)
            {

                Execute.Assertion
                     .FailWith($"SyntaxNode is not a type usage.");
            }
            return new AndConstraint<SyntaxNodeAssertions>(this);
        }

    }
}

