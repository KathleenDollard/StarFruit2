using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generator;
using System;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerTests
    {
        #region Arguments
        [Fact]
        public void Argument_names_are_as_expected()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.OriginalName.Should().Be("MyPropertyArg");
            actual.Name.Should().Be("MyProperty");
            actual.CliName.Should().Be("my-property");
        }

        [Fact]
        public void ArgumentType_is_int_when_int_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Required]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Fact]
        public void ArgumentType_is_bool_when_int_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Required(true)]
                public bool {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_used_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Required]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Required(true)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_not_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Required(false)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Hidden]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Hidden(true)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Hidden(false)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Description_from_xml_on_argument()
        {
            var desc = "This is a nice desc";
            var name = "MyPropertyArg";
            var code = $@"
                /// <summary>
                /// {desc}
                /// </summary>
                [Aliases()]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void DefaultValue_from_property_initialize_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public int {name} {{ get; set; }} = 42"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Fact]
        public void DefaultValue_as_new_object_from_property_initialize_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public DateTime {name} {{ get; set; }} = new DateTime(2020, 12, 31) "
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2", "System");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.DefaultValue.CodeRepresentation.Should().Be("new DateTime(2020, 12, 31)");
        }

        [Fact]
        public void DefaultValue_as_property_from_property_initialize_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public DateTime {name} {{ get; set; }} = Int32.MinValue"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2", "System");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.DefaultValue.CodeRepresentation.Should().Be("Int32.MinValue");
        }

        [Fact]
        public void Allowed_values_from_attribute_on_argument()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [AllowedValues(1,3,5,7,11,13)]
                public int {name} {{ get; set; }} "
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2", "System");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }
        #endregion

        #region Options
        [Fact]
        public void Option_names_are_as_expected()
        {
            var name = "MyProperty";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.OriginalName.Should().Be("MyProperty");
            actual.Name.Should().Be("MyProperty");
            actual.CliName.Should().Be("--my-property");
        }

        [Fact]
        public void Option_ArgumentType_is_int_when_int_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Required]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Fact]
        public void Option_ArgumentType_is_bool_when_int_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Required(true)]
                public bool {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Arguments.First().ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_used_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Required]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Required(true)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Required.Should().Be(true);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_not_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Required(false)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Required.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Hidden]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Hidden(true)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Hidden(false)]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Single_alias_via_attribute_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Aliases(""x"")]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Fact]
        public void Multiple_aliases_via_attribute_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Aliases(""x"",""y"")]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("x");
            actual.Aliases.Last().Should().Be("y");
        }

        [Fact]
        public void No_alias_when_none_given_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [Aliases()]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Aliases.Count().Should().Be(0);
        }

        [Fact]
        public void Description_from_xml_on_option()
        {
            var desc = "This is a nice desc";
            var name = "MyProperty";
            var code = $@"
                /// <summary>
                /// {desc}
                /// </summary>
                [Aliases()]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void DefaultValue_from_property_initialize_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                public int {name} {{ get; set; }} = 42"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("42");
        }

        [Fact]
        public void DefaultValue_as_new_object_from_property_initialize_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                public DateTime {name} {{ get; set; }} = new DateTime(2020, 12, 31) "
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2", "System");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("new DateTime(2020, 12, 31)");
        }

        [Fact]
        public void DefaultValue_as_property_from_property_initialize_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                public DateTime {name} {{ get; set; }} = Int32.MinValue"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2", "System");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Arguments.First().DefaultValue.CodeRepresentation.Should().Be("Int32.MinValue");
        }

        [Fact]
        public void Allowed_values_from_attribute_on_option()
        {
            var name = "MyProperty";
            var code = $@"
                [AllowedValues(1,3,5,7,11,13)]
                public int {name} {{ get; set; }} "
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2", "System");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Options.First();
            actual.Arguments.First().AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        #endregion

        #region Commands

        [Fact]
        public void Command_names_are_as_expected()
        {
            var name = "MyProperty";
            var code = $@"
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.OriginalName.Should().Be("MyClass");
            actual.Name.Should().Be("MyClass");
            actual.CliName.Should().Be("my-class");
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_command()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Hidden]
                public class MyClass
                {{
                public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_command()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Hidden(true)]
                public class MyClass
                {{
                  public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_command()
        {
            var name = "MyPropertyArg";
            var code = $@"
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_command()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Hidden(false)]
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Single_alias_via_attribute_on_command()
        {
            var name = "MyProperty";
            var code = $@"
                [Aliases(""x"")]
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Fact]
        public void Multiple_aliases_via_attribute_on_command()
        {
            var name = "MyProperty";
            var code = $@"
                [Aliases(""xyz"", ""abc"")]
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("xyz");
            actual.Aliases.Last().Should().Be("abc");
        }

        [Fact]
        public void No_alias_when_none_given_on_command()
        {
            var name = "MyProperty";
            var code = $@"
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.Aliases.Count().Should().Be(0);
        }

        [Fact]
        public void Description_from_xml_on_command()
        {
            var desc = "This is a nice desc";
            var name = "MyPropertyArg";
            var code = $@"
                /// <summary>
                /// {desc}
                /// </summary>
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_on_command()
        {
            var name = "MyProperty";
            var code = $@"
                [TreatUnmatchedTokensAsErrors]
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_not_on_command()
        {
            var name = "MyProperty";
            var code = $@"
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_false_when_attribute_is_on_command_and_false()
        {
            var name = "MyProperty";
            var code = $@"
                [TreatUnmatchedTokensAsErrors(false)]
                public class MyClass
                {{
                   public int {name} {{ get; set; }}
                }}"
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;
            actual.TreatUnmatchedTokensAsErrors.Should().BeFalse();
        }


        #endregion

    }
}
