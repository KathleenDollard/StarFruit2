using FluentAssertions;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
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
                                 string typeStringRepresentation,
                                 string? description,
                                 DefaultValueDescriptor? defaultValue)
            : base(testName)
        {
            string sourceCode = $@"
                var command = new Command(""my-class"", """");
                command.Arguments.Add(GetArg<{typeStringRepresentation}>(""{commandLineName}"", ""{description}"", {defaultValue.CodeRepresentation}));
                return command;
            ";

            GeneratedNamespace = "StarFruit2.Tests.TestSampleData" + testName;
            GeneratedSourceClassName = testName + "CommandSource";

            var commandDescriptor = CommandDescriptor;

            commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>(){
                new ArgumentDescriptor(new ArgTypeInfo(typeof(T)), null, originalName, null)
                {
                    Name = originalName,
                    CommandLineName = commandLineName,
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
    public class SingleStringArgTestData : SingleArgTestData<string>
    {
        // TODO: named params across the board
        public SingleStringArgTestData()
            : base(testName: "SingleStringArg",
                   "StrArg",
                   "str-arg",
                   "String",
                   "this is a description",
                   new DefaultValueDescriptor("this is a default value"))
        { }
    }

    public class SingleIntArgTestData : SingleArgTestData<int>
    {
        private const string testName = "SingleIntArg";

        public SingleIntArgTestData()
            : base(testName,
                   "IntArg",
                   "int-arg",
                   "Int32",
                   "this is a description",
                   new DefaultValueDescriptor(42))
        { }
    }

    public class SingleBoolArgTestData : SingleArgTestData<bool>
    {
        private const string testName = "SingleBoolArg";
        //private const string generatedNamespace = "StarFruit2.Tests.TestSampleData.SingleBoolArg";
        //private const string generatedClassName = testName + "CommandSource";
     
        public SingleBoolArgTestData()
            : base(testName,
                   "BoolArg",
                   "bool-arg",
                   "Boolean",
                   "this is a description",
                   new DefaultValueDescriptor(true))
        { }
    }
}
