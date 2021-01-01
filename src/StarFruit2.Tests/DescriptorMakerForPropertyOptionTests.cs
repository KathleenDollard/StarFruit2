using FluentAssertions;
using StarFruit.Common;
using System.Linq;
using Xunit;
using System;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForPropertyOptionTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Option_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.OriginalName.Should().Be("MyProperty");
            actual.Name.Should().Be("MyProperty");
            actual.CliName.Should().Be("--my-property");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Option_ArgumentType_is_int_when_int_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Required]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Option_ArgumentType_is_bool_when_int_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Required(true)]
                public bool MyProperty { get; set; }";
            var vBasicCode = @"
                Public Property MyProperty As Boolean";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Required]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Required>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Required(true)]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Required(True)>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_not_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Required(false)]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Required(False)>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Hidden]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Hidden>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Hidden(true)]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Hidden(True)>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Hidden(false)]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Hidden(False)>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Aliases(""x"")]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Aliases(""x"")>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Multiple_aliases_via_attribute_on_option(Type utilType)
        {
            var cSharpCode = @"
                [Aliases(""x"",""y"")]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Aliases(""x"",""y"")>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

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
                [Aliases()]
                public int MyProperty { get; set; }";
            var vBasicCode = @"
                <Aliases()>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

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
                /// {desc}
                /// </summary>
                [Aliases()]
                public int MyProperty {{ get; set; }}";
            var vBasicCode = @$"
                ''' <summary>
                ''' {desc}
                ''' </summary>
                <Aliases()>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_from_property_initialize_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyProperty { get; set; } = 42;";
            var vBasicCode = @"
                Public Property MyProperty As Integer = 42";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_new_object_from_property_initialize_on_option(Type utilType)
        {
            var cSharpCode = @"
                public System.DateTime MyProperty { get; set; } = new System.DateTime(2020, 12, 31);";
            var vBasicCode = @"
                Public Property MyProperty As System.DateTime = New System.DateTime(2020, 12, 31)";
            var expected = utilType.Name == nameof(CSharpLanguageUtils)
                       ? "new System.DateTime(2020, 12, 31)"
                       : "New System.DateTime(2020, 12, 31)";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                            .WrapInClass()
                            .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_property_from_property_initialize_on_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyProperty { get; set; } = System.Int32.MinValue;";
            var vBasicCode = @"
                Public Property MyProperty As Integer = System.Int32.MinValue";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("System.Int32.MinValue");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Allowed_values_from_attribute_on_option(Type utilType)
        {
            var cSharpCode = @"
                [AllowedValues(1,3,5,7,11,13)]
                public int MyProperty { get; set; } ";
            var vBasicCode = @"
                <AllowedValues(1, 3, 5, 7, 11, 13)>
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Arguments.First().AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void RawInfo_correct_for_option(Type utilType)
        {
            var cSharpCode = @"
                public int MyProperty { get; set; } ";
            var vBasicCode = @"
                Public Property MyProperty As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.RawInfo.Should().BeOfType<RawInfoForProperty>();
        }
    }
}
