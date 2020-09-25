using StarFruit2.Tests.SampleData;
using System.Collections.Generic;

namespace StarFruit2.Tests.FileSampleData
{
    public class Samples
    {
        public static List<CommandExpectedData> Data = new List<CommandExpectedData>
        {
            new CommandExpectedData
            {
                FileName = "EmptySampleData.cs",
                 Name = "my-class"
            },
            new CommandExpectedData
            {
                FileName = "SingleArgumentSampleData.cs",
                Name = "my-class",
                Arguments = new List<ArgumentExpectedData>
                    { new ArgumentExpectedData
                        {
                            Name = "my-property",
                            Type = "string",
                        }
                    }
            },
            new CommandExpectedData
            {
                FileName = "SingleOptionWithArgumentSampleData.cs",
                Name = "my-class",
                Options = new List<OptionExpectedData>
                {
                    new OptionExpectedData
                    {
                        Name = "--my-property",
                        Arguments = new List<ArgumentExpectedData>
                            { new ArgumentExpectedData
                                {
                                    Name = "my-property",
                                    Type = "string",
                                }
                            },
                    }
                }
            },
             new CommandExpectedData
            {
                FileName = "SeveralOptionsAndArgumentsSampleData.cs",
                Name = "my-class",
                Arguments = new List<ArgumentExpectedData>
                    { new ArgumentExpectedData
                        {
                            Name = "my-arg-property",
                            Type =  "string",
                        }
                    },
                Options = new List<OptionExpectedData>
                {
                    new OptionExpectedData
                    {
                        Name ="--my-property",
                        Arguments = new List<ArgumentExpectedData>
                            { new ArgumentExpectedData
                                {
                                    Name = "my-property",
                                    Type =  "string",
                                }
                            },
                    },
                    new OptionExpectedData
                    {
                        Name = "--my-property2",
                        Arguments = new List<ArgumentExpectedData>
                            { new ArgumentExpectedData
                                {
                                    Name = "my-property2",
                                    Type =  "int",
                                }
                            },
                    },
                    new OptionExpectedData
                    {
                        Name = "--my-property3",
                        Arguments = new List<ArgumentExpectedData>
                            { new ArgumentExpectedData
                                {
                                    Name = "my-property3",
                                    Type =  "DateTime",
                                }
                            },
                    }
                }
            },
        };
    }
}
