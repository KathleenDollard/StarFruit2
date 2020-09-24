using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{
    public static class DescAndSampleDataExtensions
    {
        public static SyntaxToCommandSampleDataAssertions Should(this CommandDescriptor instance)
            => new SyntaxToCommandSampleDataAssertions(instance);

        public static SyntaxToOptionSampleDataAssertions Should(this OptionDescriptor instance)
            => new SyntaxToOptionSampleDataAssertions(instance);
        public static SyntaxToArgumentSampleDataAssertions Should(this ArgumentDescriptor instance)
            => new SyntaxToArgumentSampleDataAssertions(instance);
    }
}

