using StarFruit2;
using System.Threading.Tasks;

namespace SingleLayerCli
{
    class Program
    {
        static async Task<int> Main(string[] args)
             => await new CliRootCommandSource().RunAndExitAsync(args);

        static async Task<int> MainWithCustomization(string[] args)
        {
            var commandSource = new CliRootCommandSource();
            var command = commandSource.Command; // you only need this if you have customizations
            // result is used for accessing instance, special handling for errors, etc
            var result = await commandSource.RunAsync(args);
            return result.ExitCode;
        }
    }

    public partial class CliRootCommandSource : RootCommandSource<CliRoot>
    { }
}
