using FluentAssertions;
using StarFruit.Common;
using System.Linq;
using Xunit;
using System;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForRootMethodTests
    {

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Command_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod () { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.OriginalName.Should().Be("MyMethod");
            actual.Name.Should().Be("MyMethod");
            actual.CliName.Should().Be("my-method");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Command_source_is_correct(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod () { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod() As Integer
                    Return 0
                End Function";
            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.RawInfo.Should().BeOfType<RawInfoForMethod>();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_command(Type utilType)
        {
            var cSharpCode = @"
               [Hidden]
               public void MyMethod () { return; }";
            var vBasicCode = @"
                <Hidden>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";
            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_command(Type utilType)
        {
            var cSharpCode = @"
                [Hidden(true)]
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <Hidden(True)>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_command(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod () { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_command(Type utilType)
        {
            var cSharpCode = @"
                [Hidden(false)]
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <Hidden(False)>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute_on_command(Type utilType)
        {
            var cSharpCode = @"
                [Aliases(""x"")]
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <Aliases(""x"")>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Multiple_aliases_via_attribute_on_command(Type utilType)
        {
            var cSharpCode = @"
                [Aliases(""xyz"", ""abc"")]
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <Aliases(""xyz"", ""abc"")>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

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
            var cSharpCode = @"
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <Aliases()>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(0);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Description_from_xml_on_command(Type utilType)
        {
            var desc = "This is a nice desc";
            var cSharpCode = @$"
                /// <summary>
                /// {desc}
                /// </summary>
                public void MyMethod () {{ return; }}";
            var vBasicCode = @$"
                ''' <summary>
                ''' {desc}
                ''' </summary>
                Public Sub MyMethod()
                    Return
                End Sub";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_on_command(Type utilType)
        {
            var cSharpCode = @"
                [TreatUnmatchedTokensAsErrors]
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <TreatUnmatchedTokensAsErrors>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_not_on_command(Type utilType)
        {
            var cSharpCode = @"
                public void MyMethod () { return; }";
            var vBasicCode = @"
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_false_when_attribute_is_on_command_and_false(Type utilType)
        {
            var cSharpCode = @"
                [TreatUnmatchedTokensAsErrors(false)]
                public void MyMethod () { return; }";
            var vBasicCode = @"
                <TreatUnmatchedTokensAsErrors(False)>
                Public Function MyMethod() As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForMethod(out var code);

            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeFalse();
        }

    }
}
