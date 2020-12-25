using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StarFruit2.Generate;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class SourceGeneratorUtilities
    {
        public static void GenerateAndTest(string generatedCodeName, string inputFileName, string outputPath)
        {
            var cliRootSource = File.ReadAllText(inputFileName);
            GenerateAndTestSource(generatedCodeName, cliRootSource, outputPath);
        }

        public static string? GenerateAndTestSource(string generatedCodeName, string inputSource, string? outputPath = null)
        {
            using var _ = new AssertionScope();
            CSharpCompilation cliRootCompilation = GetCliRootCompilation(inputSource)
                                                   ?? throw new InvalidOperationException();

            var outputCompilations = SourceGeneratorUtilities.Generate<Generator>(cliRootCompilation, out var outputCompilation, out var generationDiagnostics);
            generationDiagnostics.Should().NotHaveErrors();
            outputCompilation.Should().NotHaveErrors();

            var (compilationName, sourceCode) = outputCompilations.Where(x => x.compilationName.Contains($"{generatedCodeName}.generated"))
                                                                               .FirstOrDefault();
            var commandSourceCompilation = SourceGeneratorUtilities.CompileSource(sourceCode, generatedCodeName);

            if (outputPath is not null)
            {
                File.WriteAllText($"{outputPath}/{generatedCodeName}.generated.cs", sourceCode);
            }
            commandSourceCompilation!.Should().NotBeNull();
            commandSourceCompilation!.Should().NotHaveErrors();

            return commandSourceCompilation?.SyntaxTrees.First().ToString();
        }

        public static CSharpCompilation? GetCliRootCompilation(string cliRootSource)
        {
            cliRootSource.Should().NotBeNull();
            var cliRootCompilation = SourceGeneratorUtilities.CompileSource(cliRootSource, "cliRoot");
            cliRootCompilation!.Should().NotBeNull();
            cliRootCompilation!.Should().NotHaveErrors();
            return cliRootCompilation;
        }

        public static IEnumerable<(string compilationName, string sourceCode)> Generate<T>(CSharpCompilation compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> generationDiagnostics)
            where T : ISourceGenerator, new()
        {
            ISourceGenerator generator = new T();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out outputCompilation, out  generationDiagnostics);

            var output = outputCompilation.SyntaxTrees.Select(x => (x.FilePath, x.ToString()));
            return output;
        }

        public static CSharpCompilation? CompileSource(string source, string? compilationName = null)
        {
            if (source is null)
            {
                return null;
            }
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            compilationName ??= "defaultCompilation";

            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var compilation = CSharpCompilation.Create(compilationName, new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return compilation;
        }
    }
}
