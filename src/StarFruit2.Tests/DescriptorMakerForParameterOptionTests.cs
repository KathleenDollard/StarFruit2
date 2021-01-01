using FluentAssertions;
using StarFruit.Common;
using System.Linq;
using Xunit;
using System;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForParameterOptionTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Option_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.OriginalName.Should().Be("myParam");
            actual.Name.Should().Be("myParam");
            actual.CliName.Should().Be("--my-param");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Option_ArgumentType_is_int_when_int_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Option_ArgumentType_is_bool_when_int_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(bool myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParam As Boolean) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Required] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Required> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Required(true)] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Required(true)> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_not_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Required(false)] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Required(false)> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Hidden] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Hidden> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Hidden(true)] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Hidden(true)> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Hidden(false)] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Hidden(false)> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Aliases(""x"")] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Aliases(""x"")> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Multiple_aliases_via_attribute_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Aliases(""x"",""y"")] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Aliases(""x"", ""y"")> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("x");
            actual.Aliases.Last().Should().Be("y");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void No_alias_when_none_given_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Aliases()] int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Aliases()> myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(0);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Description_from_xml_on_option(Type utilType)
        {
            var desc = "This is a nice desc";
            var cSharpCode = @$"
                                       /// <summary>
                                       /// 
                                       /// </summary>
                                       /// <param name=""myParam"">{desc}</param>
                                       public int MyMethod(int myParam)
                                       {{ return 0; }}
                            ";
            var vBasicCode = @$"
                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name=""myParam"">{desc}</param>
                Public Function MyMethod(myParam As Integer) As Integer 
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_from_property_initialize_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParam=42)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(Optional myParam As Integer = 42) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_property_from_property_initialize_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParam=System.Int32.MinValue)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(Optional myParam As Integer = System.Int32.MinValue) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("-2147483648");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Allowed_values_from_attribute_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([AllowedValues(1,3,5,7,11,13)] int myParam=System.Int32.MinValue)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<AllowedValues(1, 3, 5, 7, 11, 13)> Optional myParam As Integer = System.Int32.MinValue) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }


        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void RawInfo_correct_for_option(Type utilType)
        {
            var cSharpCode = @"public int MyMethod(int myParam)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParam As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.RawInfo.Should().BeOfType<RawInfoForMethodParameter>();
        }
    }
}
