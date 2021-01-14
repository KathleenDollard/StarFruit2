using FluentAssertions;
using FluentAssertions.Execution;
using FluentDom.Generator;
using Microsoft.CodeAnalysis;
using RoslynSourceGenSupport;
using StarFruit2.Generate;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public abstract class SourceGeneratorUtilities
    {
        public static SourceGeneratorUtilities LanguageUtils(bool useVB)
             => useVB
                 ? new VBGeneratorUtilities()
                 : new CSharpGeneratorUtilities();

        public static CandidateSyntaxVisitor Receiver(bool useVB)
            => useVB
                ? new VBSyntaxReceiver()
                : new CSharpSyntaxReceiver();

        public static string LanguageName(bool useVB)
             => useVB
                 ? "VB"
                 : "CSharp";

        public static string Extension(bool useVB)
             => useVB
                 ? "vb"
                 : "cs";

        public static string ShortenedGeneratedCodeName(string generatedCodeName)
            => generatedCodeName.EndsWith("CommandSource")
                 ? "CmdSrc"
                 : "CmdSrcResult";

        public static IEnumerable<SyntaxNode> ClassDeclarations(bool useVB, string source)
            => LanguageUtils(useVB).ClassDeclarations(source);

        public static IEnumerable<SyntaxNode>? StatementsForMethod(bool useVB, string source, string methodName)
            => LanguageUtils(useVB).StatementsForMethod(source, methodName);

        public bool IsVB
            => this is VBGeneratorUtilities;

        public abstract IEnumerable<SyntaxNode>? StatementsForMethod(string source, string methodName);
        public abstract IEnumerable<SyntaxNode> ClassDeclarations(string source);
        public abstract SyntaxTree ParseToSyntaxTree(string source);
        public abstract Compilation CreateCompilation(string compilationName, SyntaxTree syntaxTree, IEnumerable<MetadataReference> references, OutputKind outputKind);
        public abstract GeneratorDriver CreateGeneratorDriver(ISourceGenerator generator);
        public abstract ISourceGenerator CreateGenerator();

        public virtual string GenerateAndTest(string generatedCodeName, string inputFileName, string outputPath, bool isExe = false)
        {
            var cliRootSource = File.ReadAllText(inputFileName);
            return GenerateAndTestSource(generatedCodeName, cliRootSource, outputPath, isExe);
        }

        public virtual string GenerateAndTestSource(string generatedCodeName, string inputSource, string? outputPath = null, bool isExe = false)
        {
            var outputFileName = GetOutputFileName(outputPath, generatedCodeName, IsVB);
            using var assertionScope = new AssertionScope(outputFileName);
            assertionScope.AddReportable("OutputFileName", outputFileName);

            Compilation cliRootCompilation = GetCliRootCompilation(inputSource, isExe)
                ?? throw new InvalidOperationException();

            var requestedSyntaxTree = Generate(cliRootCompilation, out var outputCompilation, out var generationDiagnostics)
                                        .Where(x => x.compilationName.Contains($"{generatedCodeName}.generated"))
                                        .Select(x => x.syntaxTree)
                                        .FirstOrDefault();

            ReportDiagnostics(generatedCodeName, generationDiagnostics, outputCompilation);
            OutputIfRequested(outputFileName, requestedSyntaxTree);

            return requestedSyntaxTree is null
                    ? "Compilation is null"
                    : requestedSyntaxTree.ToString();

            static string? GetOutputFileName(string? outputPath, string generatedCodeName, bool isVB) => outputPath is null
                ? null
                : $"{outputPath}/{generatedCodeName}.generated."
                  + (isVB ? "vb" : "cs");
            static void OutputIfRequested(string? fileName, SyntaxTree requestedSyntaxTree)
            {
                if (fileName is not null)
                {
                    File.WriteAllText(fileName, requestedSyntaxTree.ToString());
                }
            }
            static void ReportDiagnostics(string generatedCodeName, ImmutableArray<Diagnostic> generationDiagnostics, Compilation outputCompilation)
            {
                generationDiagnostics.Should().NotHaveErrors($"{generatedCodeName} - Generation diagnostics");
                outputCompilation.Should().NotHaveErrors($"{generatedCodeName} - Generation compilation");
            }
        }

        internal static GeneratorBase Generator(bool useVB)
            => GeneratorBase.Generator(useVB ? LanguageNames.VisualBasic : LanguageNames.CSharp);

        public virtual Compilation? GetCliRootCompilation(string cliRootSource, bool isExe = false)
        {
            cliRootSource.Should().NotBeNull();
            var cliRootCompilation = CompileSource(cliRootSource, "cliRoot", isExe);
            cliRootCompilation!.Should().NotBeNull();
            cliRootCompilation!.Should().NotHaveErrors($"CliRoot compilation");
            return cliRootCompilation;
        }

        public virtual CandidateSyntaxVisitor FindCandidatesWithSyntaxReceiver(string inputFileName)
            => FindCandidatesWithSyntaxReceiverSource(File.ReadAllText(inputFileName));

        public virtual CandidateSyntaxVisitor FindCandidatesWithSyntaxReceiverSource(string source)
        {
            var _ = source ?? throw new ArgumentException("Source cannot be null", "source");
            var syntaxTree = ParseToSyntaxTree(source);
            var receiver = Receiver(this is VBGeneratorUtilities);
            var walker = new GeneratorSyntaxWalker(receiver);
            walker.Visit(syntaxTree.GetRoot());

            return receiver;
        }

        public virtual IEnumerable<(string compilationName, SyntaxTree syntaxTree)> Generate(Compilation compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> generationDiagnostics)
        {
            ISourceGenerator generator = CreateGenerator();

            var driver = CreateGeneratorDriver(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out outputCompilation, out generationDiagnostics);

            var output = outputCompilation.SyntaxTrees.Select(x => (x.FilePath, x));
            return output;
        }

        public virtual Compilation? CompileSource(string source, string? compilationName = null, bool isExe = false)
        {
            if (source is null)
            {
                return null;
            }
            var syntaxTree = ParseToSyntaxTree(source);
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

            var kind = isExe ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary;
            return CreateCompilation(compilationName, syntaxTree, references, kind);
        }
    }
}
