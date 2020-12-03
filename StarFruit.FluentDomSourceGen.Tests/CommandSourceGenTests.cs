using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using FluentDom.Generator;
using GeneratorSupport;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Tests.Utility;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
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
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.CliName = "my-command";
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommand>
{{
   public MyCommandCommandSource()
   : base(new Command(""my-command"", null))
   {{
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
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

            //Approvals.Verify(actual);
            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Fact]
        public void Command_with_subcommand()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.CliName = "my-command";
            descriptor.CommandDescriptor.Name = "MyCommand";
            descriptor.CommandDescriptor.SubCommands.Add(new CommandDescriptor(descriptor.CommandDescriptor, "SubCommand", null));
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommand>
{{
   public MyCommandCommandSource()
   : base(new Command(""my-command"", null))
   {{
       SubCommand = new SubCommandCommandSource(this, this);
       Command.AddCommand(SubCommand);
       CommandHandler = CommandHandler.Create((InvocationContext context) =>
              {{
                  CurrentCommandSource = this;
                  CurrentParseResult = context.ParseResult;
                  return 0;
              }});
   }}

   public SubCommandCommandSource SubCommand {{ get; set; }}
}}

public class SubCommandCommandSource : MyCommandCommandSource
{{
    public SubCommandCommandSource()
    : base(new Command(null, null))
    {{
        this.parent = parent;
        CommandHandler = CommandHandler.Create(() =>
        {{
            CurrentCommandSource = this;
            return 0;
        }});
    }}

    public MyCommandCommandSource parent;
    public CommandSourceResult GetCommandSourceResult(ParseResult parseResult, exitCode int)
    {{
        return new SubCommandCommandSource(parseResult, this, exitCode);
    }}


}}
    ";

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Fact]
        public void Command_with_option()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.Options.Add(new OptionDescriptor(descriptor.CommandDescriptor, "Option1", null));
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommand>
{{
   public MyCommandCommandSource()
   : base(new Command(null, null))
   {{
      Option1 = GetOption1();
      Command.Add(Option1);
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
             {{  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
             }});
   }}

   public Option<bool> Option1 {{ get; set; }}

   public Option<bool> GetOption1()
   {{
      Option<bool> option = new Option<bool>();
      option.Description = null;
      option.IsRequired = false;
      option.IsHidden = false;
      return option;
   }}
}}
";

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Fact]
        public void Command_with_argument()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), descriptor.CommandDescriptor, "Argument1", null));
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommand>
{{
   public MyCommandCommandSource()
   : base(new Command(null, null))
   {{
      Argument1 = GetArgument1();
      Command.Add(Argument1);
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
             {{  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
             }});
   }}

   public Argument<string> Argument1 {{ get; set; }}

   public Argument<string> GetArgument1()
   {{
      Argument<string> argument = new Argument<string>();
      argument.Description = null;
      argument.IsRequired = false;
      argument.IsHidden = false;
      return argument;
   }}
}}
";

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = new CSharpGenerator().Generate(dom);

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }


       [Fact]
        public async void Compiled_command_has_expect_values()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.CliName = "my-command";
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommand>
{{
   public MyCommandCommandSource()
   : base(new Command(""my-command"", null))
   {{
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
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

            using var kernel = new CSharpKernel();

            var result = await kernel.SubmitCodeAsync(actual);
            // While assert in arrange is unusual, if this goes bad, the test is toast. 
            result.KernelEvents.ToSubscribedList().Should().NotContainErrors();

            //// act
            //var resultWithInstance = await kernel.SubmitCodeAsync($"new {className}().{methodName}()");
            //resultWithInstance.KernelEvents.ToSubscribedList().Should().NotContainErrors();

            //// assert
            //var returnValue = await resultWithInstance.KernelEvents.OfType<ReturnValueProduced>().SingleAsync();
            //var foo = returnValue.Value;
            //var cmd = foo as Command;
            //cmd.Name.Should().Be("my-class");

        }
    }
}
