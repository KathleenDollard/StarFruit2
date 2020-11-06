using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit2.Common;

namespace StarFruit2.Tests
{
    public class DummyTest
    {
        private CliDescriptor GetCliDescriptor()
        {
            var commandDescriptor = new CommandDescriptor(null, "MyClass", null)
            {
                Name = "my-class",
               // Parent = null,
            };
           // commandDescriptor.Root = commandDescriptor;
            var testName = "testName";
            var GeneratedNamespace = "StarFruit2.Tests.TestSampleData" + testName;
            var GeneratedSourceClassName = testName + "CommandSource";
            string originalName = "SomeOption";
            string commandLineName = "some-option";
            string description = "desc";
            var defaultValue = new DefaultValueDescriptor("abc");

            var option = new OptionDescriptor(commandDescriptor, originalName, null)
            {
                Name = originalName,
                CliName = commandLineName,
                Description = description,
            };
            option.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), null, originalName, null)
            {
                Name = originalName,
                CliName = commandLineName,
                Description = description,
                DefaultValue = defaultValue,
            });

            commandDescriptor.AddOptions(options:
                new List<OptionDescriptor>()
                {
                     option
                }
            );

            return new CliDescriptor
            {
                GeneratedComandSourceNamespace = GeneratedNamespace,
                GeneratedCommandSourceClassName = GeneratedSourceClassName,
                CommandDescriptor = commandDescriptor,
            };
        }
        [Fact]
        public void TestGenerateOutput()
        {
            var cliDescriptor = GetCliDescriptor();
            var generate = new Generate();
            var codeGen = new CodeGeneratorTemp(generate);

            var output = string.Join("\n", codeGen.GenerateSourceCode(cliDescriptor));
        }
    }
}
