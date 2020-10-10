using FluentAssertions;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;

namespace TestData
{
        public class SingleOptionTestData : BaseTestData
        {
            private const string testName = "SingleStringArg";
            private const string generatedNamespace = "StarFruit2.Tests.TestSampleData.SingleStringArg";
            private const string generatedClassName = testName + "CommandSource";

            // this feels wrong name since it leads to ambiguity between the class and the property
            // it works but probably should have better name
            public virtual CommandDescriptor CommandDescriptor => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };

            public SingleOptionTestData(string originalName, string commandLineName, string SourceCode, Type argType, string? description, object? defaultValue)
                : base(testName)
            {
                GeneratedNamespace = generatedNamespace;
                GeneratedSourceClassName = generatedClassName;

                var commandDescriptor = CommandDescriptor;
                var foo = new ArgumentDescriptor(new ArgTypeInfo(argType), null, originalName, null)
                {
                    Name = originalName,
                    CommandLineName = commandLineName,
                    Description = description,
                    DefaultValue = new DefaultValueDescriptor(defaultValue),
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
                    GeneratedComandSourceNamespace = generatedNamespace,
                    GeneratedCommandSourceClassName = generatedClassName,
                    CommandDescriptor = commandDescriptor,
                };

                CommandDefinitionSourceCode = SourceCode;

                // TODO: figure out appropriate test action for use in dotnet interactive

            }
        }
        public class SingleStringOptionTestData : SingleOptionTestData
        {
            //private const string testName = "SingleStringArg";
            //private const string generatedNamespace = "StarFruit2.Tests.TestSampleData.SingleStringArg";
            //private const string generatedClassName = testName + "CommandSource";
            private const string SourceCode = @"
            var command = new Command(""my-class"", """");
            command.Arguments.Add(GetOption<String>(""str-option"", ""this is a description"", ""this is a default value""));
            return command;";

            public SingleStringOptionTestData()
                : base("StrOption", "str-option", SourceCode, typeof(string), "this is a description", "this is a default value") { }
        }
  
}
