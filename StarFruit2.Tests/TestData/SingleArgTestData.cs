using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System.Collections.Generic;

namespace TestData
{
    public class SingleArgTestData<T> : BaseTestData
    {
        public CommandDescriptor CommandDescriptor
            => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };

        public SingleArgTestData(string testName,
                                         string originalName,
                                         string commandLineName,
                                         string? description,
                                         DefaultValueDescriptor? defaultValue)
            : base(testName)
        {
            string sourceCode = $@"
                var command = new Command(""my-class"", """");
                command.Arguments.Add(GetArg<{typeof(T).ToString()}>(""{commandLineName}"", ""{description}"", {defaultValue.CodeRepresentation}));
                return command;";

            GeneratedNamespace = "StarFruit2.Tests.TestSampleData." + testName;
            GeneratedSourceClassName = testName + "CommandSource";

            var commandDescriptor = CommandDescriptor;

            commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>(){
                new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(T)), null, originalName, null)
                {
                    Name = originalName,
                    CliName = commandLineName,
                    Description = description,
                    DefaultValue = defaultValue,
                }
            });

            CliDescriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = GeneratedNamespace,
                GeneratedCommandSourceClassName = GeneratedSourceClassName,
                CommandDescriptor = commandDescriptor,
            };

            CommandDefinitionSourceCode = sourceCode;

            // TODO: figure out appropriate test action for use in dotnet interactive
            //       it might be an action passed into constructor

        }
    }
    public class SingleArgStringTestData : SingleArgTestData<string>
    {
        // TODO: named params across the board
        public SingleArgStringTestData()
            : base(testName: "SingleArgString",
                   originalName: "MyPropertyArg",
                   commandLineName: "my-property",
                   description: "",
                   defaultValue: new DefaultValueDescriptor("this is a default value"))
        { }
    }

    public class SingleIntArgTestData : SingleArgTestData<int>
    {
        public SingleIntArgTestData()
            : base(testName: "SingleArgInt",
                   originalName: "MyPropertyArg",
                   commandLineName: "my-property",
                   description: "This is an int description",
                   defaultValue: new DefaultValueDescriptor(42))
        { }
    }

    public class SingleBoolArgTestData : SingleArgTestData<bool>
    {
        public SingleBoolArgTestData()
            : base(testName: "SingleArgBool",
                   originalName: "MyPropertyArg",
                   commandLineName: "my-property",
                   description: "This is a bool description",
                   defaultValue: new DefaultValueDescriptor(true))
        { }
    }
}
