using StarFruit2.Common;
using System;
using System.CommandLine;
using System.CommandLine.Binding;
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
        protected CommandSource(Command command)
            => RootCommand = command;

        public static async Task<int> RunAsync<TCli>(string[] args)
        {
            return await Create<TCli>().Parse().RunAsync(args);
        }

        public static int Run<TCli>(string[] args)
        {
            return Create<TCli>().Parse().Run(args);
        }

        public static CommandSource Create<TCli>()
        {
            var cliType = typeof(TCli);
            var fullName = $"{cliType.FullName}CommandSource";
            var type = cliType.Assembly.GetType(fullName);
            Assert.NotNull(type);
            var ret = Activator.CreateInstance(type) as CommandSource;
            Assert.NotNull(ret); // throw here if generator is broken
            return ret;
        }

        public Command RootCommand { get; set; }

        public CommandSourceResult Parse()
        {
            return new CommandSourceResult(RootCommand);
        }


        protected TValue? GetValue<TValue>(BindingContext bindingContext,
                 Argument<TValue> argument)
       => bindingContext.ParseResult.CommandResult.Children
            .OfType<ArgumentResult>()
            .Where(a => a.Argument == argument)
            .Select(a => a.GetValueOrDefault<TValue>())
            .FirstOrDefault();

        protected TValue? GetValue<TValue>(BindingContext bindingContext,
                             Option<TValue> option)
            => bindingContext.ParseResult.CommandResult.Children
                        .OfType<OptionResult>()
                        .Where(o => o.Option == option)
                        .Select(o => o.GetValueOrDefault<TValue>())
                        .FirstOrDefault();

    }

    public abstract class CommandSource<T> : CommandSource
    {

        protected CommandSource(Command command)
           : base(command) { }

        protected T? NewInstance { get; set; } // exposed through CommandSourceResult

    }
}
