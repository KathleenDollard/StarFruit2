using FluentAssertions;
using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit.Common;
using System.Linq;
using Xunit;
using FluentAssertions.Execution;

namespace StarFruit2.Tests
{
    public class DescriptorMakerTestsForPropertyArguments
    {
        [Fact]
        public void Argument_names_are_as_expected()
        {
            var code = @"
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.OriginalName.Should().Be("MyPropertyArg");
            actual.Name.Should().Be("MyProperty");
            actual.CliName.Should().Be("my-property");
        }

        [Fact]
        public void ArgumentType_is_int_when_int_on_argument()
        {
            var code = @"
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Fact]
        public void ArgumentType_is_bool_when_int_on_argument()
        {
            var code = @"
                public bool MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_used_on_argument()
        {
            var code = @"
                [Required]
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_argument()
        {
            var code = @"
                [Required(true)]
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_not_on_argument()
        {
            var code = @"
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_argument()
        {
            var code = @"
                [Required(false)]
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_argument()
        {
            var code = @"
                [Hidden]
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_argument()
        {
            var code = @"
                [Hidden(true)]
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_argument()
        {
            var code = @"
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_argument()
        {
            var code = @"
                [Hidden(false)]
                public int MyPropertyArg { get; set; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Description_from_xml_on_argument()
        {
            var desc = "This is a nice desc";
            var code = @$"
                /// <summary>
                /// {desc}
                /// </summary>
                [Aliases()]
                public int MyPropertyArg {{ get; set; }}"
                .WrapInStandardClass();
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void DefaultValue_from_property_initialize_on_argument()
        {
            var code = @"
                public int MyPropertyArg { get; set; } = 42;"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Fact]
        public void DefaultValue_as_new_object_from_property_initialize_on_argument()
        {
            var code = @"
                public DateTime MyPropertyArg { get; set; } = new DateTime(2020, 12, 31); "
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("new DateTime(2020, 12, 31)");
        }

        [Fact]
        public void DefaultValue_as_property_from_property_initialize_on_argument()
        {
            var code = @"
                public int MyPropertyArg { get; set; } = Int32.MinValue;"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("Int32.MinValue");
        }

        [Fact]
        public void Allowed_values_from_attribute_on_argument()
        {
            var code = @"
                [AllowedValues(1,3,5,7,11,13)]
                public int MyPropertyArg { get; set; } "
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Fact]
        public void MemberSource_correct_for_argument()
        {
            var code = @"
                public int MyPropertyArg { get; set; } "
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.RawInfo.Should().BeOfType<RawInfoForProperty>();
        }

        [Fact]
        public void Parameter_position_correct_for_arguments_and_options()
        {
            var code = @"                       
                       public int First{get;set;}
                       public int SecondArg{get;set;}
                       public int Third{get;set;}"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
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
