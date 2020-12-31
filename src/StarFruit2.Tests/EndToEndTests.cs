using Xunit;
using StarFruit2.Common.Descriptors;
using TestData;
using System;

namespace StarFruit2.Tests
{
    public class EndToEndTests
    {
        [Theory]
        [InlineData(typeof(EmptyTestData))]
        [InlineData(typeof(SingleArgStringTestData))]
        [InlineData(typeof(SingleIntArgTestData))]
        [InlineData(typeof(SingleBoolArgTestData))]
        public void Model_to_descriptor(Type type)
        {
            var testData = Activator.CreateInstance(type) as BaseTestData;

            CliDescriptor actual = Utils.GetCliFromFile(testData.ModelCodeFileName);

            actual.Should().Match(testData.CliDescriptor);
        }
    }
}

