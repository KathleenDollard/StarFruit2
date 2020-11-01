using Xunit;
using FluentAssertions;
using StarFruit2.Generator;
using System.Collections.Generic;

namespace StarFruit2.Tests
{
    public class GeneratorExtensionTests
    {
        [Fact]
        public void EndStatement_ReturnsSameStringIEndsInSemicolon()
        {
            var expression = "new Object();";

            var result = expression.EndStatement();

            result.Should().Be(expression);
        }

        [Fact]
        public void EndStatement_ReturnsStringWithSemiColonIfNoSemicolon()
        {
            var expression = "new Object()";
            var expectedData = "new Object();";

            var result = expression.EndStatement();

            result.Should().Be(expectedData);
        }

        [Fact]
        public void EndStatement_ReturnsSameListIfEndsInSemicolon()
        {
            var expression = new List<string> { "new Object", "{", "Prop = 42", "};" };

            var result = expression.EndStatement();

            result.Should().BeEquivalentTo(expression);
        }

        [Fact]
        public void EndStatement_ReturnsSameListWithSemicolonIfNoSemicolon()
        {
            var expression = new List<string> { "new Object", "{", "Prop = 42", "}" };
            var expectedData = new List<string> { "new Object", "{", "Prop = 42", "};" };

            var result = expression.EndStatement();

            result.Should().BeEquivalentTo(expectedData);
        }
    }
}
