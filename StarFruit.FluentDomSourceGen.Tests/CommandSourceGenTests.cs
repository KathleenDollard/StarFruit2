using FluentAssertions;
using FluentDom.Generator;
using GeneratorSupport;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using Xunit;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class CommandSourceGenTests
    {
        [Fact]
        public void Simple_command()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommandCommandSource>
{{
   public class MyCommandCommandSource()
   : base(new Command(null))
   {{
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
             {{  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
             }});
   }}



}}
";
            expected = Utils.NormalizeLineEndings(expected);

            var template = new GenerateCommandSource();
            var code = template.CreateCode(descriptor);
            var generator = new CSharpGenerator();
            var actual = generator.Generate(code);
            actual = Utils.NormalizeLineEndings(actual);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Command_with_subcommand()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.SubCommands.Add(new CommandDescriptor(descriptor.CommandDescriptor,"SubCommand",null));
            var expected = $@"";
            expected = Utils.NormalizeLineEndings(expected);

            var template = new GenerateCommandSource();
            var code = template.CreateCode(descriptor);
            var generator = new CSharpGenerator();
            var actual = generator.Generate(code);
            actual = Utils.NormalizeLineEndings(actual);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Command_with_option()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.Options.Add(new OptionDescriptor (descriptor.CommandDescriptor, "Option1", null));
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommandCommandSource>
{{
   public class MyCommandCommandSource()
   : base(new Command(null))
   {{
      Option1 = GetOption1();
      Command.Add(Option1);
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
             {{  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
             }});
   }}

   public Option<bool> Option1 {{ get; set; }}

   public Option<bool> GetOption1()
   {{
      Option<bool> option = new Option<bool>();
      option.Description = null;
      option.IsRequired = false;
      option.IsHidden = false;
   }}
}}
";
            expected = Utils.NormalizeLineEndings(expected);

            var template = new GenerateCommandSource();
            var code = template.CreateCode(descriptor);
            var generator = new CSharpGenerator();
            var actual = generator.Generate(code);
            actual = Utils.NormalizeLineEndings(actual);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Command_with_argument()
        {
            var descriptor = new CliDescriptor();
            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
            descriptor.CommandDescriptor.Arguments .Add(new ArgumentDescriptor (new ArgTypeInfoRoslyn(typeof(string)),descriptor.CommandDescriptor, "Argument1", null));
            var expected = $@"using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

public class MyCommandCommandSource : RootCommandSource<MyCommandCommandSource>
{{
   public class MyCommandCommandSource()
   : base(new Command(null))
   {{
      Argument1 = GetArgument1();
      Command.Add(Argument1);
      CommandHandler = CommandHandler.Create((InvocationContext context) =>
             {{  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
             }});
   }}

   public Argument<string> Argument1 {{ get; set; }}

   public Argument<string> GetArgument1()
   {{
      Argument<string> argument = new Argument<string>();
      argument.Description = null;
      argument.IsRequired = false;
      argument.IsHidden = false;
   }}
}}
";
            expected = Utils.NormalizeLineEndings(expected);

            var template = new GenerateCommandSource();
            var code = template.CreateCode(descriptor);
            var generator = new CSharpGenerator();
            var actual = generator.Generate(code);
            actual = Utils.NormalizeLineEndings(actual);

            actual.Should().Be(expected);
        }
    }
}
