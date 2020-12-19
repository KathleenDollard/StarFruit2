using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StarFruit2;
using StarFruit2.Generate;
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
             var actual = SourceGeneratorUtilities.GenerateCSharpOutput<Generator>(source1);

            Approvals.Verify(actual.FirstOrDefault());

        }

        [Fact]
        public void CommandSourceResult_generator_produces_correct_code()
        {
            var actual = SourceGeneratorUtilities.GenerateCSharpOutput<Generator>(source1);

            Approvals.Verify(actual.Skip(1).FirstOrDefault());
        }
    }
}
