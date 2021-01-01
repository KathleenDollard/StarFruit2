using FluentAssertions;
using StarFruit2.Common.Descriptors;
using StarFruit2.Tests;

namespace TestData
{
    public class EmptyTestData : BaseTestData
    {
        private const string testName = "Empty";
        private const string generatedNamespace = "StarFruit2.Tests.TestSampleData.Empty";
        private const string generatedClassName = testName + "CommandSource";

        public EmptyTestData()
            : base(testName)
        {
            GeneratedNamespace = generatedNamespace;
            GeneratedSourceClassName = generatedClassName;

            CliDescriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = generatedNamespace,
                GeneratedCommandSourceClassName = generatedClassName,
                CommandDescriptor = new CommandDescriptor(null, "MyClass", RawInfo.DummyClass)
                {
                    Name = "MyClass"
                }
            };

            CommandDefinitionSourceCode = @$"
                var command = new Command(""{SystemCommandLineName}"", ""{CliDescriptor.CommandDescriptor.Description}"");
                return command;";

            TestAction = c => c.Name.Should().Be("my-class");



        }

    }
}
