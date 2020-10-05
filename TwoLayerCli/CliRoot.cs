namespace TwoLayerCli
{
    public class CliRoot
    {
        private bool ctorParam;
        public CliRoot(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public int Find(int intArg, string stringOption, bool boolOption)
        { return 0; }

        public int List(bool boolOption)
        { return 0; }

    }
}
