using FluentAssertions;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerTestsForClasses
    {
        [Fact]
        public void Command_names_are_as_expected()
        {
            var code = @"
                public int MyProperty { get; set; }"
                .WrapInClass("MyClass")
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.OriginalName.Should().Be("MyClass");
            actual.Name.Should().Be("MyClass");
            actual.CliName.Should().Be("my-class");
        }


        [Fact]
        public void Command_source_is_correct()
        {
            var code = @"
                public int MyProperty { get; set; }"
                .WrapInClass("MyClass")
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.OriginalElementType .Should().Be(OriginalElementType.Class);
        }


        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_used_on_command()
        {
            var code = @"
                [Hidden]
                public class MyClass
                {
                public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_command()
        {
            var code = @"
                [Hidden(true)]
                public class MyClass
                {
                  public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(true);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_not_on_command()
        {
            var code = @"
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_command()
        {
            var code = @"
                [Hidden(false)]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.IsHidden.Should().Be(false);
        }

        [Fact]
        public void Single_alias_via_attribute_on_command()
        {
            var code = @"
                [Aliases(""x"")]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Fact]
        public void Multiple_aliases_via_attribute_on_command()
        {
            var code = @"
                [Aliases(""xyz"", ""abc"")]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("xyz");
            actual.Aliases.Last().Should().Be("abc");
        }

        [Fact]
        public void No_alias_when_none_given_on_command()
        {
            var code = @"
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

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
                public class MyClass
                {{
                   public int MyPropertyArg {{ get; set; }}
                }}"
                .WrapInStandardNamespace();
            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.Description.Should().Be(desc);
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_on_command()
        {
            var code = @"
                [TreatUnmatchedTokensAsErrors]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_true_when_attribute_is_not_on_command()
        {
            var code = @"
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();


            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeTrue();
        }

        [Fact]
        public void TreatUnmatchedTokensAsErrors_is_false_when_attribute_is_on_command_and_false()
        {
            var code = @"
                [TreatUnmatchedTokensAsErrors(false)]
                public class MyClass
                {
                   public int MyPropertyArg { get; set; }
                }"
                .WrapInStandardNamespace();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor;

            actual.TreatUnmatchedTokensAsErrors.Should().BeFalse();
        }

        [Fact]
        public void Single_SubCommand_is_found_and_names_are_as_expected()
        {
            var code = @"
                public int MyMethod(int myParam) { return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual = actualCli.CommandDescriptor.SubCommands.First();

            actual.OriginalName.Should().Be("MyMethod");
            actual.Name.Should().Be("MyMethod");
            actual.CliName.Should().Be("my-method");
        }

        [Fact]
        public void Multiple_SubCommands_are_found_and_names_are_as_expected()
        {
            var code = @"
                public int MyMethod1() { return 0; }
                public int MyMethod2(int myParam) { return 0; }
                public int MyMethod3() {  return 0; }"
                .WrapInStandardClass();

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual1 = actualCli.CommandDescriptor.SubCommands.First();
            var actual2 = actualCli.CommandDescriptor.SubCommands.Skip(1).First();
            var actual3 = actualCli.CommandDescriptor.SubCommands.Last();

            actual1.OriginalName.Should().Be("MyMethod1");
            actual1.Name.Should().Be("MyMethod1");
            actual1.CliName.Should().Be("my-method1");
            actual2.OriginalName.Should().Be("MyMethod2");
            actual2.Name.Should().Be("MyMethod2");
            actual2.CliName.Should().Be("my-method2");
            actual3.OriginalName.Should().Be("MyMethod3");
            actual3.Name.Should().Be("MyMethod3");
            actual3.CliName.Should().Be("my-method3");
        }

        [Fact]
        public void SubCommand_marked_as_async_as_expected()
        {
            var code = @"
                public async Task<int> MyMethod(int myParam) { return await Task.FromResult(0); }
                public int MyMethod2(int myParam) {  return 0; }"
                .WrapInStandardClass()
                .PrefaceWithUsing("System.Threading.Tasks"); ;

            CliDescriptor actualCli = Utils.GetCli(code);
            var actual1 = actualCli.CommandDescriptor.SubCommands.First();
            var actual2 = actualCli.CommandDescriptor.SubCommands.Skip(1).First();

            actual1.IsAsync.Should().BeTrue();
            actual2.IsAsync.Should().BeFalse();

        }

    }
    }
