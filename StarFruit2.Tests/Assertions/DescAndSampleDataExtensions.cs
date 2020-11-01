using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{
    public static class DescAndSampleDataExtensions
    {
        public static CliDescriptorAssertions Should(this CliDescriptor instance)
            => new CliDescriptorAssertions(instance);
        public static CommandDescriptorAssertions Should(this CommandDescriptor instance)
            => new CommandDescriptorAssertions(instance);
        public static OptionDescriptorAssertions Should(this OptionDescriptor instance)
            => new OptionDescriptorAssertions(instance);
        public static ArgumentDescriptorAssertions Should(this ArgumentDescriptor instance)
            => new ArgumentDescriptorAssertions(instance);
    }
}

