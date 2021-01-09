using ApprovalTests;
using ApprovalTests.Reporters;
using StarFruit.Common;
using StarFruit2;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generate;
using Xunit;
using ApprovalTests.Namers;
using Utils = StarFruit.FluentDomSourceGen.Tests.SourceGeneratorUtilities;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class GenerateFromDescriptorTests
    {

        private const string namespaceName = "";
        private const string className = "MyCommand";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Simple_command(bool useVB)
        {
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null, namespaceName, className))
            };
            descriptor.CommandDescriptor.CliName = "my-command";

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual =Utils.Generator(useVB).Generate(dom);

            NamerFactory.AdditionalInformation = Utils.LanguageName(useVB);
            Approvals.Verify(actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Command_with_subcommand(bool useVB)
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
            var actual = Utils.Generator(useVB).Generate(dom);

            NamerFactory.AdditionalInformation = Utils.LanguageName(useVB);
            Approvals.Verify(actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Command_with_option(bool useVB)
        {
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null, namespaceName, className))
            };
            descriptor.CommandDescriptor.Options.Add(new OptionDescriptor(descriptor.CommandDescriptor, "Option1", new RawInfoForProperty(null, namespaceName, className)));

            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = Utils.Generator(useVB).Generate(dom);

            NamerFactory.AdditionalInformation = Utils.LanguageName(useVB);
            Approvals.Verify(actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Command_with_argument(bool useVB)
        {
            var descriptor = new CliDescriptor
            {
                CommandDescriptor = new CommandDescriptor(null, "MyCommand", new RawInfoForType(null, namespaceName, className))
            };
            descriptor.CommandDescriptor.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), descriptor.CommandDescriptor, "Argument1", new RawInfoForProperty(null, namespaceName, className)));
            var dom = new GenerateCommandSource().CreateCode(descriptor);
            var actual = Utils.Generator(useVB).Generate(dom);

            NamerFactory.AdditionalInformation = Utils.LanguageName(useVB);
            Approvals.Verify(actual);
        }

    }
}
