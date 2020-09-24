using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using StarFruit2.Common.Descriptors;
using StarFruit2.Tests.SampleData;
using System.Linq;

namespace StarFruit2.Tests
{

    public class SyntaxToCommandSampleDataAssertions : ReferenceTypeAssertions<CommandDescriptor, SyntaxToCommandSampleDataAssertions>
    {
        public SyntaxToCommandSampleDataAssertions(CommandDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "commandDescriptor2sample";

        public AndConstraint<SyntaxToCommandSampleDataAssertions> Match(CommandExpectedData expectedData)
        {
            var sampleOptions = expectedData.Options.ToArray();
            var commandOptions = Subject.Options.ToArray();
            var sampleSubCommands = expectedData.SubCommands.ToArray();
            var commandSubCommands = Subject.SubCommands.ToArray();
            var sampleArguments = expectedData.Arguments.ToArray();
            var commandArguments = Subject.Arguments.ToArray();

            using var _ = new AssertionScope();

            Execute.Assertion
                 .ForCondition(expectedData.Name == Subject.Name)
                 .FailWith($"Command name does not match ({expectedData.Name} != {Subject.Name})")
                 .Then
                 .ForCondition(sampleOptions.Length == commandOptions.Length)
                 .FailWith($"Command does not contain same number of options ({commandOptions.Length} vs {sampleOptions.Length}")
                 .Then
                 .ForCondition(sampleSubCommands.Length == commandSubCommands.Length)
                 .FailWith("Command does not contain same number of sub-commands");

            for (int i = 0; i < sampleArguments.Length; i++)
            {
                commandArguments[i].Should().Match(sampleArguments[i]);
            }

            for (int i = 0; i < sampleOptions.Length; i++)
            {
                commandOptions[i].Should().Match(sampleOptions[i]);
            }

            for (int i = 0; i < sampleSubCommands.Length; i++)
            {
                commandSubCommands[i].Should().Match(sampleSubCommands[i]);
            }

            return new AndConstraint<SyntaxToCommandSampleDataAssertions>(this);

        }

    }
}

