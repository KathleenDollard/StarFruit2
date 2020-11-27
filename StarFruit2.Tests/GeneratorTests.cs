using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using TestData;
using Xunit;

namespace StarFruit2.Tests
{
   public  class GeneratorTests
    {
        private const string usings = "using System.CommandLine;";
        private const string nspace = "namespace StarFruit2.Tests.TestSampleData.Empty\n{";
        private const string cls = "public class EmptyCommandSource\n{";
        private const string method = "public Command GetCommand()\n{";
        private const string commandCode = @"var command = new Command(""my-class"", """");
            return command;";
        private CliDescriptor cliDescriptor;

        public GeneratorTests()
           => cliDescriptor = new EmptyTestData().CliDescriptor;

        [Fact(Skip = "Old generator")]
        public void Include_usings_outputs_only_using()
        {
            var expected = usings;
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.Usings));

            actual.Should().Be(Utils.Normalize(expected));
        }

        [Fact]
        public void Include_namespace_outputs_only_namespace()
        {
            var expected = nspace + "\n}";
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.Namespace ));

            actual.Should().Be(Utils.Normalize(expected));
        }

        [Fact]
        public void Include_class_outputs_only_class()
        {
            var expected = cls + "\n}";
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.Class));

            actual.Should().Be(Utils.Normalize(expected));
        }

        [Fact]
        public void Include_method_outputs_only_method()
        {
            var expected = method + "\n}";
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.Method));

            actual.Should().Be(Utils.Normalize(expected));
        }


        [Fact(Skip = "Old generator")]
        public void Include_commandCode_outputs_only_command_code()
        {
            var expected = commandCode;
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.CommandCode));

            actual.Should().Be(Utils.Normalize(expected));
        }

        [Fact(Skip ="Old generator")]
        public void Include_all_outputs_all()
        {
            var expected = $@"{nspace}
{cls}
{method}
{commandCode}
}}
}}
}}";
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.All));

            actual.Should().Be(Utils.Normalize(expected));
        }
    }
}
