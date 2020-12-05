using FluentAssertions;
using FluentDom.Generator;
using Xunit;

namespace FluentDom.Tests
{
    public class ExpressionTests
    {
        [Fact]
        public void Generated_Assign_correct()
        {
            var expression = Expression.Assign("left", Expression.MethodCall("M"));

            var expected = @"left = M()";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_Assign_with_expression_as_string_correct()
        {
            var expression = Expression.Assign("left", "right");

            var expected = @"left = right";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_AssignVar_correct()
        {
            var expression = Expression.AssignVar("left", "type", Expression.MethodCall("M"));

            var expected = @"type left = M()";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_MethodCall_correct()
        {
            var expression = Expression.MethodCall("M");

            var expected = @"M()";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_MethodCall_with_expression_args_correct()
        {
            var expression = Expression.MethodCall("M", Expression.MethodCall("X"), Expression.MethodCall("Y"));

            var expected = @"M(X(), Y())";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_MethodCall_with_string_args_correct()
        {
            var expression = Expression.MethodCall("M", "x", "y", "z");

            var expected = @"M(x, y, z)";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact(Skip = "TBD")]
        public void MultilineLambda()
        {
            var expression = Expression.Assign("left", "right");

            var expected = @"";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_NewObject_correct()
        {
            var expression = Expression.NewObject(new TypeRep("type"));

            var expected = @"new type()";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_NewObject_with_type_as_string_correct()
        {
            var expression = Expression.NewObject("type");

            var expected = @"new type()";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_NewObject_with_args_as_expressions_correct()
        {
            var expression = Expression.NewObject("type", Expression.MethodCall("X"), Expression.MethodCall("Y"));

            var expected = @"new type(X(), Y())";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_NewObject_with_args_as_strings_correct()
        {
            var expression = Expression.NewObject("type", "x", "y", "z");

            var expected = @"new type(x, y, z)";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_this_correct()
        {
            var expression = Expression.This();

            var expected = @"this";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_base_correct()
        {
            var expression = Expression.Base();

            var expected = @"base";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        // This is also tested in GeneralTests via TypeRep tests
        public void Generated_Value_correct()
        {
            var expression = Expression.Value("value");

            var expected = @"""value""";
            var actual = expression.CSharpString();

            actual.Should().Be(expected);
        }


    }
}
