using Microsoft.CodeAnalysis;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace StarFruit2.Tests
{
    public class XmlCommentDescriptionTests
    {
        private readonly SemanticModel semanticModel;

        public XmlCommentDescriptionTests()
        {
           semanticModel = Utils.GetSemanticModel("XmlCommentTestCode.cs");
        }

        [Fact]
        public void Can_get_property_description_from_xml_comment()
        {
            var symbol = Utils.GetPropertySymbol(semanticModel, "MyProperty");
            symbol.Should().NotBeNull();

            var actual = DescriptionFromXmlComments.XmlComments(symbol!);

            actual.Should().Be("Description for MyProperty");
        }

        [Fact]
        public void Can_get_class_description_from_xml_comment()
        {
            var symbol = Utils.GetClassSymbol(semanticModel, "MyProperty");
            symbol.Should().NotBeNull();

            var actual = DescriptionFromXmlComments.XmlComments(symbol!);

            actual.Should().Be("Description for XmlCommentTestCode");
        }

        [Fact]
        public void Can_get_method_description_from_xml_comment()
        {
            var symbol = Utils.GetMethodSymbol(semanticModel, "MyMethod");
            symbol.Should().NotBeNull();

            var actual = DescriptionFromXmlComments.XmlComments(symbol!);

            actual.Should().Be("Description for MyMethod");
        }

        [Fact]
        public void Can_get_parameter_descriptions_from_xml_comment()
        {
            var methodSymbol = Utils.GetMethodSymbol(semanticModel, "MyMethod");
            methodSymbol.Should().NotBeNull();

            var firstParam = methodSymbol!.Parameters.Where(p => p.Name == "firstParam");
            var secondParam = methodSymbol.Parameters.Where(p => p.Name == "secondParam");
            var actualFirst = DescriptionFromXmlComments.XmlComments(firstParam.Single());
            var actualSecond = DescriptionFromXmlComments.XmlComments(secondParam.Single());

            actualFirst.Should().Be("Description for firstParam");
            actualSecond.Should().Be("Description for secondParam");
        }
    }
}
