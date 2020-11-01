//using StarFruit2;
//using System.CommandLine;
//using StarFruit2.Common;
//using System.CommandLine.Invocation;
//using System.CommandLine.Parsing;
//namespace StarFruit2.Tests.TestSampleDatatestName
//{
//    public partial class MyClassCommandSource
//        : RootCommandSource<MyClass>
//    {
//        public MyClassCommandSource()
//            : base(new Command(, ))
//SomeOption = GetSomeOption()
//            Command.Add(SomeOption);
//            Command.Handler = CommandHandler.Create(() => { CurrentCommandSource = this; return this; })
//}
//protected override CommandSourceResult GetCommandSourceResult(ParseResult parseResult)
//    return new my- classCommandSourceResult(parseResult, this);
//public Option<String> SomeOption { get; set; }
//private Option<String> GetSomeOption()
//    var option = new Option<String>(some - option)
//Description = desc,
//IsRequired = False,
//IsHidden = False,
//};
//var my-class_SomeOption_arg = new Argument<String>(some - option)
//    option.Argument = var my - class_SomeOption_arg
//    argument.SetDefaultValue(\"abc\")
//        return option;
//}

////using StarFruit2;
////using System.CommandLine;
////using System;
////using StarFruit2.Common;
////using System.CommandLine.Invocation;
////using System.CommandLine.Parsing;
////namespace StarFruit2.Tests.TestSampleDatatestName
////{
////    public partial class MyClassCommandSource
////        : RootCommandSource<MyClass>
////    {
////        public MyClassCommandSource()
////            : base(new Command("", ""))
////        {
////            SomeOption = GetSomeOption();
////            Command.Add(SomeOption);
////            Command.Handler = CommandHandler.Create(() => { CurrentCommandSource = this; return this; });
////        }
////        protected override CommandSourceResult GetCommandSourceResult(ParseResult parseResult)
////        {
////            return new my- classCommandSourceResult(parseResult, this);
////        }
////        public Option<String> SomeOption { get; set; }
////        private Option<String> GetSomeOption()
////        {
////            var option = new Option<String>("some - option")
////            {
////                Description = "desc",
////                IsRequired = false,
////                IsHidden = false,
////            };
////            var my-class_SomeOption_arg = new Argument<String>(some - option);
////            option.Argument = var my - class_SomeOption_arg;
////            argument.SetDefaultValue("\"abc\"");
////            return option;
////        }
////    }
////}