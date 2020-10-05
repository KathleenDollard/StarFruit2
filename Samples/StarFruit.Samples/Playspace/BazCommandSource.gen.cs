using System.CommandLine;
using System.CommandLine.Parsing;

namespace StarFruit.Samples.Playspace
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

            return rootCommand;

        }
        public override int Run(string[] args)
        {
            var result = rootCommand.Parse(args);
            var commandResult = result.CommandResult;

            var newInstance = new CliRoot(GetValue(commandResult, ctorParam))
            {
                StringProperty = GetValue(commandResult, stringProperty)
            };

            return newInstance.Invoke(GetValue(commandResult, intArg),
                                      GetValue(commandResult, stringOption),
                                      GetValue(commandResult, boolOption));

        }

    }

}
