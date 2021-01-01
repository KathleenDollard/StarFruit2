using FluentAssertions;
using StarFruit.Common;
using System;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForMethodTests
    {

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Command_names_are_as_expected(Type utilType)
        {
            const string cSharpCode = @"public int MyMethod () { return 0; }";
            const string vBasicCode = @"
               Public Function MyMethod() As Integer
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
        public void Command_source_is_correct(Type utilType)
        {
            const string cSharpCode = @"public int MyMethod () { return 0; }";
            const string vBasicCode = @"
               Public Function MyMethod() As Integer
                    Return 0
                End Function
                 ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.RawInfo.Should().BeOfType<RawInfoForMethod>();
            actual.Name.Should().Be("MyMethod");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_command(Type utilType)
        {
            const string cSharpCode = @"
               [Hidden]
               public void MyMethod () { return; }";
            const string vBasicCode = @"
                < Hidden >
                Public Sub MyMethod()
                    Return
                End Sub
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Hidden(true)]
                public void MyMethod () { return; }";
            const string vBasicCode = @"
                < Hidden(true) >
                Public Sub MyMethod()
                    Return
                End Sub
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_command(Type utilType)
        {
            const string cSharpCode = @"public int MyMethod () { return 0; }";
            const string vBasicCode = @"
                Public Sub MyMethod()
                    Return
                End Sub
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Hidden(false)]
                public void MyMethod () { return; }";
            const string vBasicCode = @"
                < Hidden(false) >
                Public Sub MyMethod()
                    Return
                End Sub
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [Aliases(""x"")]
                public void MyMethod () { return; }";
            const string vBasicCode = @"
                <Aliases(""x"")>
                Public Sub MyMethod()
                    Return
                End Sub";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

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
                public void MyMethod () { return; }";
            const string vBasicCode = @"
                <Aliases(""xyz"", ""abc"")>
                Public Sub MyMethod()
                    Return
                End Sub";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("xyz");
            actual.Aliases.Last().Should().Be("abc");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void No_alias_when_none_given_on_command(Type utilType)
        {
            const string cSharpCode = @"public void MyMethod () { return; }";
            const string vBasicCode = @"
                Public Sub MyMethod()
                    Return
                End Sub
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

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
                public void MyMethod () {{ return; }}";
            var vBasicCode = @$"
                ''' <summary>
                ''' {desc}
                ''' </summary>
                Public Sub MyMethod()
                    Return
                End Sub
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_on_command(Type utilType)
        {
            const string cSharpCode = @"
                [TreatUnmatchedTokensAsErrors]
                public void MyMethod () { return; }";
            const string vBasicCode = @"
                <TreatUnmatchedTokensAsErrors>
                Public Sub MyMethod()
                    Return
                End Sub";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_not_on_command(Type utilType)
        {
            const string cSharpCode = @"public void MyMethod () { return; }";
            const string vBasicCode = @"
                Public Sub MyMethod()
                    Return
                End Sub
                ";
            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void TreatUnmatchedTokensAsErrors_is_false_when_attribute_is_on_command_and_false(Type utilType)
        {
            const string cSharpCode = @"
                [TreatUnmatchedTokensAsErrors(false)]
                public void MyMethod () { return; }";
            const string vBasicCode = @"
                < TreatUnmatchedTokensAsErrors(false) >
                Public Sub MyMethod()
                    Return
                End Sub";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.TreatUnmatchedTokensAsErrors.Should().BeFalse();
        }

    }
}
