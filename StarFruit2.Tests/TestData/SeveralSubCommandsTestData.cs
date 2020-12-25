using StarFruit.Common;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestData;

namespace TestData
{
    public class SeveralSubCommandsTestData : BaseTestData
    {

        public SeveralSubCommandsTestData()
   
           : base("SeveralSubCommands")
        {
            string sourceCode = $@"NOT YET IMPLEMENTED: Read corresponding file";

            var commandDescriptor = new CommandDescriptor(null, "MyClass", null, OriginalElementType.Class) { Name = "my-class" };

            commandDescriptor.AddCommands(subCommands: new List<CommandDescriptor>(){
                new CommandDescriptor( null, "MyMethod2", null, OriginalElementType.Property)
                {
                    Name = "MyMethod2",
                    CliName = "my-mymethod2",
                },
                new CommandDescriptor( null, "MyMethod1", null, OriginalElementType.Property)
                {
                    Name = "MyMethod1",
                    CliName = "my-mymethod1",
                },
                new CommandDescriptor( null, "MyMethod3", null, OriginalElementType.Property)
                {
                    Name = "MyMethod3",
                    CliName = "my-mymethod3",
                },
            });

            commandDescriptor.SubCommands.First().AddArguments(arguments: new List<ArgumentDescriptor>(){
                new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(int)), null, "myParam", null, OriginalElementType.MethodParameter )
                {
                    Name = "myParam",
                    CliName = "my-param",
                },
            });

            CliDescriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = "StarFruit2.Tests.TestSampleData.SeveralSubCommands",
                GeneratedCommandSourceClassName = "SeveralSubCommandsCommandSource",
                CommandDescriptor = commandDescriptor,
            };

            CommandDefinitionSourceCode = sourceCode;

            // TODO: figure out appropriate test action for use in dotnet interactive
            //       it might be an action passed into constructor

        }
    }
}
