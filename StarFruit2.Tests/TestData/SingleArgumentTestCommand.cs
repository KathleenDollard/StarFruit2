//using StarFruit2.Common;
//using StarFruit2.Common.Descriptors;
//using System.Collections.Generic;

//namespace TestData
//{
//    public class SingleArgumentTestCommand<T> : BaseTestData
//    {
//        public CommandDescriptor CommandDescriptor
//            => new CommandDescriptor(null, "MyClass", null) { Name = "my-class" };

//        public SingleArgumentTestCommand(string testName,
//                                         string originalName,
//                                         string commandLineName,
//                                         string typeStringRepresentation,
//                                         string? description,
//                                         DefaultValueDescriptor? defaultValue)
//            : base(testName)
//        {
//            string sourceCode = $@"
//                var command = new Command(""my-class"", """");
//                command.Arguments.Add(GetArg<{typeStringRepresentation}>(""{commandLineName}"", ""{description}"", {defaultValue.CodeRepresentation}));
//                return command;";

//            GeneratedNamespace = "StarFruit2.Tests.TestSampleData" + testName;
//            GeneratedSourceClassName = testName + "CommandSource";

//            var commandDescriptor = CommandDescriptor;

//            commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>(){
//                new ArgumentDescriptor(new ArgTypeInfo(typeof(T)), null, originalName, null)
//                {
//                    Name = originalName,
//                    CommandLineName = commandLineName,
//                    Description = description,
//                    DefaultValue = defaultValue,
//                }
//            });

//            CliDescriptor = new CliDescriptor
//            {
//                GeneratedComandSourceNamespace = GeneratedNamespace,
//                GeneratedCommandSourceClassName = GeneratedSourceClassName,
//                CommandDescriptor = commandDescriptor,
//            };

//            CommandDefinitionSourceCode = sourceCode;

//            // TODO: figure out appropriate test action for use in dotnet interactive
//            //       it might be an action passed into constructor

//        }
//    }
//    public class SingleStringArgTestData : SingleArgumentTestCommand<string>
//    {
//        // TODO: named params across the board
//        public SingleStringArgTestData()
//            : base(testName: "SingleStringArg",
//                   originalName: "StrArg",
//                   commandLineName: "str-arg",
//                   typeStringRepresentation: "String",
//                   description: "this is a description",
//                   defaultValue: new DefaultValueDescriptor("this is a default value"))
//        { }
//    }

//    public class SingleIntArgTestData : SingleArgumentTestCommand<int>
//    {
//        public SingleIntArgTestData()
//            : base(testName: "SingleIntArg",
//                   originalName: "IntArg",
//                   commandLineName: "int-arg",
//                   typeStringRepresentation: "Int32",
//                   description: "this is a description",
//                   defaultValue: new DefaultValueDescriptor(42))
//        { }
//    }

//    public class SingleBoolArgTestData : SingleArgumentTestCommand<bool>
//    {
//        public SingleBoolArgTestData()
//            : base(testName: "SingleBoolArg",
//                   originalName: "BoolArg",
//                   commandLineName: "bool-arg",
//                   typeStringRepresentation: "Boolean",
//                   description: "this is a description",
//                   defaultValue: new DefaultValueDescriptor(true))
//        { }
//    }
//}
