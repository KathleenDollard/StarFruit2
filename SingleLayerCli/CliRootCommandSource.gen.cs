using StarFruit2;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;

namespace SingleLayerCli
{
    public partial class CliRootCommandSource
    {
        public CliRootCommandSource()
           : base(new Command("cliRoot"))
        {
            Root_IntArg = new Argument<int>("int-arg"); // This would actually be a lot more involved.
            Root_StringOption = new Option<string>("string-option");
            Root_BoolOption = new Option<bool>("bool-option");
            Root_StringProperty = new Option<string>("string-property");
            Root_CtorParam = new Option<bool>("ctor-param");
            Command.AddArgument(Root_IntArg);
            Command.AddOption(Root_StringOption);
            Command.AddOption(Root_BoolOption);
            Command.AddOption(Root_StringProperty);
            Command.Handler = new CommandSourceCommandHandler(InvokeAsync);
        }

        internal Argument<int> Root_IntArg { get; set; }
        internal Option<string> Root_StringOption { get; set; }
        internal Option<bool> Root_BoolOption { get; set; }
        internal Option<string> Root_StringProperty { get; set; }
        internal Option<bool> Root_CtorParam { get; set; }

        protected CliRoot GetNewInstance(BindingContext bindingContext)
        {
            return new CliRoot(GetValue(bindingContext, Root_CtorParam))
            {
                StringProperty = GetValue(bindingContext, Root_StringProperty)
            };
        }

        protected async Task<int> InvokeAsync(BindingContext bindingContext)
        {
            NewInstance = GetNewInstance(bindingContext);

            return await NewInstance.InvokeAsync(GetValue(bindingContext, Root_IntArg),
                                      GetValue(bindingContext, Root_StringOption),
                                      GetValue(bindingContext, Root_BoolOption));

        }

    }

}
