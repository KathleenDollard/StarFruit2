using System.Threading.Tasks;

namespace MultiLayerCli
{
    public class CliRoot
    { }

    public class Tool : CliRoot
    {
        private bool ctorParam;
        public Tool(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public async Task<int> FindAsync(int intArg, string stringOption, bool boolOption)
        { return await Task.FromResult(0); }

        public async Task<int> ListAsync(bool boolOption)
        { return await Task.FromResult(0); }

    }
}