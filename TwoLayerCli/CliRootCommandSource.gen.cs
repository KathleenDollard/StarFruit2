using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace TwoLayerCli 
{
    public partial class CliRootCommandSource
    {
        private Argument<int> intArg;
        private Option<string> stringOption;
        private Option<bool> boolOption;
        private Option<string> stringProperty;
        private Option<bool> ctorParam;

        private Command rootCommand;

        public override Command GetCommand()
        {
            rootCommand = new Command("my-command");

            intArg = new Argument<int>("intArg"); // This would actually be a lot more involved.
            stringOption = new Option<string>("stringOption");
            boolOption = new Option<bool>("boolOption");
            stringProperty = new Option<string>("stringProperty");
            ctorParam = new Option<bool>("ctorParam");
            rootCommand.AddArgument(intArg);
            rootCommand.AddOption(stringOption);
            rootCommand.AddOption(boolOption);
            rootCommand.AddOption(stringProperty);
            rootCommand.Handler = CommandHandler.Create(InvokeFindAsync);

            return rootCommand;

        }
        public override async Task<int> RunAsync(string[] args) 
            => await rootCommand.InvokeAsync(args);

        public async Task<int> InvokeFindAsync(BindingContext bindingContext)
        {
            var commandResult = bindingContext.ParseResult.CommandResult;
            var newInstance = new CliRoot(GetValue(commandResult, ctorParam))
            {
                StringProperty = GetValue(commandResult, stringProperty)
            };
            return newInstance.Find(GetValue(commandResult, intArg),
                                    GetValue(commandResult, stringOption),
                                    GetValue(commandResult, boolOption));
        }

    }

}
