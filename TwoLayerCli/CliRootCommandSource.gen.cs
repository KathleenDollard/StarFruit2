using StarFruit2;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    public partial class CliRootCommandSource
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
            Command.AddOption(CtorParam);
            Command.AddOption(StringProperty);
            Command.AddCommand(GetFindCommand());
            Command.AddCommand(GetListCommand());
            // no handler because not invokable
        }

        public async Task<int> InvokeFindAsync(BindingContext bindingContext)
        {
            NewInstance = GetNewInstance(bindingContext);
            return await NewInstance.FindAsync(GetValue(bindingContext, Find_IntArg),
                                               GetValue(bindingContext, Find_StringOption),
                                               GetValue(bindingContext, Find_BoolOption));
        }

        public async Task<int> InvokeListAsync(BindingContext bindingContext)
        {
            NewInstance = GetNewInstance(bindingContext);
            return await NewInstance.ListAsync(GetValue(bindingContext, List_BoolOption));
        }

        private Command GetFindCommand()
        {
            Find_IntArg = new Argument<int>("int-arg"); // This would actually be a lot more involved.
            Find_StringOption = new Option<string>("string-option");
            Find_BoolOption = new Option<bool>("bool-option");

            FindCommand = new Command("find")
            {
                Find_BoolOption,
                Find_StringOption,
                Find_BoolOption
            };
            FindCommand.Handler = new CommandSourceCommandHandler(InvokeFindAsync);

            return FindCommand;
        }

        private Command GetListCommand()
        {

            List_BoolOption = new Option<bool>("bool-option");
            ListCommand = new Command("find")
            { 
                List_BoolOption 
            };
            ListCommand.Handler = new CommandSourceCommandHandler(InvokeListAsync);

            return ListCommand;

        }

        private CliRoot GetNewInstance(BindingContext bindingContext)
            => new CliRoot(GetValue(bindingContext, CtorParam))
            {
                StringProperty = GetValue(bindingContext, StringProperty)
            };
    }

}
