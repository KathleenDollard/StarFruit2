using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using StarFruit2.Tests;
using System.Collections.Generic;

namespace TestData
{
    public class SeveralOptionsAndArgumentsTestData : BaseTestData
    {

        public SeveralOptionsAndArgumentsTestData()
        //                                string originalName,
        //                                string commandLineName,
        //                                string typeStringRepresentation,
        //                                string? description,
        //                                DefaultValueDescriptor? defaultValue)
           : base("SeveralOptionsAndArguments")
        {
            string sourceCode = $@"NOT YET IMPLEMENTED: Read corresponding file";

            var commandDescriptor = new CommandDescriptor(null, "MyClass", RawInfo.DummyClass) { Name = "my-class" };

            commandDescriptor.AddArguments(arguments: new List<ArgumentDescriptor>(){
                new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), null, "MyArgPropertyArg", RawInfo.DummyProperty)
                {
                    Name = "MyArgPropertyArg",
                    CliName = "my-property-arg",
                },
            });

            commandDescriptor.AddOptions(options: new List<OptionDescriptor>(){
                new OptionDescriptor(null, "MyProperty", RawInfo.DummyProperty)
                {
                    Name = "MyProperty",
                    CliName = "my-property",
                },
                new OptionDescriptor(null, "MyProperty2", RawInfo.DummyProperty)
                {
                    Name = "MyProperty2",
                    CliName = "my-property2",
                },
                new OptionDescriptor( null, "MyProperty3",RawInfo.DummyProperty)
                {
                    Name = "MyProperty3",
                    CliName = "my-property2",
                },
            });

            CliDescriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = "StarFruit2.Tests.TestSampleData.SeveralOptionsAndArguments",
                GeneratedCommandSourceClassName = "SeveralOptionsAndArgumentsCommandSource",
                CommandDescriptor = commandDescriptor,
            };

            CommandDefinitionSourceCode = sourceCode;

            // TODO: figure out appropriate test action for use in dotnet interactive
            //       it might be an action passed into constructor

        }
    }
}
