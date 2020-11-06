using FluentAssertions;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;

namespace TestData
{
    public class SingleOptionTestCommand<T> : BaseTestData
    {
        public CommandDescriptor CommandDescriptor => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };

        public SingleOptionTestCommand(string testName,
                                       string originalName,
                                       string commandLineName,
                                       string typeStringRepresentation,
                                       string? description,
                                       DefaultValueDescriptor? defaultValue)
            : base(testName)
        {

            string SourceCode = $@"
                var command = new Command(""my-class"", """");
                command.Options.Add(GetOption<{typeStringRepresentation}>(""{commandLineName}"", ""{description}"", {defaultValue.CodeRepresentation}));
                return command;";
            GeneratedNamespace = "StarFruit2.Tests.TestSampleData" + testName;
            GeneratedSourceClassName = testName + "CommandSource";

            var commandDescriptor = CommandDescriptor;

            var option = new OptionDescriptor(commandDescriptor, originalName, null)
            {
                Name = originalName,
                CliName = commandLineName,
                Description = description,
            };
            option.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(T)), null, originalName, null)
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
    public class SingleStringOptionTestData : SingleOptionTestCommand<string>
    {
        public SingleStringOptionTestData()
            : base(testName: "SingleStringOpt",
                   originalName: "StrOption",
                   commandLineName: "str-option",
                   typeStringRepresentation: "String",
                   description: "this is a description",
                   defaultValue: new DefaultValueDescriptor("this is a default value"))
        { }
    }

    public class SingleIntOptionTestData : SingleOptionTestCommand<int>
    {
        public SingleIntOptionTestData()
            : base(testName: "SingleIntOpt",
                   originalName: "IntOption",
                   commandLineName: "int-option",
                   typeStringRepresentation: "Int32",
                   description: "this is a description",
                   defaultValue: new DefaultValueDescriptor(42))
        { }
    }

    public class SingleBoolOptionTestData : SingleOptionTestCommand<bool>
    {
        public SingleBoolOptionTestData()
            : base(testName: "SingleBoolOpt",
                   originalName: "BoolOption",
                   commandLineName: "bool-option",
                   typeStringRepresentation: "Boolean",
                   description: "this is a description",
                   defaultValue: new DefaultValueDescriptor(true))
        { }
    }
}