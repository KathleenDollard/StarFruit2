using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using TestData;
using Xunit;

namespace StarFruit2.Tests
{
    public class GeneratorWithArgsAndOptionsTests
    {
        [Theory]
        [InlineData(typeof(SingleArgStringTestData))]
        [InlineData(typeof(SingleIntArgTestData))]
        [InlineData(typeof(SingleBoolArgTestData))]
        [InlineData(typeof(SingleStringOptionTestData))]
        [InlineData(typeof(SingleIntOptionTestData))]
        [InlineData(typeof(SingleBoolOptionTestData))]
        //[InlineData(typeof(SingleArgAndSingleOptionOptionTestData))]

        public void Include_commandCode_outputs_command_code(Type testDataType)
        {
            var testData = Activator.CreateInstance(testDataType) as BaseTestData;
            var cliDescriptor = testData.CliDescriptor;
            if (testData is null)
            {
                throw new InvalidOperationException("unexpected test data type");
            }
            var expected = testData.CommandDefinitionSourceCode;
            var actual = Utils.Normalize(CodeGenerator.GetSourceCode(cliDescriptor, CodeGenerator.Include.CommandCode));

            actual.Should().Be(Utils.Normalize(expected));
        }

        
    }
}
