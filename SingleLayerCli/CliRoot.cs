namespace SingleLayerCli
{
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
}
