using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{

    public class ArgumentDescriptorAssertions : ReferenceTypeAssertions<ArgumentDescriptor, ArgumentDescriptorAssertions>
    {
        public ArgumentDescriptorAssertions(ArgumentDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "argumentDescriptor2sample";

        public AndConstraint<ArgumentDescriptorAssertions> Match(ArgumentDescriptor expected)
        {
            using var _ = new AssertionScope();

            Subject.OriginalName.Should().Be(expected.OriginalName);
            Subject.CliName.Should().Be(expected.CliName);
            Subject.Name.Should().Be(expected.Name);
            Subject.Description.Should().Be(expected.Description);
            Subject.Required.Should().Be(expected.Required);
            Subject.IsHidden.Should().Be(expected.IsHidden);
            //Subject.Aliases.Should().Be(expected.Aliases);
            Subject.ArgumentType.TypeAsString().Should().Be(expected.ArgumentType.TypeAsString());

            return new AndConstraint<ArgumentDescriptorAssertions>(this);
        }
    }
}

