// COMMENTING OUT FOR BUILDING

//using StarFruit2;
//using System.CommandLine;
//using System.CommandLine.Binding;
//using System.Threading.Tasks;

//namespace MultiLayerCli
//{

//    public partial class CliRootCommandSource
//    {
//        private ToolCommandSource toolCommandSource;

//        public CliRootCommandSource()
//           : base(new Command("cliRoot"))
//        {
//            toolCommandSource = new ToolCommandSource();
//            Command.AddCommand(toolCommandSource.Command);
//        }
//    }

//    public class ToolCommandSource : CommandSource<Tool>
//    {
//        public Argument<int> Find_intArg { get; set; }
//        public Option<string> Find_StringOption { get; set; }
//        public Option<bool> Find_BoolOption { get; set; }
//        public Option<bool> List_BoolOption { get; set; }
//        public Option<string> StringProperty { get; set; }
//        public Option<bool> CtorParam { get; set; }

//        private Command FindCommand { get; set; }

//        private Command ListCommand { get; set; }

//        public ToolCommandSource()
//            : base(new Command("tool"))
//        {
//            StringProperty = new Option<string>("string-property");
//            CtorParam = new Option<bool>("ctor-param");
//            Command.AddOption(CtorParam);
//            Command.AddOption(StringProperty);
//            Command.AddCommand(GetFindCommand());
//            Command.AddCommand(GetListCommand());
//            // no handler because this is not invokable
//        }


//        public async Task<int> InvokeFindAsync(BindingContext bindingContext)
//        {
//            var newInstance = GetNewToolInstance(bindingContext);
//            return await newInstance.FindAsync(GetValue(bindingContext, Find_intArg),
//                                               GetValue(bindingContext, Find_StringOption),
//                                               GetValue(bindingContext, Find_BoolOption));
//        }

//        public async Task<int> InvokeListAsync(BindingContext bindingContext)
//        {
//            var newInstance = GetNewToolInstance(bindingContext);
//            return await newInstance.ListAsync(GetValue(bindingContext, List_BoolOption));
//        }

//        private Command GetFindCommand()
//        {
//            FindCommand = new Command("find");

//            Find_intArg = new Argument<int>("int-arg"); // This would actually be a lot more involved.
//            Find_StringOption = new Option<string>("string-option");
//            Find_BoolOption = new Option<bool>("bool-option");
//            FindCommand.AddArgument(Find_intArg);
//            FindCommand.AddOption(Find_StringOption);
//            FindCommand.AddOption(Find_BoolOption);
//            FindCommand.Handler = new CommandSourceCommandHandler(InvokeFindAsync);

//            return FindCommand;
//        }

//        private Command GetListCommand()
//        {
//            ListCommand = new Command("find");

//            List_BoolOption = new Option<bool>("bool-option");
//            ListCommand.AddOption(List_BoolOption);
//            ListCommand.Handler = new CommandSourceCommandHandler(InvokeListAsync);

//            return ListCommand;

//        }

//        private Tool GetNewToolInstance(BindingContext bindingContext)
//        => new Tool(GetValue(bindingContext, CtorParam))
//        {
//            StringProperty = GetValue(bindingContext, StringProperty)
//        };
//    }

//}
