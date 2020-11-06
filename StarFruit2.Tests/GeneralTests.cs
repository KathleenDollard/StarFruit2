using FluentAssertions;
using System;
using System.Linq;
using TestData;
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

        [Theory]
        [InlineData(typeof(EmptyTestData),0)]
        [InlineData(typeof(SingleArgStringTestData),1)]
        [InlineData(typeof(SingleIntArgTestData),1)]
        [InlineData(typeof(SingleBoolArgTestData),1)]
        [InlineData(typeof(SeveralOptionsAndArgumentsTestData), 4)]
        [InlineData(typeof(SeveralSubCommandsTestData), 4)]
        public void CliDescriptor_Descendants_captures_options_and_arguments(Type testDataType, int expectedCount)
        {
            var testData = Activator.CreateInstance(testDataType) as BaseTestData;
            var cliDescriptor = testData.CliDescriptor;
            if (testData is null)
            {
                throw new InvalidOperationException("unexpected test data type");
            }

            var descedants = cliDescriptor.Descendants;

            descedants.Count().Should().Be(expectedCount);


        }

        [Fact]
        public void CliDescriptor_Descendants_captures_subcommands()
        {

        }

    }
}
