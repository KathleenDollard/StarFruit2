using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StarFruit2.Tests
{
    public class XmlCommentDescriptionTests
    {
        private readonly SyntaxTree syntaxTree;
        private readonly SemanticModel semanticModel;

        public XmlCommentDescriptionTests()
        {
            (syntaxTree, semanticModel) = GetSemanticModel("XmlCommentTestCode.cs");
        }
        [Fact]
        public void Can_get_property_description_from_xml_comment()
        {
            var node = syntaxTree.GetRoot()
                                 .DescendantNodes()
                                 .OfType<PropertyDeclarationSyntax>()
                                 .Where(p => p.Identifier.ToString() == "MyProperty")
                                 .Single();
            var symbol = semanticModel.GetDeclaredSymbol(node);
            var actual = DescriptionFromXmlComments.XmlComments(symbol);

            actual.Should().Be("Description for MyProperty");
        }

        [Fact]
        public void Can_get_class_description_from_xml_comment()
        {
            var node = syntaxTree.GetRoot()
                                 .DescendantNodes()
                                 .OfType<ClassDeclarationSyntax>()
                                 .Where(p => p.Identifier.ToString() == "XmlCommentTestCode")
                                 .Single();
            var symbol = semanticModel.GetDeclaredSymbol(node);
            var actual = DescriptionFromXmlComments.XmlComments(symbol);

            actual.Should().Be("Description for XmlCommentTestCode");
        }

        [Fact]
        public void Can_get_method_description_from_xml_comment()
        {
            var node = syntaxTree.GetRoot()
                                 .DescendantNodes()
                                 .OfType<MethodDeclarationSyntax>()
                                 .Where(p => p.Identifier.ToString() == "MyMethod")
                                 .Single();
            var symbol = semanticModel.GetDeclaredSymbol(node);
            var actual = DescriptionFromXmlComments.XmlComments(symbol);

            actual.Should().Be("Description for MyMethod");
        }

        [Fact]
        public void Can_get_parameter_descriptions_from_xml_comment()
        {
            var node = syntaxTree.GetRoot()
                                 .DescendantNodes()
                                 .OfType<MethodDeclarationSyntax>()
                                 .Where(p => p.Identifier.ToString() == "MyMethod")
                                 .Single();
            var methodSymbol = semanticModel.GetDeclaredSymbol(node);
            var firstParam = methodSymbol.Parameters.Where(p => p.Name == "firstParam");
            var secondParam = methodSymbol.Parameters.Where(p => p.Name == "secondParam");
            var actualFirst = DescriptionFromXmlComments.XmlComments(firstParam.Single());
            var actualSecond = DescriptionFromXmlComments.XmlComments(secondParam.Single());

            actualFirst.Should().Be("Description for firstParam");
            actualSecond.Should().Be("Description for secondParam");
        }


        private static (SyntaxTree syntaxTree, SemanticModel semanticModel) GetSemanticModel(string fileName)
        {
            var code = File.ReadAllText($"{fileName}");
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = GetCompilation(tree);
            var model = compilation.GetSemanticModel(tree);
            return (tree, model);
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
