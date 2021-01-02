using FluentAssertions;
using StarFruit.Common;
using System.Linq;
using Xunit;
using System;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForPropertyTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Option_names_are_as_expected(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"public int {name} {{ get; set; }}";
            var vBasicCode = @$"Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            SymbolDescriptor actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First()
                        : actualCli.CommandDescriptor.Options.First();

            actual.OriginalName.Should().Be(name);
            actual.Name.Should().Be("MyProperty");
            actual.CliName.Should().Be(useArg ? "my-property" : "--my-property");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Option_ArgumentType_is_int_when(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Required]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First()
                        : actualCli.CommandDescriptor.Options.First().Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Option_ArgumentType_is_bool_when(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Required(true)]
                public bool {name} {{ get; set; }}";
            var vBasicCode = @$"
                Public Property {name} As Boolean";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First()
                        : actualCli.CommandDescriptor.Options.First().Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_true_when_Required_attribute_used(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Required]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Required>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First().Required
                        : actualCli.CommandDescriptor.Options.First().Required;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_true_when_Required_attribute_with_true_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Required(true)]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Required(True)>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First().Required
                        : actualCli.CommandDescriptor.Options.First().Required;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_false_when_Required_attribute_not(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First().Required
                        : actualCli.CommandDescriptor.Options.First().Required;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_false_when_Required_attribute_with_false_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Required(false)]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Required(False)>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First().Required
                        : actualCli.CommandDescriptor.Options.First().Required;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_true_when_Hidden_attribute_used(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Hidden]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Hidden>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.Arguments.First().IsHidden
                        : actualCli.CommandDescriptor.Options.First().IsHidden;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Hidden(true)]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Hidden(True)>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                     ? actualCli.CommandDescriptor.Arguments.First().IsHidden
                     : actualCli.CommandDescriptor.Options.First().IsHidden;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_false_when_Hidden_attribute_not(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                   ? actualCli.CommandDescriptor.Arguments.First().IsHidden
                   : actualCli.CommandDescriptor.Options.First().IsHidden;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [Hidden(false)]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Hidden(False)>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First().IsHidden
                         : actualCli.CommandDescriptor.Options.First().IsHidden;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute(Type utilType)
        {
            var name = "MyProperty";
            var cSharpCode = @$"
                [Aliases(""x"")]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Aliases(""x"")>
                Public Property {name} As Integer";

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
        public void Multiple_aliases_via_attribute(Type utilType)
        {
            var name = "MyProperty";
            var cSharpCode = @$"
                [Aliases(""x"",""y"")]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Aliases(""x"",""y"")>
                Public Property {name} As Integer";

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
        public void No_alias_when_none_given(Type utilType)
        {
            var name = "MyProperty";
            var cSharpCode = @$"
                [Aliases()]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <Aliases()>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Options.First();

            actual.Aliases.Count().Should().Be(0);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Description_from_xml(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var desc = "This is a nice desc";
            var cSharpCode = @$"
                /// <summary>
                /// {desc}
                /// </summary>
                [Aliases()]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                ''' <summary>
                ''' {desc}
                ''' </summary>
                <Aliases()>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            SymbolDescriptor actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First()
                         : actualCli.CommandDescriptor.Options.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void DefaultValue_from_property_initialize(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                public int {name} {{ get; set; }} = 42;";
            var vBasicCode = @$"
                Public Property {name} As Integer = 42";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First().DefaultValue
                         : actualCli.CommandDescriptor.Options.First().Arguments.First().DefaultValue;

            actual.Should().NotBeNull();
            actual!.CodeRepresentation.Should().Be("42");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void DefaultValue_as_new_object_from_property_initialize(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                public System.DateTime {name} {{ get; set; }} = new System.DateTime(2020, 12, 31);";
            var vBasicCode = @$"
                Public Property {name} As System.DateTime = New System.DateTime(2020, 12, 31)";
            var expected = utilType.Name == nameof(CSharpLanguageUtils)
                       ? "new System.DateTime(2020, 12, 31)"
                       : "New System.DateTime(2020, 12, 31)";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                            .WrapInClass()
                            .CliDescriptorForClass(out var code);

            var actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First().DefaultValue
                         : actualCli.CommandDescriptor.Options.First().Arguments.First().DefaultValue;

            actual.Should().NotBeNull();
            actual!.CodeRepresentation.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void DefaultValue_as_property_from_property_initialize(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                public int {name} {{ get; set; }} = System.Int32.MinValue;";
            var vBasicCode = @$"
                Public Property {name} As Integer = System.Int32.MinValue";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First().DefaultValue
                         : actualCli.CommandDescriptor.Options.First().Arguments.First().DefaultValue;

            actual.Should().NotBeNull();
            actual!.CodeRepresentation.Should().Be("System.Int32.MinValue");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Allowed_values_from_attribute(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                [AllowedValues(1,3,5,7,11,13)]
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                <AllowedValues(1, 3, 5, 7, 11, 13)>
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First().AllowedValues
                         : actualCli.CommandDescriptor.Options.First().Arguments.First().AllowedValues;

            actual.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void RawInfo_correct_for_option(Type utilType, bool useArg)
        {
            var name = useArg ? "MyPropertyArg" : "MyProperty";
            var cSharpCode = @$"
                public int {name} {{ get; set; }}";
            var vBasicCode = @$"
                Public Property {name} As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            SymbolDescriptor  actual = useArg
                         ? actualCli.CommandDescriptor.Arguments.First()
                         : actualCli.CommandDescriptor.Options.First();

            actual.RawInfo.Should().BeOfType<RawInfoForProperty>();
        }
    }
}
