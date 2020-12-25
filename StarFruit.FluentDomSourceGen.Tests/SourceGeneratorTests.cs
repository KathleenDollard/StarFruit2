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
    public class WhereSingleLayerSource
    {
        private readonly ITestOutputHelper _output;

        public WhereSingleLayerSource(ITestOutputHelper output)
        {
            _output = output;
        }

        private const string source1 = @"
using StarFruit2.Common;
using System.Threading.Tasks;

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

  

    }
}
