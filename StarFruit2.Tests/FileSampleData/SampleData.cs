using StarFruit2.Common.Descriptors;
using StarFruit2.Tests.SampleData;
using System.Collections.Generic;
using System.CommandLine;

namespace StarFruit2.Tests.FileSampleData
{
    public class Samples
    {
        public class SampleData
        {
public string FileName { get; set; }
            public CommandDescriptor CommandDescriptor { get; set; }
            public Command Command { get; set; }
        }

        public static List<SampleData> Data = new List<SampleData>
        {
            new SampleData
            {
                FileName = "EmptySampleData.cs",
                new CommandDescriptor ( = "my-class"
            },
            new SampleData
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
        }
        //    new CommandExpectedData
        //    {
        //        FileName = "SingleOptionWithArgumentSampleData.cs",
        //        Name = "my-class",
        //        Options = new List<OptionExpectedData>
        //        {
        //            new OptionExpectedData
        //            {
        //                Name = "--my-property",
        //                Arguments = new List<ArgumentExpectedData>
        //                    { new ArgumentExpectedData
        //                        {
        //                            Name = "my-property",
        //                            Type = "string",
        //                        }
        //                    },
        //            }
        //        }
        //    },
        //     new CommandExpectedData
        //    {
        //        FileName = "SeveralOptionsAndArgumentsSampleData.cs",
        //        CommandDescriptor 
        //        Name = "my-class",
        //        Arguments = new List<ArgumentExpectedData>
        //            { new ArgumentExpectedData
        //                {
        //                    Name = "my-arg-property",
        //                    Type =  "string",
        //                }
        //            },
        //        Options = new List<OptionExpectedData>
        //        {
        //            new OptionExpectedData
        //            {
        //                Name ="--my-property",
        //                Arguments = new List<ArgumentExpectedData>
        //                    { new ArgumentExpectedData
        //                        {
        //                            Name = "my-property",
        //                            Type =  "string",
        //                        }
        //                    },
        //            },
        //            new OptionExpectedData
        //            {
        //                Name = "--my-property2",
        //                Arguments = new List<ArgumentExpectedData>
        //                    { new ArgumentExpectedData
        //                        {
        //                            Name = "my-property2",
        //                            Type =  "int",
        //                        }
        //                    },
        //            },
        //            new OptionExpectedData
        //            {
        //                Name = "--my-property3",
        //                Arguments = new List<ArgumentExpectedData>
        //                    { new ArgumentExpectedData
        //                        {
        //                            Name = "my-property3",
        //                            Type =  "DateTime",
        //                        }
        //                    },
        //            }
        //        }
        //    },
        //};
    }
}
