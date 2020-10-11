using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using StarFruit2.Common.Descriptors;
using StarFruit2.Tests.SampleData;
using System.Linq;

namespace StarFruit2.Tests
{

    public class OptionDescriptorAssertions : ReferenceTypeAssertions<OptionDescriptor, OptionDescriptorAssertions>
    {
        public OptionDescriptorAssertions(OptionDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "commanddesc2sample";

        public AndConstraint<OptionDescriptorAssertions> Match(OptionDescriptor expected)
        {
            var sampleArguments = expected.Arguments.ToArray();
            var commandArguments = Subject.Arguments.ToArray();

            using var _ = new AssertionScope();
            Execute.Assertion
                 .ForCondition(expected.Name == Subject.Name)
                 .FailWith($"Option name does not match ({Subject.Name} !+ {expected.Name})")
                 .Then
                 .ForCondition(sampleArguments.Length == commandArguments.Length)
                 .FailWith("Option does not contain same number of arguments");
            
            for (int i = 0; i < sampleArguments.Length; i++)
            {
                commandArguments[i].Should().Match(sampleArguments[i]);
            }


            return new AndConstraint<OptionDescriptorAssertions>(this);

        }

    }
}

