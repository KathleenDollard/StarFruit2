using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common.Descriptors;
using StarFruit2.Common;
using StarFruit2.Tests.SampleData;
using System;

namespace StarFruit2.Tests
{

    public class SourceToDescriptoArgumentAssertions : ReferenceTypeAssertions<ArgumentDescriptor, SourceToDescriptoArgumentAssertions>
    {
        public SourceToDescriptoArgumentAssertions(ArgumentDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "argumentDescriptor2sample";

        public AndConstraint<SourceToDescriptoArgumentAssertions> Match(ArgumentDescriptor expected)
        {
            using var _ = new AssertionScope();
            Execute.Assertion
                 .ForCondition(expected.Name == Subject.Name)
                 .FailWith($"Argument name does not match ({Subject.Name} != {expected.Name})")
                 .Then
                 .ForCondition(DoTypesMatch(expected.ArgumentType, Subject.ArgumentType))
                 .FailWith($"Argument type does not match ({GetTypeName(Subject.ArgumentType)} != {expected.ArgumentType })");


            return new AndConstraint<SourceToDescriptoArgumentAssertions>(this);

            static bool DoTypesMatch(ArgTypeInfo expected, ArgTypeInfo actual)
            {
                var actualTypeName = GetTypeName(actual);
                var expectedTypeName = GetTypeName(expected);
                return actualTypeName == expectedTypeName;


            }

            static string GetTypeName(ArgTypeInfo actual)
            {
                return actual.TypeRepresentation switch
                {
                    Type t => t.Name,
                    PredefinedTypeSyntax p => p.ToString(),
                    IdentifierNameSyntax i => i.Identifier.ToString(),
                    _ => throw new InvalidOperationException()
                };

            }

        }
    }
}

