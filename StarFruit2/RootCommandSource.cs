using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;

namespace StarFruit2
{
    public abstract class RootCommandSource<T> : RootCommandSource
    {
        protected RootCommandSource(Command command, CommandSourceBase parentCommandSource)
            : base(command, parentCommandSource) { }

        protected T? NewInstance { get; set; } // exposed through CommandSourceResult

    }

    public abstract class RootCommandSource : CommandSourceBase
    {
        protected RootCommandSource(Command command, CommandSourceBase parentCommandSource)
            : base(command, parentCommandSource) { }

        public CommandSourceBase? CurrentCommandSource { get; set; }
        public ParseResult? CurrentParseResult { get; set; }

        public CommandSourceResult Parse(string args)
        {
            // See CommandExtensions.GetInvocationPipeline for breakdown of implementation
            // This does not call the user's method, but an invoke in the CommandSource that sets the result
            // This allows the user to validate and manipulate the results prior to running their method
            // or sidestepping implementation if they just want the data
            var parser = new CommandLineBuilder(Command)
                               .UseDefaults()
                               .Build();

            var parseResult = parser.Parse(args);

            var exit = parseResult.Invoke();
            if (exit != 0)
            {
                // TODO: should we allow this?
            }
            return CurrentCommandSource switch
            {
                null => new EmptyCommandSourceResult(parseResult, this, exit),
                _ => CurrentCommandSource.GetCommandSourceResult(parseResult, exit)
            };
        }

        public async Task<int> RunAsync(string[] args)  
            => await RunAsync(string.Join("", args));

        public async Task<int> RunAsync(string args) 
            => await Parse(args).RunAsync();

        public int Run(string[] args) 
            => Run(string.Join("", args));

        public int Run(string args) 
            => Parse(args).Run();

        public int Repeat(string[] args) 
            => Repeat(string.Join(" ", args));

        public  int Repeat(string input)
        {
            try
            {
                var lastExitCode = 0;
                while (input is not null)
                {
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        lastExitCode = Parse(input).Run();
                    }
                    Console.WriteLine("Enter something:");
                    input = Console.ReadLine();
                }
                return lastExitCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }

        protected TValue GetValue<TValue>(BindingContext bindingContext,
                 Argument<TValue> argument)
       => bindingContext.ParseResult.CommandResult.Children
            .OfType<ArgumentResult>()
            .Where(a => a.Argument == argument)
            .Select(a => a.GetValueOrDefault<TValue>())
            .FirstOrDefault();

        protected TValue GetValue<TValue>(BindingContext bindingContext,
                             Option<TValue> option)
            => bindingContext.ParseResult.CommandResult.Children
                        .OfType<OptionResult>()
                        .Where(o => o.Option == option)
                        .Select(o => o.GetValueOrDefault<TValue>())
                        .FirstOrDefault();

    }
}
