using StarFruit2;
using System;

namespace SingleLayerCli
{
    class Program
    {
        static int Main(string[] args)
        {
            var commandSource = new CliRootCommandSource();
            var command = commandSource.GetCommand(); // you only need this if you have customizations
            return commandSource.Run(args);
        }
    }

    public partial class CliRootCommandSource : CommandSource<CliRoot>
    { }
}
