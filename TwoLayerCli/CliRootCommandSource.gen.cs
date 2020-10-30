using StarFruit2;
using StarFruit2.Common;
using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace TwoLayerCli
{

    public partial class CliRootCommandSource : RootCommandSource<CliRoot>
    {
        public CliRootCommandSource()
             : base(new Command("cli-root"))
        {
            StringProperty = new Option<string>("string-property");
            CtorParam = new Option<bool>("ctor-param");
            Command.AddOption(CtorParam);
            Command.AddOption(StringProperty);
            Find = new FindCommandSource(this, this);
            List = new ListCommandSource(this, this);
            Command.AddCommand(Find.Command);
            Command.AddCommand(List.Command);

            Command.Handler = CommandHandler.Create(() => { CurrentCommandSource = this; return 0; });
        }


        public Option<string> StringProperty { get; set; }
        public Option<bool> CtorParam { get; set; }

        public ListCommandSource List { get; set; }
        public FindCommandSource Find { get; set; }

    }

    public class FindCommandSource : CommandSource
    {
        internal CliRootCommandSource parent;

        internal FindCommandSource(CliRootCommandSource root, CliRootCommandSource parent)
        {
            this.parent = parent;
            IntArg = GetIntArg();

            StringOption = GetStringOption();

            BoolOption = GetBoolOption();

            Command = new Command("find", "Yep, cool");
            Command.Add(IntArg);
            Command.Add(StringOption);
            Command.Add(BoolOption);

            Command.Handler = CommandHandler.Create(() => { root.CurrentCommandSource = this; return 0; });
        }

        protected override CommandSourceResult GetCommandSourceResult(ParseResult parseResult)
        {
            return new FindCommandSourceResult(parseResult, this);
        }

        public Argument<int> IntArg { get; private set; }
        public Option<string> StringOption { get; set; }
        public Option<bool> BoolOption { get; set; }

        public Option<string> StringProperty => parent.StringProperty;
        public Option<bool> CtorParam => parent.CtorParam;

        private Argument<int> GetIntArg()
        {
            return new Argument<int>("int-arg")
            {
                Description = "this is cool",
                Arity = new ArgumentArity(0, 1), // this is based on whether it is a collection, required and Arity if the descriptor supports that
                IsHidden = false,
            };
            //IntArg.AllowedValues.AddRange("X", "Z");
            // IntArg.SetDefaultValue... only if we have one
        }
        private Option<string> GetStringOption()
        {
            var option = new Option<string>("--string-option")
            {
                Description = "this is cooler",
                IsRequired = true,
                IsHidden = false,
            };
            var find_StringOption_arg = new Argument<string>("name"); // most of the stuff from arg above
            option.Argument = find_StringOption_arg;
            option.AddAlias("-a");
            return option;
        }
        private Option<bool> GetBoolOption()
        {
            var option = new Option<bool>("bool-option"); // similar to string option
            return option;
        }
    }

    public class ListCommandSource : CommandSource
    {
        internal CliRootCommandSource parent;

        internal ListCommandSource(CliRootCommandSource root, CliRootCommandSource parent)
        {
            this.parent = parent;
            VerbosityOption = GetVerbosityOption();

            Command = new Command("list");
            Command.Add(VerbosityOption);

            Command.Handler = CommandHandler.Create(() => { root.CurrentCommandSource = this; return 0; });
        }

        protected override CommandSourceResult GetCommandSourceResult(ParseResult parseResult)
        {
            return new ListCommandSourceResult(parseResult, this);
        }

        public Option<VerbosityLevel> VerbosityOption { get; set; }

        public Option<string> StringProperty => parent.StringProperty;
        public Option<bool> CtorParam => parent.CtorParam;


        private Option<VerbosityLevel> GetVerbosityOption()
        {
            return new Option<VerbosityLevel>("verbosity-option");
        }
    }


    //public partial class CliRootCommandSourceOld : CommandSource<CliRoot>
    //{
    //    public Argument<int> Find_IntArg { get; private set; }
    //    public Option<string> Find_StringOption { get; set; }
    //    public Option<bool> Find_BoolOption { get; set; }
    //    public Option<bool> List_BoolOption { get; set; }
    //    public Option<string> StringProperty { get; set; }
    //    public Option<bool> CtorParam { get; set; }

    //    public Command ListCommand { get; set; }
    //    public Command FindCommand { get; set; }

    //    public CliRootCommandSource()
    //       : base(new Command("cli-root"))
    //    {
    //        StringProperty = new Option<string>("string-property");
    //        CtorParam = new Option<bool>("ctor-param");
    //        RootCommand.AddOption(CtorParam);
    //        RootCommand.AddOption(StringProperty);
    //        RootCommand.AddCommand(GetFindCommand());
    //        RootCommand.AddCommand(GetListCommand());
    //        // no handler because not invokable
    //    }

    //    public int InvokeFind(BindingContext bindingContext)
    //    {
    //        NewInstance = GetNewInstance(bindingContext);
    //        var intArg = GetValue(bindingContext, Find_IntArg);
    //        var stringOption = GetValue(bindingContext, Find_StringOption);
    //        var boolOption = GetValue(bindingContext, Find_BoolOption);
    //        RunFunc = () => NewInstance.FindAsync(stringOption, boolOption, intArg);
    //        return 0;
    //    }

    //    public int InvokeList(BindingContext bindingContext)
    //    {
    //        NewInstance = GetNewInstance(bindingContext);
    //        var boolOption = GetValue(bindingContext, Find_BoolOption);
    //        RunFunc = () => NewInstance.ListAsync ( boolOption);
    //        return 0;
    //    }

    //    private Command GetFindCommand()
    //    {
    //        Find_IntArg = new Argument<int>("int-arg")
    //        {
    //            Description = "this is cool",
    //            Arity = new ArgumentArity(0,1), // this is based on whether it is a collection, required and Arity if the descriptor supports that
    //            IsHidden = false,
    //        };
    //        //Find_IntArg.AllowedValues.AddRange("X", "Z");
    //        // Find_IntArg.SetDefaultValue... only if we have one

    //        Find_StringOption = new Option<string>("--string-option")
    //        {
    //            Description = "this is cooler",
    //            IsRequired = true,
    //            IsHidden = false,
    //        };
    //        var find_StringOption_arg = new Argument("name"); // most of the stuff from arg above
    //        Find_StringOption.Argument = find_StringOption_arg;
    //        Find_StringOption.AddAlias("-a");


    //        Find_BoolOption = new Option<bool>("bool-option"); // similar to string option

    //        FindCommand = new Command("find", "Yep, cool")
    //        {
    //            Find_BoolOption,
    //            Find_StringOption,
    //            Find_BoolOption
    //        };
    //        //FindCommand.IsHidden = false;
    //        //Find_StringOption.AddAlias("-a");
    //        //Find_StringOption.TreatUnmatchedTokensAsErrors = true;

    //        FindCommand.Handler = new CommandSourceCommandHandler(InvokeFind);

    //        return FindCommand;
    //    }

    //    private Command GetListCommand()
    //    {

    //        List_BoolOption = new Option<bool>("bool-option");
    //        ListCommand = new Command("find")
    //        {
    //            List_BoolOption
    //        };
    //        ListCommand.Handler = new CommandSourceCommandHandler(InvokeList);

    //        return ListCommand;

    //    }

    //    private CliRoot GetNewInstance(BindingContext bindingContext)
    //        => new CliRoot(GetValue(bindingContext, CtorParam))
    //        {
    //            StringProperty = GetValue(bindingContext, StringProperty)
    //        };
    //}

}
