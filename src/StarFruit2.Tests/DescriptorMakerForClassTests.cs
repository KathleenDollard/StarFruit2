using FluentAssertions;
using StarFruit.Common;
using System;
using System.CommandLine.Parsing;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForClassTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Command_names_are_as_expected(Type utilType)
        {
            const string cSharpCode = "public int MyProperty { get; set; }";
            const string vBasicCode = "Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.OriginalName.Should().Be(LanguageUtils.StandardClassName);
            actual.Name.Should().Be(LanguageUtils.StandardClassName);
            actual.CliName.Should().Be(LanguageUtils.StandardClassName.ToKebabCase());
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Command_source_is_correct(Type utilType)
        {
            const string cSharpCode = "public int MyProperty { get; set; }";
            const string vBasicCode = "Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.RawInfo.Should().BeOfType<RawInfoForType>();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Hidden]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <Hidden>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Hidden(true)]
                public class MyClass
                {
                  public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <Hidden(true)>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_command(Type utilType)
        {
            const string cSharpCode = @"
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";
            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Hidden(false)]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <Hidden(false)>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Aliases(""x"")]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <Aliases(""x"")>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Multiple_aliases_via_attribute_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Aliases(""xyz"", ""abc"")]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <Aliases(""xyz"", ""abc"")>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";


            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("xyz");
            actual.Aliases.Last().Should().Be("abc");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void No_alias_when_none_given_on_command(Type utilType)
        {
            const string cSharpCode = @"
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(0);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Description_from_xml_on_command(Type utilType)
        {
            var desc = "This is a nice desc";
            string cSharpCode = @$"
                /// <summary>
                /// {desc}
                /// </summary>
                public class MyClass
                {{
                   public int MyPropertyArg {{ get; set; }}
                }}";
            var vBasicCode = $@"
                ''' <summary>
                ''' {desc}
                ''' </summary>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [TreatUnmatchedTokensAsErrors]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <TreatUnmatchedTokensAsErrors>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_not_on_command(Type utilType)
        {
            const string cSharpCode = @"
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_false_when_attribute_is_on_command_and_false(Type utilType)
        {
            const string cSharpCode = @"
                [TreatUnmatchedTokensAsErrors(false)]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }";

            const string vBasicCode = @"
                <TreatUnmatchedTokensAsErrors(false)>
                Public Class [MyClass]
                    Public Property MyPropertyArg As Integer
                End Class
                ";
            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_SubCommand_is_found_and_names_are_as_expected(Type utilType)
        {
            const string cSharpCode = "public int MyMethod(int myParam) { return 0; }";

            const string vBasicCode = @"
                Public Function MyMethod(myParam As Integer) As Integer
                    Return 0
                End Function
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.OriginalName.Should().Be("MyMethod");
            actual.Name.Should().Be("MyMethod");
            actual.CliName.Should().Be("my-method");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Multiple_SubCommands_are_found_and_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod1() { return 0; }
                public int MyMethod2(int myParam) { return 0; }
                public int MyMethod3() {  return 0; }";

            const string vBasicCode = @"
                Public Function MyMethod1() As Integer
                    Return 0
                End Function
                Public Function MyMethod2(myParam As Integer) As Integer
                    Return 0
                End Function
                Public Function MyMethod3() As Integer
                    Return 0
                End Function
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);



            var actual1 = actualCli.CommandDescriptor.SubCommands.First();
            var actual2 = actualCli.CommandDescriptor.SubCommands.Skip(1).First();
            var actual3 = actualCli.CommandDescriptor.SubCommands.Last();

            actual1.OriginalName.Should().Be("MyMethod1");
            actual1.Name.Should().Be("MyMethod1");
            actual1.CliName.Should().Be("my-method1");
            actual2.OriginalName.Should().Be("MyMethod2");
            actual2.Name.Should().Be("MyMethod2");
            actual2.CliName.Should().Be("my-method2");
            actual3.OriginalName.Should().Be("MyMethod3");
            actual3.Name.Should().Be("MyMethod3");
            actual3.CliName.Should().Be("my-method3");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void SubCommand_marked_as_async_as_expected(Type utilType)
        {
            const string cSharpCode = @"
                public async Task<int> MyMethod(int myParam) { return await Task.FromResult(0); }
                public int MyMethod2(int myParam) {  return 0; }
                ";

            const string vBasicCode = @"
                Public Async Function MyMethod(myParam As Integer) As Task(Of Integer)
                    Return Await Task.FromResult(0)
                End Function
                Public Function MyMethod2(myParam As Integer) As Integer
                    Return 0
                End Function
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass(usings: "System.Threading.Tasks")
                .CliDescriptorForClass(out var code);

            var actual1 = actualCli.CommandDescriptor.SubCommands.First();
            var actual2 = actualCli.CommandDescriptor.SubCommands.Skip(1).First();

            actual1.IsAsync.Should().BeTrue();
            actual2.IsAsync.Should().BeFalse();

        }

    }
}
