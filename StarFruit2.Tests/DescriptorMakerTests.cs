using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generator;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerTests
    {
        [Fact]
        public void Required_is_true_when_Required_attribute_used_on_option()
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
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_option()
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
        public void Required_is_false_when_Required_attribute_not_on_option()
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
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_option()
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
        public void Hidden_is_true_when_Hidden_attribute_used_on_option()
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
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_option()
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
        public void Hidden_is_false_when_Hidden_attribute_not_on_option()
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
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_option()
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
        public void Single_alias_via_attribute_on_option()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Aliases(""x"")]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Fact]
        public void Multiple_aliases_via_attribute_on_option()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Aliases(""x"",""y"")]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("x");
            actual.Aliases.Last().Should().Be("y");
        }

        [Fact]
        public void No_alias_when_none_given_on_option()
        {
            var name = "MyPropertyArg";
            var code = $@"
                [Aliases()]
                public int {name} {{ get; set; }}"
                .WrapInClass("MyClass")
                .WrapInNamespace("MyNamespace")
                .PrefaceWithUsing("StarFruit2");

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.Arguments.First();
            actual.Aliases.Count().Should().Be(0);
        }

    }
}
