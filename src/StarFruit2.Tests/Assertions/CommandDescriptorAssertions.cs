using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{

    public class CommandDescriptorAssertions : ReferenceTypeAssertions<CommandDescriptor, CommandDescriptorAssertions>
    {
        public CommandDescriptorAssertions(CommandDescriptor instance)
            : base(instance)
        { }

        protected override string Identifier => "commandDescriptor2sample";

        public AndConstraint<CommandDescriptorAssertions> Match(CommandDescriptor expected)
        {
            var sampleOptions = expected.Options.ToArray();
            var commandOptions = Subject.Options.ToArray();
            var sampleSubCommands = expected.SubCommands.ToArray();
            var commandSubCommands = Subject.SubCommands.ToArray();
            var sampleArguments = expected.Arguments.ToArray();
            var commandArguments = Subject.Arguments.ToArray();

            using var _ = new AssertionScope();

            Execute.Assertion
                 .ForCondition(expected.Name == Subject.Name)
                 .FailWith($"Command name does not match ({Subject.Name}!= {expected.Name} )")
                 .Then
                 .ForCondition(sampleOptions.Length == commandOptions.Length)
                 .FailWith($"Command does not contain same number of options ({commandOptions.Length} vs {sampleOptions.Length}")
                 .Then
                 .ForCondition(sampleArguments.Length == commandArguments.Length)
                 .FailWith($"Command does not contain same number of arguments ({sampleArguments.Length} vs {commandArguments.Length}")
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

            return new AndConstraint<CommandDescriptorAssertions>(this);

        }

    }
}

