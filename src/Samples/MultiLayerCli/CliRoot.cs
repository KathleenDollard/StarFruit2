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

        public  int Find(int intArg, string stringOption, bool boolOption)
        { return  0; }

        public  int ListAsync(bool boolOption)
        { return 0; }

    }
}