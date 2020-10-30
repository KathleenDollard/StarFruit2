using StarFruit2;
using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    public partial class CliRootCommandSource : CommandSource<CliRoot>
    {
        public Argument<int> Find_IntArg { get; private set; }
        public Option<string> Find_StringOption { get; set; }
        public Option<bool> Find_BoolOption { get; set; }
        public Option<bool> List_BoolOption { get; set; }
        public Option<string> StringProperty { get; set; }
        public Option<bool> CtorParam { get; set; }

        public Command ListCommand { get; set; }
        public Command FindCommand { get; set; }

        public CliRootCommandSource()
           : base(new Command("cli-root"))
        {
            StringProperty = new Option<string>("string-property");
            CtorParam = new Option<bool>("ctor-param");
            RootCommand.AddOption(CtorParam);
            RootCommand.AddOption(StringProperty);
            RootCommand.AddCommand(GetFindCommand());
            RootCommand.AddCommand(GetListCommand());
            // no handler because not invokable
        }

        public int InvokeFind(BindingContext bindingContext)
        {
            NewInstance = GetNewInstance(bindingContext);
            var intArg = GetValue(bindingContext, Find_IntArg);
            var stringOption = GetValue(bindingContext, Find_StringOption);
            var boolOption = GetValue(bindingContext, Find_BoolOption);
            RunFunc = () => NewInstance.FindAsync(stringOption, boolOption, intArg);
            return 0;
        }

        public int InvokeList(BindingContext bindingContext)
        {
            NewInstance = GetNewInstance(bindingContext);
            var boolOption = GetValue(bindingContext, Find_BoolOption);
            RunFunc = () => NewInstance.ListAsync ( boolOption);
            return 0;
        }

        private Command GetFindCommand()
        {
            Find_IntArg = new Argument<int>("int-arg")
            {
                Description = "this is cool",
                Arity = new ArgumentArity(0,1), // this is based on whether it is a collection, required and Arity if the descriptor supports that
                IsHidden = false,
            };
            //Find_IntArg.AllowedValues.AddRange("X", "Z");
            // Find_IntArg.SetDefaultValue... only if we have one

            Find_StringOption = new Option<string>("--string-option")
            {
                Description = "this is cooler",
                IsRequired = true,
                IsHidden = false,
            };
            var find_StringOption_arg = new Argument("name"); // most of the stuff from arg above
            Find_StringOption.Argument = find_StringOption_arg;
            Find_StringOption.AddAlias("-a");


            Find_BoolOption = new Option<bool>("bool-option"); // similar to string option

            FindCommand = new Command("find", "Yep, cool")
            {
                Find_BoolOption,
                Find_StringOption,
                Find_BoolOption
            };
            //FindCommand.IsHidden = false;
            //Find_StringOption.AddAlias("-a");
            //Find_StringOption.TreatUnmatchedTokensAsErrors = true;

            FindCommand.Handler = new CommandSourceCommandHandler(InvokeFind);

            return FindCommand;
        }

        private Command GetListCommand()
        {

            List_BoolOption = new Option<bool>("bool-option");
            ListCommand = new Command("find")
            {
                List_BoolOption
            };
            ListCommand.Handler = new CommandSourceCommandHandler(InvokeList);

            return ListCommand;

        }

        private CliRoot GetNewInstance(BindingContext bindingContext)
            => new CliRoot(GetValue(bindingContext, CtorParam))
            {
                StringProperty = GetValue(bindingContext, StringProperty)
            };
    }

}
