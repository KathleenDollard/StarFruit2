using StarFruit2.Common;
using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;

namespace StarFruit2
{
    // <summary>
    // Create an empty partial class deriving from this with a generic type
    // argument that indicates the root of your CLI model.
    // Your CLI model is a plain object with properties, constructor parameters and
    // method parameters treated as arguments and options.
    // </summary> 
    // <remarks>
    // <para>
    // For a single layer invocation style CLI (baz [arguments] [options])
    // </para>
    // <list type="bullet">
    // Create a class with a single Invoke method. 
    // The name of the class does not matter. "CliRoot" is suggested.
    // </list> 
    // <para>
    // For a two layer invocation style CLI with no subcommands on other subcommands 
    // (baz [subcommands] [arguments] [options])
    // </para>
    // <list type="bullet">
    // Create a class with a method for each subcommand. 
    // The subcommand name will be the name of the method stylistically changed for Posix. 
    // The name of the class does not matter. "CliRoot" is suggested.
    // </list> 
    // <para>
    // For deeper invocation style CLIs with subcommands that have additional subcommands 
    // (baz [subcommands] [arguments] [options])
    // </para>
    // <list type="bullet">
    // Subcommands that contain invokable commands, create a class with a method for 
    // each subcommand. 
    // Class and method names will be used for subcommand names, stylistically changed for Posix. 
    // The name of the root class does not matter. "CliRoot" is suggested.
    // Using an inheritance hierarchy, group these subcommand classes in base classes
    // </list> 
    // <para>
    // While the approach of using an inheritance hierarchy may not initially feel intuitive, 
    // it provides easy access to values in parent subcommands, an easily defined entry point, 
    // natural guarantees against circularity, and an easier way to spot orphans.
    // </para>
    // <para>
    // To use your your invocation style CLI, create a Main method that looks something like this:
    // </para>
    // <code>
    // public class Program
    // {
    //     public int Main(string[] args)
    //     {
    //         var commandSource = new CliRootCommandSource();
    //         var command = commandSource.GetCommand(); // you only need this if you have customizations
    //         return commandSource.Run(args);
    //     }
    // }
    // </code>
    // <para>
    // You can run your app with the --help option to see the structure. Examples:
    // </para>
    // <code>
    // > [exeName] --help
    // > [exeName] [subcommandName] --help
    // > [exeName] [subcommandName] [subcommandName] --help
    // </code>
    // <para>
    // If the defaults are not what you like, several attributes are available to annotate your Cli model.
    // </para>
    // </remarks>

    public abstract class CommandSource
    {
        public static RootCommandSource Create<TCli>()
        {
            var cliType = typeof(TCli);
            var fullName = $"{cliType.FullName}CommandSource";
            var type = cliType.Assembly.GetType(fullName);
            Assert.NotNull(type);
            var ret = Activator.CreateInstance(type) as RootCommandSource;
            Assert.NotNull(ret); // throw here if generator is broken
            return ret;
        }

        public static async Task<int> RunAsync<TCli>(string[] args)
        {
            return await Create<TCli>().Parse(args).RunAsync();
        }

        public static int Run<TCli>(string[] args)
        {
            return Create<TCli>().Parse(args).Run();
        }

        public Command Command;

        protected internal virtual CommandSourceResult GetCommandSourceResult(ParseResult parseResult)
        {
            return new NotInvocableCommandSourceResult(parseResult);
        }
    }

    public abstract class RootCommandSource : CommandSource
    {
        protected RootCommandSource(Command command)
        => Command = command;

        public Command Command { get; set; }

        public CommandSource? CurrentCommandSource { get;  set; }

        public CommandSourceResult Parse(string[] args)
        {
            // See CommandExtensions.GetInvocationPipeline for breakdown of implementation
            // This does not call the user's method, but an invoke in the CommandSource that sets the result
            // This allows the user to validate and manipulate the results prior to running their method
            // or sidestepping implementation if they just want the data
            var parseResult = Command.Parse(args);
            var exit = parseResult.Invoke();
            if (exit != 0)
            {
                // TODO: should we allow this?
            }
            Assert.NotNull(CurrentCommandSource);
            var result = CurrentCommandSource.GetCommandSourceResult(parseResult);
            Assert.NotNull(result);
            return result;
        }


        protected TValue? GetValue<TValue>(BindingContext bindingContext,
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

    public abstract class RootCommandSource<T> : RootCommandSource
    {

        protected RootCommandSource(Command command)
           : base(command) { }

        protected T? NewInstance { get; set; } // exposed through CommandSourceResult

    }
}
