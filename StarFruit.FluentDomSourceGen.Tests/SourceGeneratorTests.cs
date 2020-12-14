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

namespace SourceGeneratorTests
{
    public class Tests
    {
        private readonly ITestOutputHelper _output;

        public Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            string source = @"
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
            var actual = GetGeneratedOutput(source);

            Approvals.Verify(actual);

        }

        private string GetGeneratedOutput(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var compilation = CSharpCompilation.Create("foo", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // TODO: Uncomment this line if you want to fail tests when the injected program isn't valid _before_ running generators
            var compileDiagnostics = compilation.GetDiagnostics();
            Assert.False(compileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "Input failed to compile: " + compileDiagnostics.FirstOrDefault()?.GetMessage());

            ISourceGenerator generator = new Generator();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);
            Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "Output failed to compile: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

            string output = outputCompilation.SyntaxTrees.Last().ToString();

            _output.WriteLine(output);

            return output;
        }
    }
}
