using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using FluentDom.Generator;
using StarFruit.Common;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generate;
using Xunit;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class CommandSourceGenTests
    {
        [UseReporter(typeof(VisualStudioReporter))]
        [Fact]
        public void Simple_command()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null, OriginalElementType.Class);
            descriptor.CommandDescriptor.CliName = "my-command";
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommandCommandSource>
{{
   public MyCommandCommandSource()
   : this(null, null)
   {{
   }}
   public MyCommandCommandSource(RootCommandSource root, CommandSource parent)
   : base(new Command(""my-command"", null), parent)
   {{
      Command.Handler = CommandHandler.Create((InvocationContext context) =>
             {{  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
             }});
   }}


}}
";

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Fact]
        public void Command_with_subcommand()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null, OriginalElementType.Class);
            descriptor.CommandDescriptor.CliName = "my-command";
            descriptor.CommandDescriptor.Name = "MyCommand";
            descriptor.CommandDescriptor.SubCommands.Add(new CommandDescriptor(descriptor.CommandDescriptor, "SubCommand", null, OriginalElementType.Class));

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
        }

        [Fact]
        public void Command_with_option()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null, OriginalElementType.Class);
            descriptor.CommandDescriptor.Options.Add(new OptionDescriptor(descriptor.CommandDescriptor, "Option1", null, OriginalElementType.Property ));

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
        }

        [Fact]
        public void Command_with_argument()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null, OriginalElementType.Class);
            descriptor.CommandDescriptor.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), descriptor.CommandDescriptor, "Argument1", null, OriginalElementType.Property));
            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
        }
    }
}
