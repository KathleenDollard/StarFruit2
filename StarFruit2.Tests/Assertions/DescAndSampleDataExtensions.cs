using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{
    public static class DescAndSampleDataExtensions
    {
        public static SourceToDescriptorCliAssertions Should(this CliDescriptor instance)
            => new SourceToDescriptorCliAssertions(instance);
        public static SourceToDescriptorCommandAssertions Should(this CommandDescriptor instance)
            => new SourceToDescriptorCommandAssertions(instance);
        public static SourceToDescriptorOptionAssertions Should(this OptionDescriptor instance)
            => new SourceToDescriptorOptionAssertions(instance);
        public static SourceToDescriptoArgumentAssertions Should(this ArgumentDescriptor instance)
            => new SourceToDescriptoArgumentAssertions(instance);
    }
}

