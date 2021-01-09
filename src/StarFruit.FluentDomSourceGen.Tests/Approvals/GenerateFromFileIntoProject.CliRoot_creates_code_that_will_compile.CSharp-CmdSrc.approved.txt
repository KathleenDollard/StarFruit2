using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System;

namespace StarFruit.FromClass
{
   public class CliRootCommandSource : RootCommandSource<CliRootCommandSource>
   {
      public CliRootCommandSource()
      : this(null, null)
      {
      }
      public CliRootCommandSource(RootCommandSource root, CommandSourceBase parent)
      : base(new Command("cli-root", "This is the entry point, the end user types the executable name"), parent)
      {
         StringPropertyOption = GetStringProperty();
         Command.Add(StringPropertyOption);
         ctorParamOption = GetctorParam();
         Command.Add(ctorParamOption);
         FindCommand = new FindCommandSource(this, this);
         Command.AddCommand(FindCommand.Command);
         ListCommand = new ListCommandSource(this, this);
         Command.AddCommand(ListCommand.Command);
         Command.Handler = CommandHandler.Create((InvocationContext context) =>
             {  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
             });
      }

      public Option<string> StringPropertyOption { get; set; }
      public Option<bool> ctorParamOption { get; set; }
      public FindCommandSource FindCommand { get; set; }
      public ListCommandSource ListCommand { get; set; }

      public Option<string> GetStringProperty()
      {
         Option<string> opt = new Option<string>("--string-property");
         opt.Description = "This is a string property";
         opt.IsRequired = false;
         opt.IsHidden = false;
         var optionArg = opt.Argument;
         optionArg.SetDefaultValue("abc");
         opt.AddAlias("-o");
         return opt;
      }
      public Option<bool> GetctorParam()
      {
         Option<bool> opt = new Option<bool>("--ctor-param");
         opt.Description = "This is a constructor parameter";
         opt.IsRequired = false;
         opt.IsHidden = false;
         return opt;
      }
   }
   public class FindCommandSource : CommandSourceBase
   {
      public FindCommandSource(RootCommandSource root, CommandSourceBase parent)
      : base(new Command("find", "Use this to find things"), parent)
      {
         intArgArgument = GetintArg();
         Command.Add(intArgArgument);
         stringOptionOption = GetstringOption();
         Command.Add(stringOptionOption);
         boolOptionOption = GetboolOption();
         Command.Add(boolOptionOption);
         Command.Handler = CommandHandler.Create(() =>
             {  
                root.CurrentCommandSource = this;
                return 0;
             });
      }

      public Argument<int> intArgArgument { get; set; }
      public Option<string> stringOptionOption { get; set; }
      public Option<bool> boolOptionOption { get; set; }

      public Argument<int> GetintArg()
      {
         Argument<int> argument = new Argument<int>("int");
         argument.Description = "This is an integer argument";
         argument.IsHidden = false;
         argument.SetDefaultValue(42);
         return argument;
      }
      public Option<string> GetstringOption()
      {
         Option<string> opt = new Option<string>("--string");
         opt.Description = "This is an string argument";
         opt.IsRequired = false;
         opt.IsHidden = false;
         return opt;
      }
      public Option<bool> GetboolOption()
      {
         Option<bool> opt = new Option<bool>("--bool");
         opt.Description = "This is an bool argument";
         opt.IsRequired = false;
         opt.IsHidden = false;
         return opt;
      }
   }
   public class ListCommandSource : CommandSourceBase
   {
      public ListCommandSource(RootCommandSource root, CommandSourceBase parent)
      : base(new Command("list", "List the elements you are interested in"), parent)
      {
         verbosityOption = Getverbosity();
         Command.Add(verbosityOption);
         Command.Handler = CommandHandler.Create(() =>
             {  
                root.CurrentCommandSource = this;
                return 0;
             });
      }

      public Option<VerbosityLevel> verbosityOption { get; set; }

      public Option<VerbosityLevel> Getverbosity()
      {
         Option<VerbosityLevel> opt = new Option<VerbosityLevel>("--verbosity");
         opt.Description = "The degree of detail desired";
         opt.IsRequired = false;
         opt.IsHidden = false;
         return opt;
      }
   }
}
