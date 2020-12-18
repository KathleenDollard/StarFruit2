//using ApprovalTests;
//using ApprovalTests.Reporters;
//using FluentAssertions;
//using FluentDom.Generator;
//using Microsoft.DotNet.Interactive;
//using Microsoft.DotNet.Interactive.CSharp;
//using Microsoft.DotNet.Interactive.Tests.Utility;
//using StarFruit2;
//using StarFruit2.Common;
//using StarFruit2.Common.Descriptors;
//using StarFruit2.Generate;
//using System.CommandLine;
//using System.Threading.Tasks;
//using Xunit;

//namespace StarFruit.FluentDomSourceGen.Tests
//{
//    public class CommandSourceGenTests
//    {
//        [UseReporter(typeof(VisualStudioReporter))]
//        [Fact]
//        public void Simple_command()
//        {
//            var descriptor = new CliDescriptor();
//            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
//            descriptor.CommandDescriptor.CliName = "my-command";
//            var expected = $@"using StarFruit2;
//using System.CommandLine;
//using StarFruit2.Common;
//using System.CommandLine.Invocation;
//using System.CommandLine.Parsing;

//public class MyCommandCommandSource : RootCommandSource<MyCommandCommandSource>
//{{
//   public MyCommandCommandSource()
//   : base(new Command(""my-command"", null))
//   {{
//      Command.Handler = CommandHandler.Create((InvocationContext context) =>
//             {{  
//                CurrentCommandSource = this;
//                CurrentParseResult = context.ParseResult;
//                return 0;
//             }});
//   }}


//}}
//";

//            var dom = new GenerateCommandSource().CreateCode(descriptor);
//            var actual = new CSharpGenerator().Generate(dom);

//            Approvals.Verify(actual);
//            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
//        }

//        [Fact]
//        public void Command_with_subcommand()
//        {
//            var descriptor = new CliDescriptor();
//            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
//            descriptor.CommandDescriptor.CliName = "my-command";
//            descriptor.CommandDescriptor.Name = "MyCommand";
//            descriptor.CommandDescriptor.SubCommands.Add(new CommandDescriptor(descriptor.CommandDescriptor, "SubCommand", null));

//            var dom = new GenerateCommandSource().CreateCode(descriptor);
//            var actual = new CSharpGenerator().Generate(dom);

//            Approvals.Verify(actual);
//        }

//        [Fact]
//        public void Command_with_option()
//        {
//            var descriptor = new CliDescriptor();
//            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
//            descriptor.CommandDescriptor.Options.Add(new OptionDescriptor(descriptor.CommandDescriptor, "Option1", null));

//            var dom = new GenerateCommandSource().CreateCode(descriptor);
//            var actual = new CSharpGenerator().Generate(dom);

//            Approvals.Verify(actual);
//        }

//        [Fact]
//        public void Command_with_argument()
//        {
//            var descriptor = new CliDescriptor();
//            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
//            descriptor.CommandDescriptor.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), descriptor.CommandDescriptor, "Argument1", null));
//            var dom = new GenerateCommandSource().CreateCode(descriptor);
//            var actual = new CSharpGenerator().Generate(dom);

//            Approvals.Verify(actual);
//        }


//        [Fact(Skip = "")]
//        public async System.Threading.Tasks.Task Compiled_command_has_expected_valuesAsync()
//        {
//            var descriptor = new CliDescriptor();
//            descriptor.CommandDescriptor = new CommandDescriptor(null, "MyCommand", null);
//            descriptor.CommandDescriptor.CliName = "my-command";

//            var dom = new GenerateCommandSource().CreateCode(descriptor);
//            var actual = new CSharpGenerator().Generate(dom);

//            using var kernel = new CSharpKernel();
//            await SetupKernel(kernel);

//            var result = await kernel.SubmitCodeAsync(actual);
//            // While assert in arrange is unusual, if this goes bad, the test is toast.
//            result.KernelEvents.ToSubscribedList().Should().NotContainErrors();

//            //// act
//            //var resultWithInstance = await kernel.SubmitCodeAsync($"new {className}().{methodName}()");
//            //resultWithInstance.KernelEvents.ToSubscribedList().Should().NotContainErrors();

//            //// assert
//            //var returnValue = await resultWithInstance.KernelEvents.OfType<ReturnValueProduced>().SingleAsync();
//            //var foo = returnValue.Value;
//            //var cmd = foo as Command;
//            //cmd.Name.Should().Be("my-class");

//        }

//        private async Task SetupKernel(CSharpKernel kernel)
//        {
//            var code = $@"
//#r ""{typeof(RequiredAttribute).Assembly.Location}""
//#r ""{typeof(CommandSource).Assembly.Location}""
//#r ""{typeof(Command).Assembly.Location}""
//#r ""{typeof(object).Assembly.Location}""
//";

//            await kernel.SubmitCodeAsync(code);
//        }
//    }
//}
