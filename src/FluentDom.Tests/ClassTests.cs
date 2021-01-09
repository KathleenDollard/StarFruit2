using FluentAssertions;
using FluentDom.Generator;
using System;
using Xunit;

namespace FluentDom.Tests
{
    public class ClassTests
    {
        private const string ClassName = "George";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_class_is_correct(bool useVB)
        {
            var cls = new Class(ClassName);
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @$"public class {ClassName}
                                    {{
                                    }}",
                VBGenerator => $@"Public Class {ClassName}
                                  End Class",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputClass(cls).GetOutput().NormalizeWhitespace();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_class_with_base_is_correct(bool useVB)
        {
            var cls = new Class(ClassName)
                        .Base("Fred");
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @$"public class {ClassName} : Fred
                                    {{
                                    }}",
                VBGenerator => $@"Public Class {ClassName}
                                      Inherits Fred
                                  End Class",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputClass(cls).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_code_with_member_is_correct(bool useVB)
        {
            var memberName = "AMember";
            var cls = new Class(ClassName)
                        .Member(new Property(memberName, "string"));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @$"public class {ClassName}
                                    {{
                                         public string {memberName} {{ get; set; }}
                                    }}",
                VBGenerator => $@"Public Class {ClassName}
                                      Public Property {memberName} As String
                                  End Class",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputClass(cls).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

    }
}
