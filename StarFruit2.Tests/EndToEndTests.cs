using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using Xunit;
using StarFruit2.Common.Descriptors;
using TestData;
using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using StarFruit2.Generator;
using Starfruit2;

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

        [Theory(Skip ="Old generator")]
        [InlineData(typeof(EmptyTestData))]
        public void Descriptor_to_source(Type type)
        {
            var testData = Activator.CreateInstance(type) as BaseTestData;

            var actual = CodeGenerator.GetSourceCode(testData.CliDescriptor, CodeGenerator.Include.CommandCode);

            actual.Should().NotBeNullOrEmpty();
            var normActual = Utils.Normalize(actual);
            var normExpected = Utils.Normalize(testData.GeneratedSource);
            normActual.Should().Be(normExpected);
        }


    }
}

