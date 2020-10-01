using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using StarFruit2.Common.Descriptors;
using StarFruit2.Tests.SampleData;
using System.Linq;

namespace StarFruit2.Tests
{

    public class SourceToDescriptorCliAssertions : ReferenceTypeAssertions<CliDescriptor, SourceToDescriptorCliAssertions>
    {
        public SourceToDescriptorCliAssertions(CliDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "cliDescriptor2sample";

        public AndConstraint<SourceToDescriptorCliAssertions> Match(CliDescriptor expected)
        {
            using var _ = new AssertionScope();

            Execute.Assertion
                 .ForCondition(expected.GeneratedCommandSourceClassName == Subject.GeneratedCommandSourceClassName)
                 .FailWith($"Command source class name does not match ({Subject.GeneratedCommandSourceClassName} != {expected.GeneratedCommandSourceClassName} )")
                 .Then
                 .ForCondition(expected.GeneratedComandSourceNamespace == Subject.GeneratedComandSourceNamespace)
                 .FailWith($"Command source class namespace name does not match ({Subject.GeneratedComandSourceNamespace} != {expected.GeneratedComandSourceNamespace} )");

            return new AndConstraint<SourceToDescriptorCliAssertions>(this);
        }

    }
}

