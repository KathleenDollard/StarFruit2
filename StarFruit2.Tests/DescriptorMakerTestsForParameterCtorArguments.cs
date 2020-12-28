using FluentAssertions;
using FluentAssertions.Execution;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class MyClass
    {
        public MyClass(int myParamArg)
        {}
    }

    public class DescriptorMakerTestsForParameterCtorArguments
    {
        [Fact]
        public void Argument_names_are_as_expected()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg)
    {}
}"
                      .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.OriginalName.Should().Be("myParamArg");
            actual.Name.Should().Be("myParam");
            actual.CliName.Should().Be("my-param");
        }

        [Fact]
        public void ArgumentType_is_int_when_int_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg)
    {}
}"
                       .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Fact]
        public void ArgumentType_is_bool_when_int_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(bool myParamArg)
    {}
}"
                       .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_used_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass([Required] int myParamArg)
    {}
}"
                       .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass([Required(true)] int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_not_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass([Required(false)] int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass([Hidden] int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass([Hidden(true)] int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass([Hidden(false)] int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Description_from_xml_on_argument()
        {
            var desc = "This is a nice param desc";
            var code = @$"
public class MyClass
{{
    /// <summary>
    /// 
    /// </summary>
    /// <param name=""myParamArg"">{desc}</param> 
    public MyClass([Hidden(false)] int myParamArg)
    {{}}
}}"
                .WrapInStandardNamespace();
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void DefaultValue_from_parameter_initialize_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg=42)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("42");
        }


        [Fact]
        public void DefaultValue_is_null_If_not_given_as_parameter_initialize_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.Should().BeNull();
        }

        [Fact]
        public void DefaultValue_as_parameter_from_parameter_initialize_on_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg=System.Int32.MinValue)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("-2147483648");
        }

        [Fact]
        public void Allowed_values_from_attribute_on_argument()
        {
            var code = @"                       
public class MyClass
{
    public MyClass([AllowedValues(1,3,5,7,11,13)] int myParamArg=System.Int32.MinValue)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Fact]
        public void Parameter_position_correct_for_arguments_and_options()
        {
            var code = @"                       
public class MyClass
{
    public MyClass(int first, string secondArg, int third)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual1 = actualCli.CommandDescriptor.Arguments.First();
            var actual2 = actualCli.CommandDescriptor.Options.First();
            var actual3 = actualCli.CommandDescriptor.Options.Skip(1).First();

            using var x = new AssertionScope();
            actual1.Position.Should().Be(1);
            actual2.Position.Should().Be(0);
            actual3.Position.Should().Be(2);
        }

        [Fact]
        public void MemberSource_correct_for_argument()
        {
            var code = @"
public class MyClass
{
    public MyClass(int myParamArg)
    {}
}"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.RawInfo.Should().BeOfType<RawInfoForCtorParameter>();
        }

    }
}
