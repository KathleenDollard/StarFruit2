using StarFruit2;
using StarFruit2.Common;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace TwoLayerCli
{

    public partial class CliRootCommandSource : RootCommandSource<CliRoot>
    {
        public CliRootCommandSource()
             : base(new Command("cli-root"))
        {
            StringProperty = GetStringProperty();
            CtorParam = new Option<bool>("ctor-param");
            Command.AddOption(CtorParam);
            Command.AddOption(StringProperty);
            Find = new FindCommandSource(this, this);
            Command.AddCommand(Find.Command);
            List = new ListCommandSource(this, this);
            Command.AddCommand(List.Command);

            Command.Handler = CommandHandler.Create((InvocationContext context) =>
            {
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
            });
        }

        public Option<string> StringProperty { get; set; }
        public Option<bool> CtorParam { get; set; }

        public ListCommandSource List { get; set; }
        public FindCommandSource Find { get; set; }

        private Option<string> GetStringProperty()
        {
            var option = new Option<string>("--string-property")
            {
                Description = "this is cooler",
                IsRequired = true,
                IsHidden = false,
            };
            var optionArg = new Argument<string>("name");
            // only if DefaultValue is not null
            optionArg.SetDefaultValue("abc");
            option.Argument = optionArg;
            option.AddAlias("-a");
            return option;
        }
    }

    public class FindCommandSource : CommandSource
    {
        internal CliRootCommandSource parent;

        internal FindCommandSource(CliRootCommandSource root, CliRootCommandSource parent)
            : base(new Command("find", "Yep, cool"))
        {
            this.parent = parent;
            IntArg = GetIntArg();
            Command.Add(IntArg);
            StringOption = GetStringOption();
            Command.Add(StringOption);
            BoolOption = GetBoolOption();
            Command.Add(BoolOption);

            Command.Handler = CommandHandler.Create(() => { root.CurrentCommandSource = this; return 0; });
        }

        protected override CommandSourceResult GetCommandSourceResult(ParseResult parseResult, int exitCode)
        {
            return new FindCommandSourceResult(parseResult, this, exitCode);
        }

        public Argument<int> IntArg { get; private set; }
        public Option<string> StringOption { get; set; }
        public Option<bool> BoolOption { get; set; }

        // may no longer need 85 or 86 
        public Option<string> StringProperty => parent.StringProperty;
        public Option<bool> CtorParam => parent.CtorParam;

        private Argument<int> GetIntArg()
        {
            var argument = new Argument<int>("int-arg")
            {
                Description = "this is cool",
                Arity = new ArgumentArity(0, 1), // this is based on whether it is a collection, required and Arity if the descriptor supports that
                IsHidden = false,
            };
            // only if DefaultValue is not null
            argument.SetDefaultValue(42);
            return argument;
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
            : base(new Command("list"))
        {
            this.parent = parent;
            VerbosityOption = GetVerbosityOption();

            Command.Add(VerbosityOption);

            Command.Handler = CommandHandler.Create(() => { root.CurrentCommandSource = this; return 0; });
        }

        protected override CommandSourceResult GetCommandSourceResult(ParseResult parseResult, int exitCode)
        {
            return new ListCommandSourceResult(parseResult, this, exitCode);
        }

        public Option<VerbosityLevel> VerbosityOption { get; set; }

        public Option<string> StringProperty => parent.StringProperty;
        public Option<bool> CtorParam => parent.CtorParam;


        private Option<VerbosityLevel> GetVerbosityOption()
        {
            return new Option<VerbosityLevel>("verbosity-option");
        }
    }

}
