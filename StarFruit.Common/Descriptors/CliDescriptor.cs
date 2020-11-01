using StarFruit.Common;

namespace StarFruit2.Common.Descriptors
{
    public class CliDescriptor
    {
        private string? generatedCommandSourceClassName;

        public string? GeneratedComandSourceNamespace { get; set; }

        // don't need this any more
        public string? GeneratedCommandSourceClassName
        {
            get
                => generatedCommandSourceClassName
                    ?? $"{GeneratedComandSourceNamespace?.AfterLast(".")}CommandSource";

            set => generatedCommandSourceClassName = value;
        }
        public CommandDescriptor? CommandDescriptor { get; set; }
    }
}
