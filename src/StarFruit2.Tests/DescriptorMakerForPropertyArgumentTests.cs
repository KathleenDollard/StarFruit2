using FluentAssertions;
using StarFruit.Common;
using System.Linq;
using Xunit;
using FluentAssertions.Execution;
using System;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForPropertyArgumentTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Argument_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.OriginalName.Should().Be("MyPropertyArg");
            actual.Name.Should().Be("MyProperty");
            actual.CliName.Should().Be("my-property");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void ArgumentType_is_int_when_int_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void ArgumentType_is_bool_when_int_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public bool MyPropertyArg { get; set; }";
            var vBasicCode = @"
                Public Property MyPropertyArg As Boolean";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [Required]
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                <Required>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [Required(true)]
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                <Required(True)>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_not_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [Required(false)]
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                <Required(False)>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [Hidden]
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                <Hidden>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [Hidden(true)]
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                <Hidden(True)>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [Hidden(false)]
                public int MyPropertyArg { get; set; }";
            var vBasicCode = @"
                <Hidden(False)>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Description_from_xml_on_argument(Type utilType)
        {
            var desc = "This is a nice desc";
            var cSharpCode = @$"
                /// <summary>
                /// {desc}
                /// </summary>
                [Aliases()]
                public int MyPropertyArg {{ get; set; }}";
            var vBasicCode = @$"
                ''' <summary>
                ''' {desc}
                ''' </summary>
                <Aliases()>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_from_property_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; } = 42;";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer = 42";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_new_object_from_property_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public System.DateTime MyPropertyArg { get; set; } = new System.DateTime(2020, 12, 31); ";
            var vBasicCode = @"
                Public Property MyPropertyArg As System.DateTime = New System.DateTime(2020, 12, 31)";
            var expected = utilType.Name == nameof(CSharpLanguageUtils)
                            ? "new System.DateTime(2020, 12, 31)"
                            : "New System.DateTime(2020, 12, 31)";


            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_property_from_property_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; } = System.Int32.MinValue;";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer = System.Int32.MinValue";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("System.Int32.MinValue");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Allowed_values_from_attribute_on_argument(Type utilType)
        {
            var cSharpCode = @"
                [AllowedValues(1,3,5,7,11,13)]
                public int MyPropertyArg { get; set; } ";
            var vBasicCode = @"
                <AllowedValues(1, 3, 5, 7, 11, 13)>
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void RawInfo_correct_for_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyPropertyArg { get; set; } ";
            var vBasicCode = @"
                Public Property MyPropertyArg As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.RawInfo.Should().BeOfType<RawInfoForProperty>();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Parameter_position_correct_for_arguments_and_options(Type utilType)
        {
            var cSharpCode = @"                       
                public int First{get;set;}
                public int SecondArg{get;set;}
                public int Third{get;set;}";
            var vBasicCode = @"
                Public Property First As Integer
                Public Property SecondArg As Integer
                Public Property Third As Integer";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual1 = actualCli.CommandDescriptor.Arguments.First();
            var actual2 = actualCli.CommandDescriptor.Options.First();
            var actual3 = actualCli.CommandDescriptor.Options.Skip(1).First();

            using var x = new AssertionScope();
            actual1.Position.Should().Be(1);
            actual2.Position.Should().Be(0);
            actual3.Position.Should().Be(2);
        }

    }
}
