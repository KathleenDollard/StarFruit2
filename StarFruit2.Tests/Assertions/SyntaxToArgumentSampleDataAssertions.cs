using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common.Descriptors;
using StarFruit2.Tests.SampleData;
using System;

namespace StarFruit2.Tests
{

    public class SyntaxToArgumentSampleDataAssertions : ReferenceTypeAssertions<ArgumentDescriptor, SyntaxToArgumentSampleDataAssertions>
    {
        public SyntaxToArgumentSampleDataAssertions(ArgumentDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "argumentDescriptor2sample";

        public AndConstraint<SyntaxToArgumentSampleDataAssertions> Match(ArgumentExpectedData sampleData)
        {
            using var _ = new AssertionScope();
            Execute.Assertion
                 .ForCondition(sampleData.Name == Subject.Name)
                 .FailWith($"Argument name does not match ({Subject.Name} != {sampleData.Name})")
                 .Then
                 .ForCondition(DoTypesMatch(sampleData.Type, Subject.ArgumentType.TypeRepresentation ))
                 .FailWith($"Argument type does not match ({TypeNameFromTypeRepresentation(Subject.ArgumentType.TypeRepresentation)} != {sampleData.Type})");


            return new AndConstraint<SyntaxToArgumentSampleDataAssertions>(this);

            static bool DoTypesMatch(string type, object typeRep)
            {
                return typeRep switch
                {
                    Type t => t.Name == type,
                    PredefinedTypeSyntax p => p.ToString() == type,
                    IdentifierNameSyntax i => i.Identifier.ToString() == type,
                    _ =>throw new InvalidOperationException()
                };
            }

            static string TypeNameFromTypeRepresentation(object typeRep)
            {
                return typeRep switch
                {
                    Type t => t.FullName,
                    PredefinedTypeSyntax p => p.ToString(),
                    IdentifierNameSyntax i => i.ToString(),
                    _ => throw new InvalidOperationException()
                };
            }
        }

    }
}

