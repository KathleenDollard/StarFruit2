using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class SourceGeneratorUtilities
    {

        public static IEnumerable<string?> GenerateCSharpOutput<T>(string source)
            where T : ISourceGenerator, new()
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

            var compileDiagnostics = compilation.GetDiagnostics();
            if (compileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                throw new GeneratorFailException("Input failed to compile: ", compileDiagnostics);
            }

            ISourceGenerator generator = new T();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);
            if (generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                throw new GeneratorFailException("Output failed to compile: ", generateDiagnostics);
            }

            var output = outputCompilation.SyntaxTrees.Skip(1)
                                                      .Select(x => x.ToString());

            return output;
        }
    }
}
