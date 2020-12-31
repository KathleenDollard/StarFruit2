using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using StarFruit2.Generate;
using System;
using System.IO;
using System.Linq;
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

        // TODO: Consider whether tests based on this const are redundant with those based on files. Perhaps remove CliRoot details

        // These using choices are deliberate.
        // * StarFruit2.Common is redundant with default, 
        // * System.Threading.Tasks may or may not be in the defualt generation,
        // * System.IO is not in the generation
        private const string classBasedSource = @"
using StarFruit2;
using StarFruit2.Common;
using System.Threading.Tasks;
using System.IO;

namespace Tests
{
    public class Program
    {
       static int Main(string[] args)
            => CommandSource.Run<CliRoot>(args);
    }
    public class CliRoot
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

        private const string multipleGenericAndInterfaceIndicators = @"
using StarFruit2;
using StarFruit2.Common;
using System.Threading.Tasks;
using System.IO;

namespace Tests
{
    public class Program
    {
       static int Main(string[] args)
       {
          var x = CommandSource.Create<CliRoot>();
          CommandSource.Run<CliRoot>(args);
          return 0;
       }
    }
    public class CliRoot : ICliRoot
    {
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
            var output = SourceGeneratorUtilities.GenerateAndTestSource(generatedCodeName, classBasedSource, null);
            Approvals.Verify(output);

        }

        [Fact]
        public void CommandSourceResult_generator_produces_correct_code()
        {
            var generatedCodeName = "CliRootCommandSourceResult";
            var output = SourceGeneratorUtilities.GenerateAndTestSource(generatedCodeName, classBasedSource, null);
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
        public void CliFromMethod_creates_CommandSource_that_will_compile()
        {
            var generatedCodeName = "FormatCommandSource";
            var outPath = "../../../../StarFruit.FluentDomSourceGen.Tests.FromMethodOutput/";
            var generatedSource = SourceGeneratorUtilities.GenerateAndTest(generatedCodeName, Path.Combine(outPath,"Program.cs"), outPath);
            generatedSource.Length.Should().BeGreaterThan(1000);
        }

        [Fact]
        public void CliFromMethod_creates_CommandSourceResult_that_will_compile()
        {
            var generatedCodeName = "FormatCommandSourceResult";
            var outPath = "../../../../StarFruit.FluentDomSourceGen.Tests.FromMethodOutput/";
            var generatedSource = SourceGeneratorUtilities.GenerateAndTest(generatedCodeName, Path.Combine(outPath, "Program.cs"), outPath);
            generatedSource.Length.Should().BeGreaterThan(1000);
        }

        [Fact]
        public void CliRoot_creates_a_single_CommandSource_set_for_multiple_indicators_of_root()
        {
            var generatedCodeName = "CliRootCommandSource";
            var cliRootCompilation = SourceGeneratorUtilities.GetCliRootCompilation(multipleGenericAndInterfaceIndicators)
                ?? throw new InvalidOperationException();

            var outputPairs = SourceGeneratorUtilities.Generate<Generator>(cliRootCompilation, out var outputCompilation, out var generationDiagnostics);

            var matchingPairs = outputPairs.Where(x => x.compilationName.EndsWith($"{generatedCodeName}.generated.cs"));
            matchingPairs.Should().HaveCount(1);
            generationDiagnostics.Should().NotHaveErrors($"{generatedCodeName} - Generation diagnostics");
            outputCompilation.Should().NotHaveErrors($"{generatedCodeName} - Generation compilation");
        }



        // The intention of this test is for you to copy problematic input code into Temp.cs to allow generator debugging and access to output.
        // If this is resolving a bug, create a test for the simple part of the bug once identified.
        [Fact]
        public void Generate_CommandSource_for_debugging_when_needed()
        {
            SourceGeneratorUtilities.GenerateAndTest("CommandSource", @"..\..\..\Temp.cs", $"../../../../Temp");
        }

        // The intention of this test is for you to copy problematic input code into Temp.cs to allow generator debugging and access to output
        // If this is resolving a bug, create a test for the simple part of the bug once identified.
        [Fact]
        public void Generate_CommandSourceResult_for_debugging_when_needed()
        {
            SourceGeneratorUtilities.GenerateAndTest("CommandSourceResult", @"..\..\..\Temp.cs", $"../../../../Temp");
        }

        // The intention of this test is for you to copy problematic input code into Temp.cs to allow generator debugging and access to output
        // If this is resolving a bug, create a test for the simple part of the bug once identified.
        [Fact]
        public void Test_SyntaxReceiver_for_debugging_when_needed()
        {
            var receiver = SourceGeneratorUtilities.FindCandidatesWithSyntaxReceiver(@"..\..\..\Temp.cs");
            receiver.Candidates.Should().NotBeNullOrEmpty();
            receiver.Candidates.Should().HaveCount(1);
        }

    }
}
