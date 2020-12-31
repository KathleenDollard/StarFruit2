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
    public class DirectGeneratorTests
    {

        private const string namespaceName = "";
        private const string className = "MyCommand";

        [UseReporter(typeof(VisualStudioReporter))]
        [Fact]
        public void Simple_command()
        {
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null,namespaceName, className ))
            };
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
   public MyCommandCommandSource(RootCommandSource root, CommandSourceBase parent)
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
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null, namespaceName, className))
                {
                    CliName = "my-command",
                    Name = "MyCommand"
                }
            };
            descriptor.CommandDescriptor.SubCommands.Add(new CommandDescriptor(descriptor.CommandDescriptor, "SubCommand", new RawInfoForType(null, namespaceName, "SubCommand")));

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
        }

        [Fact]
        public void Command_with_option()
        {
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null, namespaceName, className))
            };
            descriptor.CommandDescriptor.Options.Add(new OptionDescriptor(descriptor.CommandDescriptor, "Option1", new RawInfoForProperty(null,namespaceName, className )));

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
        }

        [Fact]
        public void Command_with_argument()
        {
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null, namespaceName, className))
            };
            descriptor.CommandDescriptor.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), descriptor.CommandDescriptor, "Argument1", new RawInfoForProperty(null, namespaceName, className)));
            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            Approvals.Verify(actual);
        }

    }
}
