using FluentAssertions;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;

namespace TestData
{
    public class SingleOptionTestData<T> : BaseTestData
    {
        public CommandDescriptor CommandDescriptor => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };

        public SingleOptionTestData(string testName,
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
            var foo = new ArgumentDescriptor(new ArgTypeInfo(typeof(T)), null, originalName, null)
            {
                Name = originalName,
                CommandLineName = commandLineName,
                Description = description,
                DefaultValue = defaultValue,
            };

            commandDescriptor.AddOptions(options:
                new List<OptionDescriptor>()
                {
                        new OptionDescriptor(foo, originalName, null)
                        {
                            Name = originalName,
                            CommandLineName = commandLineName,
                            Description = description,
                            Arguments = new List<ArgumentDescriptor>() { foo }
                        }
                }
            );

            //commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>()
            //{
            //    new ArgumentDescriptor(new ArgTypeInfo(argType), null, originalName, null)
            //    {
            //        Name = originalName,
            //        CommandLineName = commandLineName,
            //        Description = description,
            //        DefaultValue = new DefaultValueDescriptor(defaultValue),
            //    }
            //});

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
    public class SingleStringOptionTestData : SingleOptionTestData<string>
    {
        private const string testName = "SingleStringOpt";

        public SingleStringOptionTestData()
            : base(testName,
                   "StrOption",
                   "str-option",
                   "String",
                   "this is a description",
                   new DefaultValueDescriptor("this is a default value"))
        { }
    }

    public class SingleIntOptionTestData : SingleOptionTestData<int>
    {
        private const string testName = "SingleIntOpt";

        public SingleIntOptionTestData()
            : base(testName,
                   "IntOption",
                   "int-option",
                   "Int32",
                   "this is a description",
                   new DefaultValueDescriptor(42))
        { }
    }

    public class SingleBoolOptionTestData : SingleOptionTestData<bool>
    {
        private const string testName = "SingleBoolOpt";

        public SingleBoolOptionTestData()
            : base(testName,
                   "BoolOption",
                   "bool-option",
                   "Boolean",
                   "this is a description",
                   new DefaultValueDescriptor(true))
        { }
    }

}
