using FluentAssertions;
using Xunit;

namespace StarFruit2.Tests
{
    public class GeneralTests
    {
        [Fact]
        public void Normalized_strings_have_expected_whitespace()
        {
            var input = $@"   
      internal static class CommandSource
        {{

            var x     = 42;      
            
            var y = 53;

";

            var actual = Utils.Normalize(input);
            var expected = "internal static class CommandSource\n{\nvar x = 42;\nvar y = 53;";

            actual.Should().Be(expected);
        }
    }
}
