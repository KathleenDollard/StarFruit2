using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.CommandLine;

namespace TestData
{
    public class BaseTestData
    {
        public BaseTestData()
        { }

        public BaseTestData(string testName)
        {
            TestName = testName;
            ModelCodeFileName = $"{testName}.Code.cs";
            SystemCommandLineName = "my-class";

            CompiledClassName = testName;
        }

        public string TestName { get; set; }
        public string GeneratedNamespace { get; set; }
        public string GeneratedSourceClassName { get; set; }
        public string ModelCodeFileName { get; set; }
        public string SystemCommandLineName { get; set; }
        public CliDescriptor CliDescriptor { get; set; }
        public IEnumerable<ArgumentDescriptor> ArgumentDescriptors { get; set; } = new List<ArgumentDescriptor>();
        public IEnumerable<OptionDescriptor> OptionDescriptors { get; set; } = new List<OptionDescriptor>();
        public IEnumerable<CommandDescriptor> SubCommandDescriptors { get; set; } = new List<CommandDescriptor>();

        public string GeneratedSource
            => CommandDefinitionSourceCode;

        public string CommandDefinitionSourceCode { get; set; }

        // This should be a class that matches ICommandSource
        public string CompiledClassName { get; set; }
        public Action<Command> TestAction { get; set; }

        protected string GeneratedSourceOpening
            => @$"using System.CommandLine;

   public class {GeneratedSourceClassName}
   {{
        public Command GetCommand()
        {{";

        protected string GeneratedSourceClosing
            => $@"
        }}
   }}";


    }
    //public class Samples
    //{
    //    public static List<CommandExpectedData> Data = new List<CommandExpectedData>
    //    {

    //        new CommandExpectedData
    //        {
    //            FileName = "SingleArgumentSampleData.cs",
    //            Name = "my-class",
    //            Arguments = new List<ArgumentExpectedData>
    //                { new ArgumentExpectedData
    //                    {
    //                        Name = "my-property",
    //                        Type = "string",
    //                    }
    //                }
    //        },
    //        new CommandExpectedData
    //        {
    //            FileName = "SingleOptionWithArgumentSampleData.cs",
    //            Name = "my-class",
    //            Options = new List<OptionExpectedData>
    //            {
    //                new OptionExpectedData
    //                {
    //                    Name = "--my-property",
    //                    Arguments = new List<ArgumentExpectedData>
    //                        { new ArgumentExpectedData
    //                            {
    //                                Name = "my-property",
    //                                Type = "string",
    //                            }
    //                        },
    //                }
    //            }
    //        },
    //         new CommandExpectedData
    //        {
    //            FileName = "SeveralOptionsAndArgumentsSampleData.cs",
    //            Name = "my-class",
    //            Arguments = new List<ArgumentExpectedData>
    //                { new ArgumentExpectedData
    //                    {
    //                        Name = "my-arg-property",
    //                        Type =  "string",
    //                    }
    //                },
    //            Options = new List<OptionExpectedData>
    //            {
    //                new OptionExpectedData
    //                {
    //                    Name ="--my-property",
    //                    Arguments = new List<ArgumentExpectedData>
    //                        { new ArgumentExpectedData
    //                            {
    //                                Name = "my-property",
    //                                Type =  "string",
    //                            }
    //                        },
    //                },
    //                new OptionExpectedData
    //                {
    //                    Name = "--my-property2",
    //                    Arguments = new List<ArgumentExpectedData>
    //                        { new ArgumentExpectedData
    //                            {
    //                                Name = "my-property2",
    //                                Type =  "int",
    //                            }
    //                        },
    //                },
    //                new OptionExpectedData
    //                {
    //                    Name = "--my-property3",
    //                    Arguments = new List<ArgumentExpectedData>
    //                        { new ArgumentExpectedData
    //                            {
    //                                Name = "my-property3",
    //                                Type =  "DateTime",
    //                            }
    //                        },
    //                }
    //            }
    //        },
    //    };
    //}
}
