//using StarFruit2.Tests.SampleData;
//using System.Collections.Generic;

//namespace StarFruit2.Tests
//{
//    public class Sample
//    {
//        public static Dictionary<string, CommandSampleData> Data = new Dictionary<string, CommandSampleData>
//        {
//            ["Empty"] = new CommandSampleData
//            {
//                Code = @"
//                    public class MyClass
//                    {
//                        public void MyMethod()
//                        {
//                        }
//                    }",
//                Name = "my-class"
//            },
//            ["SingleSimpleArgument"] = new CommandSampleData
//            {
//                Code = @"
//                    public class MyClass
//                    {
//                        public string MyPropertyArg{get; set;}
//                    }",
//                Name = "my-class",
//                Arguments = new List<ArgumentSampleData>
//                    { new ArgumentSampleData
//                        {
//                            Name = "my-property",
//                            Type = "string",
//                        }
//                    }
//            },
//            ["SingleOptionWithArgument"] = new CommandSampleData
//            {
//                Code = @"
//                    public class MyClass
//                    {
//                        public string MyProperty{get; set;}
//                    }",
//                Name = "my-class",
//                Options = new List<OptionSampleData>
//                {
//                    new OptionSampleData
//                    {
//                        Name = "--my-property",
//                        Arguments = new List<ArgumentSampleData>
//                            { new ArgumentSampleData
//                                {
//                                    Name = "my-property",
//                                    Type = "string",
//                                }
//                            },
//                    }
//                }
//            },
//            ["SeveralOptionsAndArguments"] = new CommandSampleData
//            {
//                Code = @"
//                    public class MyClass
//                    {
//                        public string MyProperty{get; set;}
//                        public string MyArgPropertyArg{get; set;}
//                        public int MyProperty2{get; set;}
//                        public datetime MyProperty3{get; set;}
//                    }",
//                Name = "my-class",
//                Arguments = new List<ArgumentSampleData>
//                    { new ArgumentSampleData
//                        {
//                            Name = "my-arg-property",
//                            Type =  "string",
//                        }
//                    },
//                Options = new List<OptionSampleData>
//                {
//                    new OptionSampleData
//                    {
//                        Name ="--my-property",
//                        Arguments = new List<ArgumentSampleData>
//                            { new ArgumentSampleData
//                                {
//                                    Name = "my-property",
//                                    Type =  "string",
//                                }
//                            },
//                    },
//                    new OptionSampleData
//                    {
//                        Name = "--my-property2",
//                        Arguments = new List<ArgumentSampleData>
//                            { new ArgumentSampleData
//                                {
//                                    Name = "my-property2",
//                                    Type =  "int",
//                                }
//                            },
//                    },
//                    new OptionSampleData
//                    {
//                        Name = "--my-property3",
//                        Arguments = new List<ArgumentSampleData>
//                            { new ArgumentSampleData
//                                {
//                                    Name = "my-property3",
//                                    Type =  "datetime",
//                                }
//                            },
//                    }
//                }
//            },
//        };
//    }
//}
