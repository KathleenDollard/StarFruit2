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

            var cSharpExpected = @"left = M()";
            var vbExpected = cSharpExpected;

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_Assign_with_expression_as_string_correct()
        {
            var expression = Expression.Assign("left", "right");

            var cSharpExpected = @"left = right";
            var vbExpected = cSharpExpected;

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_AssignVar_correct()
        {
            var expression = Expression.AssignVar("left", "myType", Expression.MethodCall("M"));

            var cSharpExpected = "myType left = M()";
            var vbExpected = "Dim left As myType = M()";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_MethodCall_correct()
        {
            var expression = Expression.MethodCall("M");

            var cSharpExpected = @"M()";
            var actual = expression.CSharpString();

            actual.Should().Be(cSharpExpected);
        }

        [Fact]
        public void Generated_MethodCall_with_expression_args_correct()
        {
            var expression = Expression.MethodCall("M", Expression.MethodCall("X"), Expression.MethodCall("Y"));

            var cSharpExpected = @"M(X(), Y())";
            var vbExpected = cSharpExpected;

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_MethodCall_with_string_args_correct()
        {
            var expression = Expression.MethodCall("M", "x", "y", "z");

            var cSharpExpected = @"M(x, y, z)";
            var vbExpected = cSharpExpected;

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact(Skip = "TBD")]
        public void MultilineLambda()
        {
            var expression = Expression.Assign("left", "right");

            var cSharpExpected = @"";
            var vbExpected = cSharpExpected;

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_NewObject_correct()
        {
            var expression = Expression.NewObject(new TypeRep("myType"));

            var cSharpExpected = @"new myType()";
            var vbExpected = @"New myType()";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_NewObject_with_type_as_string_correct()
        {
            var expression = Expression.NewObject("myType");

            var cSharpExpected = @"new myType()";
            var vbExpected = @"New myType()";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_NewObject_with_args_as_expressions_correct()
        {
            var expression = Expression.NewObject("myType", Expression.MethodCall("X"), Expression.MethodCall("Y"));

            var cSharpExpected = @"new myType(X(), Y())";
            var vbExpected = @"New myType(X(), Y())";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_NewObject_with_args_as_strings_correct()
        {
            var expression = Expression.NewObject("myType", "x", "y", "z");

            var cSharpExpected = @"new myType(x, y, z)";
            var vbExpected = @"New myType(x, y, z)";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_this_correct()
        {
            var expression = Expression.This();

            var cSharpExpected = "this";
            var vbExpected = "Me";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Generated_base_correct()
        {
            var expression = Expression.Base();

            var cSharpExpected = "base";
            var vbExpected = "MyBase";

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        // This is also tested in GeneralTests via TypeRep tests
        public void Generated_Value_correct()
        {
            var expression = Expression.Value("value");

            var cSharpExpected = @"""value""";
            var vbExpected = cSharpExpected;

            var actualCSharp = expression.CSharpString();
            var actualVB = expression.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }


    }
}
