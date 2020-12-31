using StarFruit2.Common;
using System;
using System.Linq;
using System.Reflection;
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

    public static class CommandSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <remarks>
        /// Method entry points have two limitations not present on classes: 
        /// * The entry MethodName + CommandSource must be unique, usually not a problem, and
        /// * The assembly containing the entry method must be the entry assembly for the process (the startup assembly)
        /// </remarks>
        /// <returns></returns>
        public static RootCommandSource Create(string methodName)
        {
            var commandSourceName = $"{methodName}CommandSource";
            var assembly = Assembly.GetEntryAssembly();
            var type = assembly.GetTypes()
                               .Where(x=>x.Name == commandSourceName)
                               .FirstOrDefault();
            Assert.NotNull(type);
            var ret = Activator.CreateInstance(type) as RootCommandSource;
            Assert.NotNull(ret); // throw here if generator is broken
            return ret;
        }

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
            return await RunAsync<TCli>(string.Join("", args));
        }

        public static async Task<int> RunAsync<TCli>(string args)
        {
            return await Create<TCli>().Parse(args).RunAsync();
        }

        public static int Run<TCli>(string[] args)
        {
            return Run<TCli>(string.Join("", args));
        }

        public static int Run<TCli>(string args)
        {
            return Create<TCli>().Parse(args).Run();
        }

        public static int Repeat<TCli>(string[] args)
        {
            return Repeat<TCli>(string.Join(" ", args));
        }

        public static int Repeat<TCli>(string input)
        {
            try
            {
                var lastExitCode = 0;
                var rootCommandSource = Create<TCli>();
                while (input is not null)
                {
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        lastExitCode = rootCommandSource.Parse(input).Run();
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
    }
}
