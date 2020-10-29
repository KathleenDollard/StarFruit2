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
            // gen by looping over subcommand opt and args
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
            Find_IntArg = GetArg<int>("int-arg", "this is a description" /* pass stuff here, taken from descriptor within GENERATOR*/);
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

        // TODO: consider putting me in the base class, maybe GetOpt<T> as well, see CodeGenerator.cs GetArgument
        private Argument<T> GetArg<T>(string name, string description /* ... more stuff */)
        {
            var arg = new Argument<T>(name) // This would actually be a lot more involved.
            { Description = description, };
            arg.SetDefaultValue(1);

            return arg;
        }

        private Command GetListCommand()
        {

            List_BoolOption = new Option<bool>("bool-option");
            ListCommand = new Command("list");
            //{ 
            //    List_BoolOption 
            //};
            ListCommand.Add(List_BoolOption);
            ListCommand.Handler = new CommandSourceCommandHandler(InvokeListAsync);

            return ListCommand;

        }

        // must be here, since we have to gen 89 and 91
        private CliRoot GetNewInstance(BindingContext bindingContext)
        {
            return new CliRoot(GetValue(bindingContext, CtorParam))
            {
                StringProperty = GetValue(bindingContext, StringProperty)
            };
        }
    }

}
