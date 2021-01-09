using System.Linq;
using Xunit;
using FluentAssertions;
using FluentDom.Generator;

namespace FluentDom.Tests
{
    public class NamespaceTests
    {
        private readonly string CodeNamespace = "CodeNamespace";
        private readonly string FirstNamespace = "FirstNamespace";
        private readonly string SecondNamespace = "Second.Namespace";
        private readonly string ThirdNamespace = "Third.Yes.No.Namespace";
        private readonly string BadNamespace = "First-Namespace";
        private readonly string FirstNamespaceAlias = "First";
        private readonly string EmptyNamespace = "";
        private readonly string? NullNamespace = null;


        [Fact]
        public void Can_create_namespace_with_name()
        {
            var code = Code.Create(CodeNamespace)
                         .Usings(SecondNamespace);
            CheckCode(code, CodeNamespace, 1);
            CheckUsing(code.UsingStore.First(), SecondNamespace);
        }

        [Fact]
        public void Can_create_namespace_with_name_and_alias()
        {
            var code = Code.Create(CodeNamespace)
                         .Usings(new Using(FirstNamespace, FirstNamespaceAlias));
            CheckCode(code, CodeNamespace, 1);
            CheckUsing(code.UsingStore.First(), FirstNamespace, FirstNamespaceAlias);
        }

        [Fact]
        public void Can_create_namespace_with_name_and_using_static()
        {
            var code = Code.Create(CodeNamespace)
                         .Usings(new Using(FirstNamespace, FirstNamespaceAlias, true));
            CheckCode(code, CodeNamespace, 1);
            CheckUsing(code.UsingStore.First(), FirstNamespace, FirstNamespaceAlias, true);
        }

        [Fact]
        public void Can_create_namespace_with_several_usings()
        {
            var code = Code.Create(CodeNamespace)
                         .Usings(
                              new Using(FirstNamespace, FirstNamespaceAlias, true),
                              SecondNamespace,
                              ThirdNamespace);
            CheckCode(code, CodeNamespace, 3);
            CheckUsing(code.UsingStore.First(), FirstNamespace, FirstNamespaceAlias, true);
            CheckUsing(code.UsingStore.Skip(1).First(), SecondNamespace);
            CheckUsing(code.UsingStore.Skip(2).First(), ThirdNamespace);
        }

        [Fact]
        public void Namespace_with_empty_or_bad_name_does_not_crash()
        {
            var code = Code.Create(CodeNamespace)
                         .Usings(
                             "",
                             BadNamespace);
            CheckCode(code, CodeNamespace, 2);
            CheckUsing(code.UsingStore.First(), "");
            CheckUsing(code.UsingStore.Skip(1).First(), BadNamespace);
        }

        [Fact]
        public void Namespace_that_is_null_is_ignored()
        {
            var code = Code.Create(CodeNamespace)
                         .Usings(
                             "",
                             BadNamespace,
                             null);
            CheckCode(code, CodeNamespace, 2);
            CheckUsing(code.UsingStore.First(), "");
            CheckUsing(code.UsingStore.Skip(1).First(), BadNamespace);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_namespace_with_usings_is_correct(bool useVB)
        {
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @"using static First = FirstNamespace;
                                     using Second.Namespace;
                                     using Third.Yes.No.Namespace;
                           
                                     namespace CodeNamespace
                                     {
                                     }",
                VBGenerator => @"Imports First = FirstNamespace
                                 Imports Second.Namespace
                                 Imports Third.Yes.No.Namespace
                    
                                 Namespace CodeNamespace
                                 End Namespace",
            };
            var code = Code.Create(CodeNamespace)
                           .Usings(
                                new Using(FirstNamespace, FirstNamespaceAlias, true),
                                SecondNamespace,
                                ThirdNamespace);
            var actual = generator.Generate(code);


            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_namespace_not_output_for_null(bool useVB)
        {
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @"using static First = FirstNamespace;
                                     using Second.Namespace;
                                     using Third.Yes.No.Namespace;
                                     ",
                VBGenerator => @"Imports First = FirstNamespace
                                 Imports Second.Namespace
                                 Imports Third.Yes.No.Namespace
                                  ",
            };
            var code = Code.Create(NullNamespace)
                           .Usings(
                                new Using(FirstNamespace, FirstNamespaceAlias, true),
                                SecondNamespace,
                                ThirdNamespace);
            var actual = generator.Generate(code);


            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_namespace_not_output_for_empty_string(bool useVB)
        {
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @"using static First = FirstNamespace;
                                     using Second.Namespace;
                                     using Third.Yes.No.Namespace;
                                     ",
                VBGenerator => @"Imports First = FirstNamespace
                                 Imports Second.Namespace
                                 Imports Third.Yes.No.Namespace
                                  ",
            };
            var code = Code.Create(EmptyNamespace)
                           .Usings(
                                new Using(FirstNamespace, FirstNamespaceAlias, true),
                                SecondNamespace,
                                ThirdNamespace);
            var actual = generator.Generate(code);


            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        private void CheckCode(Code code, string nspace, int usingCount)
        {
            code.Should().NotBeNull();
            code.Namespace.Should().Be(nspace);
            code.UsingStore.Should().HaveCount(usingCount);
        }

        private void CheckUsing(Using usingObj, string nspace, string alias = null, bool usingStatic = false)
        {
            if (nspace is null)
            {
                usingObj.UsingNamespace.Should().BeNull();
            }
            else
            {
                usingObj.UsingNamespace.Should().Be(nspace);
            }
            if (alias is null)
            {
                usingObj.Alias.Should().BeNull();
            }
            else
            {
                usingObj.Alias.Should().Be(alias);
            }
            usingObj.UsingStatic.Should().Be(usingStatic);
        }

    }
}
