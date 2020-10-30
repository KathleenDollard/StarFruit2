using StarFruit2;
using System;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    class Program
    {
        static int Main(string[] args)
            => CommandSource.Run<CliRoot>(args);
    }

    class Program
    {
        static int Main(string[] args)
            => CommandSource.Create<CliRoot>().Parse().Run(args);
    }

    class Program2
    {
        static int Main(string[] args)
        {
            var commandSource = CommandSource.Create<CliRoot>() as CliRootCommandSource;
            // modify System.CommandLine elements here
            var commandSourceResult = commandSource.Parse(args);
            if (commandSourceResult.EarlyReturn)
            {
                return commandSourceResult.ExitCode;
            }
            // Property validation and modify if accessible
            switch (CommandSourceResult.MethodParams)
            {
                case Find_MethodParams:
                // validation here and values can be changed
                case List_MethodParams:
                default:
                    break;
            }
            // if you didn’t early return on help, etc, Execute does nothing
            var exitCode = CommandSourceResult.Run();
            return exitCode;
        }
    }
}
