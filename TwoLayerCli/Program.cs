using StarFruit2;
using System;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    class Program
    {
        static async Task<int> Main(string[] args)
             => await new CliRootCommandSource().RunAndExitAsync(args);
    }

    public partial class CliRootCommandSource : RootCommandSource<CliRoot>
    { }
}
