using FluentAssertions;
using FluentAssertions.Execution;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerTestsForParameterOptions
    {
        [Fact]
        public void Option_names_are_as_expected()
        {
            var code = @"
public int MyMethod(int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.OriginalName.Should().Be("myParam");
            actual.Name.Should().Be("myParam");
            actual.CliName.Should().Be("--my-param");
        }

        [Fact]
        public void Option_ArgumentType_is_int_when_int_on_option()
        {
            var code = @"
public int MyMethod(int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Fact]
        public void Option_ArgumentType_is_bool_when_int_on_option()
        {
            var code = @"
public int MyMethod(bool myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_used_on_option()
        {
            var code = @"
public int MyMethod([Required] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_option()
        {
            var code = @"
public int MyMethod([Required(true)] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_not_on_option()
        {
            var code = @"
public int MyMethod(int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_option()
        {
            var code = @"
public int MyMethod([Required(false)] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_option()
        {
            var code = @"
public int MyMethod([Hidden] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_option()
        {
            var code = @"
public int MyMethod([Hidden(true)] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_option()
        {
            var code = @"
public int MyMethod(int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_option()
        {
            var code = @"
public int MyMethod([Hidden(false)] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Single_alias_via_attribute_on_option()
        {
            var code = @"
public int MyMethod([Aliases(""x"")] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Fact]
        public void Multiple_aliases_via_attribute_on_option()
        {
            var code = @"
public int MyMethod([Aliases(""x"",""y"")] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("x");
            actual.Aliases.Last().Should().Be("y");
        }

        [Fact]
        public void No_alias_when_none_given_on_option()
        {
            var code = @"
public int MyMethod([Aliases()] int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(0);
        }

        [Fact]
        public void Description_from_xml_on_option()
        {
            var desc = "This is a nice desc";
            var code = @$"
                       /// <summary>
                       /// 
                       /// </summary>
                       /// <param name=""myParam"">{desc}</param>
                       public int MyMethod(int myParam)
                       {{ return 0; }}
            "

.WrapInStandardClass();
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void DefaultValue_from_property_initialize_on_option()
        {
            var code = @"
public int MyMethod(int myParam=42)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Fact]
        public void DefaultValue_as_property_from_property_initialize_on_option()
        {
            var code = @"
public int MyMethod(int myParam=Int32.MinValue)
{ return 0; }"

                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("-2147483648");
        }

        [Fact]
        public void Allowed_values_from_attribute_on_option()
        {
            var code = @"
public int MyMethod([AllowedValues(1,3,5,7,11,13)] int myParam=Int32.MinValue)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Arguments.First().AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }


        [Fact]
        public void MemberSource_correct_for_option()
        {
            var code = @"public int MyMethod(int myParam)
{ return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.RawInfo.Should().BeOfType<RawInfoForMethodParameter>();
        }
    }
}
