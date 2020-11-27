using FluentAssertions;
using FluentDom.Generator;
using GeneratorSupport;
using StarFruit2.Common.Descriptors;
using System;
using Xunit;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class CommandSourceGenTests
    {
        [Fact]
        public void Simple_command()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            var expected = $@"";

            var template = new GenerateCommandSource();
            var code = template.CreateCode(descriptor);
            var generator = new GeneratorBase();
            var actual = generator.Generate(code);

            actual.Should().Be(expected);
        }
    }
}
