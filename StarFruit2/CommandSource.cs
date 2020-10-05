using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;

namespace StarFruit2
{
    /// <summary>
    /// Create an empty partial class deriving from this with a generic type
    /// argument that indicates the root of your CLI model.
    /// Your CLI model is a plain object with properties, constructor parameters and
    /// method parameters treated as arguments and options.
    /// </summary> 
    /// <remarks>
    /// <para>
    /// For a single layer invocation style CLI (baz [arguments] [options])
    /// </para>
    /// <list type="bullet">
    /// Create a class with a single Invoke method. 
    /// The name of the class does not matter. "CliRoot" is suggested.
    /// </list> 
    /// <para>
    /// For a two layer invocation style CLI with no subcommands on other subcommands 
    /// (baz [subcommands] [arguments] [options])
    /// </para>
    /// <list type="bullet">
    /// Create a class with a method for each subcommand. 
    /// The subcommand name will be the name of the method stylistically changed for Posix. 
    /// The name of the class does not matter. "CliRoot" is suggested.
    /// </list> 
    /// <para>
    /// For deeper invocation style CLIs with subcommands that have additional subcommands 
    /// (baz [subcommands] [arguments] [options])
    /// </para>
    /// <list type="bullet">
    /// Subcommands that contain invokable commands, create a class with a method for 
    /// each subcommand. 
    /// Class and method names will be used for subcommand names, stylistically changed for Posix. 
    /// The name of the root class does not matter. "CliRoot" is suggested.
    /// Using an inheritance hierarchy, group these subcommand classes in base classes
    /// </list> 
    /// <para>
    /// While the approach of using an inheritance hierarchy may not initially feel intuitive, 
    /// it provides easy access to values in parent subcommands, an easily defined entry point, 
    /// natural guarantees against circularity, and an easier way to spot orphans.
    /// </para>
    /// <para>
    /// To use your your invocation style CLI, create a Main method that looks something like this:
    /// </para>
    /// <code>
    /// public class Program
    /// {
    ///     public int Main(string[] args)
    ///     {
    ///         var commandSource = new CliRootCommandSource();
    ///         var command = commandSource.GetCommand(); // you only need this if you have customizations
    ///         return commandSource.Run(args);
    ///     }
    /// }
    /// </code>
    /// <para>
    /// You can run your app with the --help option to see the structure. Examples:
    /// </para>
    /// <code>
    /// > [exeName] --help
    /// > [exeName] [subcommandName] --help
    /// > [exeName] [subcommandName] [subcommandName] --help
    /// </code>
    /// <para>
    /// If the defaults are not what you like, several attributes are available to annotate your Cli model.
    /// </para>
    /// </remarks>
    public abstract class CommandSource<T>
    {
        public abstract Command GetCommand();
        public abstract Task<int> RunAsync(string[] args);

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
