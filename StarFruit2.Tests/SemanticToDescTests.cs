using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Starfruit2_B;
using System.IO;
using System.Linq;
using Xunit;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{
    public class SemanticToDescTests
    {
        [Theory]
        [InlineData("EmptySampleData.cs")]
        [InlineData("SingleArgumentSampleData.cs")]
        [InlineData("SingleOptionWithArgumentSampleData.cs")]
        [InlineData("SeveralOptionsAndArgumentsSampleData.cs")]
        public void Parse_class(string fileName)
        {
            var sample = FileSampleData.Samples.Data
                            .First(x => x.FileName == fileName);
            CommandDescriptor command = GetCommand(fileName);

            command.Should().Match(sample.CommandExpectedData);
        }

        private static Common.Descriptors.CommandDescriptor GetCommand(string fileName)
        {
            var code = File.ReadAllText($"FileSampleData/{fileName}");
            var tree = CSharpSyntaxTree.ParseText(code);
            var maker = new SyntaxTreeDescriptorMaker();

            var command = maker.GetCommandDescriptor(tree.GetRoot().DescendantNodes()
                               .OfType<ClassDeclarationSyntax>()
                               .Single());
            return command;
        }

    }
}

