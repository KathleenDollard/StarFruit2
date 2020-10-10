using FluentAssertions;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;

namespace TestData
{
    public class SingleArgTestData : BaseTestData
    {
        private const string testName = "SingleStringArg";
        private const string generatedNamespace = "StarFruit2.Tests.TestSampleData.SingleStringArg";
        private const string generatedClassName = testName + "CommandSource";

        // this feels wrong name since it leads to ambiguity between the class and the property
        // it works but probably should have better name
        public virtual CommandDescriptor CommandDescriptor => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };

        public SingleArgTestData(string originalName, string commandLineName, string SourceCode, Type argType, string? description, object? defaultValue)
            : base(testName)
        {
            GeneratedNamespace = generatedNamespace;
            GeneratedSourceClassName = generatedClassName;

            var commandDescriptor = CommandDescriptor;

            commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>(){
                new ArgumentDescriptor(new ArgTypeInfo(argType), null, originalName, null)
                {
                    Name = originalName,
                    CommandLineName = commandLineName,
                    Description = description,
                    DefaultValue = new DefaultValueDescriptor(defaultValue),
                }
            });

            CliDescriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = generatedNamespace,
                GeneratedCommandSourceClassName = generatedClassName,
                CommandDescriptor = commandDescriptor,
            };

            CommandDefinitionSourceCode = SourceCode;

            // TODO: figure out appropriate test action for use in dotnet interactive

        }
    }
    public class SingleStringArgTestData : SingleArgTestData
    {
        //private const string testName = "SingleStringArg";
        //private const string generatedNamespace = "StarFruit2.Tests.TestSampleData.SingleStringArg";
        //private const string generatedClassName = testName + "CommandSource";
        private const string SourceCode = @"
            var command = new Command(""my-class"", """");
            command.Arguments.Add(GetArg<String>(""str-arg"", ""this is a description"", ""this is a default value""));
            return command;";

        public SingleStringArgTestData()
            : base("StrArg", "str-arg", SourceCode, typeof(string), "this is a description", "this is a default value") { }
    }

    public class SingleIntArgTestData : SingleArgTestData
    {
        private const string SourceCode = @"
            var command = new Command(""my-class"", """");
            command.Arguments.Add(GetArg<Int32>(""int-arg"", ""this is a description"", 42));
            return command;";

        public SingleIntArgTestData()
            : base("IntArg", "int-arg", SourceCode, typeof(int), "this is a description", 42) { }
    }

    public class SingleBoolArgTestData : SingleArgTestData
    {
        private const string SourceCode = @"
            var command = new Command(""my-class"", """");
            command.Arguments.Add(GetArg<Boolean>(""bool-arg"", ""this is a description"", True));
            return command;";

        public SingleBoolArgTestData()
            : base("BoolArg", "bool-arg", SourceCode, typeof(bool), "this is a description", true) { }
    }
}
