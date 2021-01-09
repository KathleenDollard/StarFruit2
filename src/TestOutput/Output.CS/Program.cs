using StarFruit2;
using System;
using System.Threading.Tasks;

namespace StarFruit.FromClass
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            //var x = TwoLayerCli.
            RepeatMain(args);
            return 0;
            //Main_Simplest(args);
            //Main_with_CliModifications(args);
            //// Main_with_direct_execution(args);
            //return await Main_what_really_happens(args);
        }

        private static void RepeatMain(string[] args)
        {
            var input = string.Join("", args);
            while (input is not null)
            {
                CommandSource.Run<CliRoot>(input);
                Console.WriteLine("Enter something:");
                input = Console.ReadLine();
            }
        }

        //static int Main_Simplest(string[] args)
        //    => CommandSource.Run<CliRoot>(args);

        //static int Main_with_CliModifications(string[] args)
        //{
        //    return 0;
        //    var commandSource = CommandSource.Create<CliRoot>() as CliRootCommandSource;
        //    //_ = commandSource ?? throw new InvalidOperationException("CommandSource not found or unexpected type");

        //    //// Modify commandSource.Command CLI tree
        //    //// For example, might need complex default, or rarely to add additional elements, 
        //    //// Note: The CommandSource structure makes it easy to find what you want to change
        //    //commandSource.Find.IntArg.SetDefaultValue(DateTime.Now.DayOfYear);

        //    //return commandSource.Parse(args).Run();
        //}

        ////static int Main_with_direct_execution(string[] args)
        ////{
        ////             return CommandSource.Run<CliRoot>(args, Run);

        ////    static int Run(CliRootCommandSourceResult result)
        ////    {
        ////        return result switch
        ////        {
        ////            FindCommandSourceResult => result.CreateInstance().FindAsync(), // validation here and values can be changed
        ////            ListCommandSourceResult => List(), // 
        ////            _ => 1
        ////        };
        ////    }
        ////}

        //static async Task<int> Main_what_really_happens(string[] args)
        //{
        //    return 0;
        //    var commandSource = CommandSource.Create<CliRoot>() as CliRootCommandSource;
        //    // modify System.CommandLine elements here

        //    var commandSourceResult = commandSource.Parse(string.Join("", args));
        //    if (commandSourceResult.EarlyReturn)
        //    {
        //        return commandSourceResult.ExitCode;
        //    }
        //    var cliRootResult = commandSourceResult as CliRootCommandSourceResult;
        //    // Property validation and modify if accessible
        //    switch (cliRootResult)
        //    {
        //        case FindCommandSourceResult:
        //        // validation here and values can be changed
        //        case ListCommandSourceResult:
        //        default:
        //            break;
        //    }
        //    // if you didn’t early return on help, etc, Execute does nothing
        //    var exitCode = await commandSourceResult.RunAsync();
        //    return exitCode;
        //}

    }
}
