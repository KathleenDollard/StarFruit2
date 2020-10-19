using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Text;
using TestData;

namespace StarFruit2.Tests.TestData
{
    public class RequiredHiddenAliasedTestCommand : BaseTestData
    {
        public CommandDescriptor CommandDescriptor => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };
        private const string testName = "RequiredHiddenAliased";
        private const string optionName = "MyProperty2";
        private const string optionCliName = "my-property2";
        private const string argName = "MyPropertyArg";
        private const string argCliName = "my-property";

        public RequiredHiddenAliasedTestCommand()
            : base(testName)
        {
            string SourceCode = $@"
                var command = new Command(""my-class"", """");
                command.Options.Add(GetOption<int>(""{optionCliName }"", """"));
                command.Options.Add(GetArgument<int>(""{argCliName}"", """"));
                return command;";
            GeneratedNamespace = "StarFruit2.Tests.TestData." + testName;
            GeneratedSourceClassName = testName + "CommandSource";

            var commandDescriptor = CommandDescriptor;
            var option = new OptionDescriptor(commandDescriptor, optionName, null)
            {
                Name = optionName,
                CliName = optionCliName,
            };
            option.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(int)), null, optionName, null)
            {
                Name = optionName,
                CliName = optionCliName,
            });

            commandDescriptor.AddOptions(options:
                new List<OptionDescriptor>()
                {
                     option
                }
            );

            commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>(){
                new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(int)), null, argName, null)
                {
                    Name = argName,
                    CliName = argCliName,
                }
            });
            
            CliDescriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = GeneratedNamespace,
                GeneratedCommandSourceClassName = GeneratedSourceClassName,
                CommandDescriptor = commandDescriptor,
            };

            CommandDefinitionSourceCode = SourceCode;

            // TODO: figure out appropriate test action for use in dotnet interactive

        }
    }
}
