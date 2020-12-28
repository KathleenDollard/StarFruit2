using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StarFruit2.Generate;
using TwoLayerCli;
using Xunit;
using Xunit.Abstractions;

[assembly: UseReporter(typeof(VisualStudioReporter))]

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class SourceGeneratorTests
    {
        private readonly ITestOutputHelper _output;

        public SourceGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // These using choices are deliberate.
        // * StarFruit2.Common is redundant with default, 
        // * System.Threading.Tasks may or may not be in the defualt generation,
        // * System.IO is not in the generation
        private const string source1 = @"
using StarFruit2.Common;
using System.Threading.Tasks;
using System.IO;

namespace TwoLayerCli
{
    public class CliRoot : ICliRoot
    {
        // Usage: > cli-root <int> --ctor-param --string-property Fred --string-option Flintstone --bool-option
        private bool ctorParam;
        public CliRoot(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public async Task<int> InvokeAsync(int intArg, string stringOption, bool boolOption)
        { return await Task.FromResult(0); }

    }
}";


        [Fact]
        public void CommandSource_generator_produces_correct_code()
        {
            var generatedCodeName = "CliRootCommandSource";
            var output = SourceGeneratorUtilities.GenerateAndTestSource(generatedCodeName, source1, null);
            Approvals.Verify(output);

        }

        [Fact]
        public void CommandSourceResult_generator_produces_correct_code()
        {
            var generatedCodeName = "CliRootCommandSourceResult";
            var output = SourceGeneratorUtilities.GenerateAndTestSource(generatedCodeName, source1, null);
            Approvals.Verify(output);
        }

        [Fact]
        public void CliRoot_creates_CommandSource_that_will_compile()
        {
            var generatedCodeName = "CliRootCommandSource";
            SourceGeneratorUtilities.GenerateAndTest(generatedCodeName, "CliRoot.cs", $"../../../../StarFruit.FluentDomSourceGen.Tests.Output");
        }

        [Fact]
        public void CliRoot_creates_CommandSourceResult_that_will_compile()
        {
            var generatedCodeName = "CliRootCommandSourceResult";
            SourceGeneratorUtilities.GenerateAndTest(generatedCodeName, "CliRoot.cs", $"../../../../StarFruit.FluentDomSourceGen.Tests.Output");
        }


        [Fact]
        public void Generate_CommandSource_for_debugging_when_needed()
        {
            SourceGeneratorUtilities.GenerateAndTest("CommandSource", @"..\..\..\Temp.cs", $"../../../../Temp");
        }

        [Fact]
        public void Generate_CommandSourceResult_for_debugging_when_needed()
        {
            SourceGeneratorUtilities.GenerateAndTest("CommandSourceResult", @"..\..\..\Temp.cs", $"../../../../Temp");
        }


        [Fact]
        public void Test_SyntaxReceiver_for_debugging_when_needed()
        {
            var receiver = SourceGeneratorUtilities.FindCandidatesWithSyntaxReceiver(@"..\..\..\Temp.cs");
            receiver.CandidateCliTypes.Should().NotBeNullOrEmpty();
            receiver.CandidateCliTypes.Should().HaveCount(1);
        }

    }
}
