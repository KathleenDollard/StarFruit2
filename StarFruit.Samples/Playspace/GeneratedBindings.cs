using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using StarFruit2;

namespace StarFruit.Samples.Playspace
{
    public class Program
    {
        public int Main2(string[] args)
        {
            var CommandSource = new BazCommandSource();
            var command = CommandSource.GetCommand(); // you only need this if you have customizations
            return CommandSource.Run(args);
        }
    }

    // > baz <int> --ctor-param --string-property Fred --string-option Flintstone --bool-option
    public class Baz
    {
        private bool ctorParam;
        public Baz(bool ctorParam) 
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public int Invoke(int intArg, string stringOption, bool boolOption)
        { return 0; }

    }


    public partial class BazCommandSource : ICommandSource<Baz>
    { }


    public partial class BazCommandSource : ICommandSource
    {
        private Argument<int> intArg;
        private Option<string> stringOption;
        private Option<bool> boolOption;
        private Option<string> stringProperty;
        private Option<bool> ctorParam;

        private Command rootCommand;

        public Command GetCommand()
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
        public int Run(string[] args)
        {
            var result = rootCommand.Parse(args);
            var commandResult = result.CommandResult;

            var newInstance = new Baz(GetValue(commandResult, ctorParam))
            {
                StringProperty = GetValue(commandResult, stringProperty)
            };

            return newInstance.Invoke(GetValue(commandResult, intArg),
                                      GetValue(commandResult, stringOption),
                                      GetValue(commandResult, boolOption));

        }

    }

    public partial class BazCommandSource
    {
        public T GetValue<T>(CommandResult commandResult,
                             Argument<T> argument) 
            => commandResult.Children
                        .OfType<ArgumentResult>()
                        .Where(a => a.Argument == argument)
                        .Select(a => a.GetValueOrDefault<T>())
                        .FirstOrDefault();

        public T GetValue<T>(CommandResult commandResult,
                             Option<T> option) 
            => commandResult.Children
                        .OfType<OptionResult>()
                        .Where(o => o.Option == option)
                        .Select(o => o.GetValueOrDefault<T>())
                        .FirstOrDefault();
    }
}
