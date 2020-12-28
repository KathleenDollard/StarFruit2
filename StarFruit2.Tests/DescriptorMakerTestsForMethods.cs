using FluentAssertions;
using FluentAssertions.Execution;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerTestsForMethods
    {

        [Fact]
        public void Command_names_are_as_expected()
        {
            var code = @"
                public int MyMethod () { return 0; }"
                .WrapInClass("MyClass")
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.OriginalName.Should().Be("MyMethod");
            actual.Name.Should().Be("MyMethod");
            actual.CliName.Should().Be("my-method");
        }

        [Fact]
        public void Command_source_is_correct()
        {
            var code = @"
                public int MyMethod () { return 0; }"
                .WrapInClass("MyClass")
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.RawInfo.Should().BeOfType<RawInfoForMethod>();
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_command()
        {
            var code = @"
               [Hidden]
               public void MyMethod () { return; }"
                 .WrapInClass("MyClass")
                 .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_command()
        {
            var code = @"
                [Hidden(true)]
                public void MyMethod () { return; }"
                  .WrapInClass("MyClass")
                  .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_command()
        {
            var code = @"
                public int MyMethod () { return 0; }"
                .WrapInClass("MyClass")
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_command()
        {
            var code = @"
                [Hidden(false)]
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Single_alias_via_attribute_on_command()
        {
            var code = @"
                [Aliases(""x"")]
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Fact]
        public void Multiple_aliases_via_attribute_on_command()
        {
            var code = @"
                [Aliases(""xyz"", ""abc"")]
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("xyz");
            actual.Aliases.Last().Should().Be("abc");
        }

        [Fact]
        public void No_alias_when_none_given_on_command()
        {
            var code = @"
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.Aliases.Count().Should().Be(0);
        }

        [Fact]
        public void Description_from_xml_on_command()
        {
            var desc = "This is a nice desc";
            var code = @$"
                /// <summary>
                /// {desc}
                /// </summary>
                public void MyMethod () {{ return; }}"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_on_command()
        {
            var code = @"
                [TreatUnmatchedTokensAsErrors]
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_not_on_command()
        {
            var code = @"
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_false_when_attribute_is_on_command_and_false()
        {
            var code = @"
                [TreatUnmatchedTokensAsErrors(false)]
                public void MyMethod () { return; }"
                   .WrapInClass("MyClass")
                   .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.TreatUnmatchedTokensAsErrors.Should().BeFalse();
        }

    }
}
