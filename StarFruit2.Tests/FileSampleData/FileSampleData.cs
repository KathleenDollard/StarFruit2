using StarFruit2.Tests.SampleData;
using System.Collections.Generic;

namespace StarFruit2.Tests.FileSampleData
{
    public class Samples
    {
        public static List<FileCommandSampleData> Data = new List<FileCommandSampleData>
        {
            new FileCommandSampleData("EmptySampleData.cs", new CommandExpectedData
            {
                  Name = "my-class"
            }),
            new FileCommandSampleData("SingleArgumentSampleData.cs", new CommandExpectedData
            {
                Name = "my-class",
                Arguments = new List<ArgumentSampleData>
                    { new ArgumentSampleData
                        {
                            Name = "my-property",
                            Type = "string",
                        }
                    }
            }),
            new FileCommandSampleData("SingleOptionWithArgumentSampleData.cs",new CommandExpectedData
            {
                Name = "my-class",
                Options = new List<OptionSampleData>
                {
                    new OptionSampleData
                    {
                        Name = "--my-property",
                        Arguments = new List<ArgumentSampleData>
                            { new ArgumentSampleData
                                {
                                    Name = "my-property",
                                    Type = "string",
                                }
                            },
                    }
                }
            }),
             new FileCommandSampleData("SeveralOptionsAndArgumentsSampleData.cs", new CommandExpectedData
            {
                Name = "my-class",
                Arguments = new List<ArgumentSampleData>
                    { new ArgumentSampleData
                        {
                            Name = "my-arg-property",
                            Type =  "string",
                        }
                    },
                Options = new List<OptionSampleData>
                {
                    new OptionSampleData
                    {
                        Name ="--my-property",
                        Arguments = new List<ArgumentSampleData>
                            { new ArgumentSampleData
                                {
                                    Name = "my-property",
                                    Type =  "string",
                                }
                            },
                    },
                    new OptionSampleData
                    {
                        Name = "--my-property2",
                        Arguments = new List<ArgumentSampleData>
                            { new ArgumentSampleData
                                {
                                    Name = "my-property2",
                                    Type =  "int",
                                }
                            },
                    },
                    new OptionSampleData
                    {
                        Name = "--my-property3",
                        Arguments = new List<ArgumentSampleData>
                            { new ArgumentSampleData
                                {
                                    Name = "my-property3",
                                    Type =  "DateTime",
                                }
                            },
                    }
                }
            }),
        };
    }
}
