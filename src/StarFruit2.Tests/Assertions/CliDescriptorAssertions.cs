using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{

    public class CliDescriptorAssertions : ReferenceTypeAssertions<CliDescriptor, CliDescriptorAssertions>
    {
        public CliDescriptorAssertions(CliDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "cliDescriptor2sample";

        public AndConstraint<CliDescriptorAssertions> Match(CliDescriptor expected)
        {
            using var _ = new AssertionScope();

            Subject.CommandDescriptor.Should().NotBeNull();
            Execute.Assertion
                 .ForCondition(expected.GeneratedCommandSourceClassName == Subject.GeneratedCommandSourceClassName)
                 .FailWith($"CommandSourceClassName does not match ({Subject.GeneratedCommandSourceClassName} != {expected.GeneratedCommandSourceClassName} )")
                 .Then
                 .ForCondition(expected.GeneratedComandSourceNamespace == Subject.GeneratedComandSourceNamespace)
                 .FailWith($"CommandSourceNamespace does not match ({Subject.GeneratedComandSourceNamespace} != {expected.GeneratedComandSourceNamespace} )");

            Subject.CommandDescriptor.Should().Match(expected.CommandDescriptor);

            return new AndConstraint<CliDescriptorAssertions>(this);
        }

    }
}

