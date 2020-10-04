using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using Xunit;
using StarFruit2.Common.Descriptors;
using TestData;
using System;
using FluentAssertions;
using Starfruit2_B;
using Microsoft.CodeAnalysis;
using StarFruit2.Generator;

namespace StarFruit2.Tests
{
    public class EndToEndTests
    {
        [Theory]
        [InlineData(typeof(EmptyTestData))]
        public void Model_to_descriptor(Type type)
        {
            var testData = Activator.CreateInstance(type) as BaseTestData;

            CliDescriptor actual = GetCli(testData.ModelCodeFileName );

            actual.Should().Match(testData.CliDescriptor);
        }

        [Theory]
        [InlineData(typeof(EmptyTestData))]
        public void Descriptor_to_source(Type type)
        {
            var testData = Activator.CreateInstance(type) as BaseTestData;

            var actual = CodeGenerator.GetSourceCode(testData.CliDescriptor, CodeGenerator.Include.CommandCode );

            actual.Should().NotBeNullOrEmpty();
            var normActual = Utils.Normalize(actual);
            var normExpected = Utils.Normalize(testData.GeneratedSource);
            normActual.Should().Be(normExpected);
        }

        private static CliDescriptor GetCli(string fileName)
        {
            var code = File.ReadAllText($"TestData/{fileName}");
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = GetCompilation(tree);
            var rootCommand = tree.GetRoot().DescendantNodes()
                               .OfType<ClassDeclarationSyntax>()
                               .Single();

            var cli = RoslyDescriptorMakerFactory.CreateCliDescriptor(rootCommand, compilation);
            return cli;
        }

        private static CSharpCompilation GetCompilation(SyntaxTree tree)
        {
            MetadataReference mscorlib =
                       MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            MetadataReference[] references = { mscorlib };

            return CSharpCompilation.Create("TransformationCS",
                                            new SyntaxTree[] { tree },
                                            references,
                                            new CSharpCompilationOptions(
                                                    OutputKind.ConsoleApplication));
        }

    }
}

