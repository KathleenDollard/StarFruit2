using FluentAssertions;
using FluentDom.Generator;
using Xunit;

namespace FluentDom.Tests
{
    public class ClassTests
    {
        private const string ClassName = "George";


        [Fact]
        public void Generated_class_is_correct()
        {
            var cls = new Class(ClassName);

            var expected = @$"public class {ClassName}
{{
}}
";

            var actual = new CSharpGenerator().OutputClass(cls).GetOutput().NormalizeWhitespace();

            actual.Should().Be(expected.NormalizeWhitespace());
        }

        [Fact]
        public void Generated_class_with_base_is_correct()
        {
            var cls = new Class(ClassName)
                        .Base("Fred");

            var expected = @$"public class {ClassName} : Fred
{{
}}
";
            var actual = new CSharpGenerator().OutputClass(cls).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_code_with_member_is_correct()
        {
            var memberName = "AMember";
            var cls = new Class(ClassName)
                        .Member(new Property(memberName, "string"));
            var expected = @$"public class {ClassName}
{{
   public string AMember {{ get; set; }}
}}
";

            var actual = new CSharpGenerator().OutputClass(cls).GetOutput();

            actual.Should().Be(expected);
        }

    }
}
