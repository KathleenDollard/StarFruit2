using StarFruit2;
using System;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    class Program
    {
        static int Main2(string[] args)
            => CommandSource.Run<CliRoot>(args);
    }

    class Program2
    {
        static int Main2(string[] args)
            => CommandSource.Create<CliRoot>().Parse(args).Run();
    }

    class Program3
    {
        static async Task<int> Main(string[] args)
        {
            var commandSource = CommandSource.Create<CliRoot>() as CliRootCommandSource;
            // modify System.CommandLine elements here
            commandSource.CtorParam.Argument.SetDefaultValue(DateTime.Today.Day);

            var commandSourceResult = commandSource.Parse(args);
            if (commandSourceResult.EarlyReturn)
            {
                return commandSourceResult.ExitCode;
            }
            // Property validation and modify if accessible
            switch (commandSourceResult)
            {
                case FindCommandSourceResult:
                // validation here and values can be changed
                case ListCommandSourceResult:
                default:
                    break;
            }
            // if you didn’t early return on help, etc, Execute does nothing
            var exitCode = await commandSourceResult.RunAsync();
            return exitCode;
        }

    }
}
