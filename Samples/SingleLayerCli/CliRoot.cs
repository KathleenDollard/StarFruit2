using System.Threading.Tasks;

namespace SingleLayerCli
{
    public class CliRoot
    {
        // Usage: > cli-root <int> --ctor-param --string-property Fred --string-option Flintstone --bool-option
        private bool ctorParam;
        public CliRoot(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public async Task<int> InvokeAsync(int intArg, string stringOption, bool boolOption)
        { return await Task.FromResult(0); }

    }
}
