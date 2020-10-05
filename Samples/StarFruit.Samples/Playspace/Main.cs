using System.Runtime.InteropServices.ComTypes;

namespace StarFruit.Samples.Playspace
{
    // This is all the code the user needs to write
    public class Program
    {
        public int Main(string[] args)
        {
            var commandSource = new CliRootCommandSource();
            var command = commandSource.GetCommand(); // you only need this if you have customizations
            return commandSource.Run(args);
        }
    }

    public class CliRoot
    {
        // Usage: > baz <int> --ctor-param --string-property Fred --string-option Flintstone --bool-option
        private bool ctorParam;
        public CliRoot(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public int Invoke(int intArg, string stringOption, bool boolOption)
        { return 0; }

    }

    public partial class CliRootCommandSource : CommandSource<CliRoot>
    {
    }
}
