using StarFruit.Common;

namespace StarFruit2.Common.Descriptors
{
    public class CliDescriptor
    {
        private string? generatedCommandSourceClassName;

        public string? GeneratedComandSourceNamespace { get; set; }
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
