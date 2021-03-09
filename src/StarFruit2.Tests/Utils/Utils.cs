using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generate;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace StarFruit2.Tests
{
    internal static class Utils
    {
        private const string testNamespace = "StarFruit2.Tests";

        #region string methods
        internal static string Normalize(string value)
        {
            if (value is null)
            {
                throw new InvalidOperationException("You cannot normalize strings that are null");
            }
            // normalize unicode, combining characters, diacritics
            value = value.Normalize(NormalizationForm.FormC);
            value = value.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = value.Split("\n");
            value = string.Join("\n", lines.Select(x => x.Trim()));
            value = ReplaceWithRepeating(value, "\n\n", "\n");
            value = ReplaceWithRepeating(value, "  ", " ");

            return value.Trim();
        }

        internal static ISymbol? GetClassSymbol(SemanticModel semanticModel, string v)
        {
            var node = semanticModel.SyntaxTree.GetRoot()
                                 .DescendantNodes()
                                 .OfType<ClassDeclarationSyntax>()
                                 .Where(p => p.Identifier.ToString() == "XmlCommentTestCode")
                                 .Single();
            return semanticModel.GetDeclaredSymbol(node);
        }

        internal static IMethodSymbol? GetMethodSymbol(SemanticModel semanticModel, string v)
        {
            var node = semanticModel.SyntaxTree.GetRoot()
                                      .DescendantNodes()
                                      .OfType<MethodDeclarationSyntax>()
                                      .Where(p => p.Identifier.ToString() == "MyMethod")
                                      .Single();
            return semanticModel.GetDeclaredSymbol(node);
        }

        internal static ISymbol? GetPropertySymbol(SemanticModel semanticModel, string v)
        {
            var node = semanticModel.SyntaxTree.GetRoot()
                                 .DescendantNodes()
                                 .OfType<PropertyDeclarationSyntax>()
                                 .Where(p => p.Identifier.ToString() == "MyProperty")
                                 .Single();
            return semanticModel.GetDeclaredSymbol(node);
        }

        internal static IEnumerable<string> Normalize(IEnumerable<string> values)
            => values.Select(v => Normalize(v));

        private static string ReplaceWithRepeating(string value, string oldValue, string newValue)
        {
            var len = value.Length + 1; // plus one forces this to run at least once
            while (len != value.Length)
            {
                len = value.Length;
                value = value.Replace(oldValue, newValue);
            }

            return value;
        }
        #endregion

        #region Create CliDescriptors and other descriptors
        internal static CliDescriptor GetCliFromFile(string fileName)
            => GetClassBasedCli(File.ReadAllText($"TestData/{fileName}"));

        internal static CliDescriptor GetClassBasedCli(string code)
            => GetCli<ClassDeclarationSyntax>(code);

        internal static CliDescriptor GetMethodBasedCli(string code)
            => GetCli<MethodDeclarationSyntax>(code);

        private static CliDescriptor? GetCli<T>(string code)
            where T : SyntaxNode
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = GetCompilation(tree);
            var warningsAndErrors = compilation.GetDiagnostics().Where(x => x.Severity.HasFlag(DiagnosticSeverity.Error) || x.Severity.HasFlag(DiagnosticSeverity.Warning));
            warningsAndErrors.Should().NotHaveWarningsOrErrors();
            var rootCommand = tree.GetRoot().DescendantNodes()
                               .OfType<T>()
                               .FirstOrDefault();
            if (rootCommand is null)
            {
                return null;
            }
            var (symbols, semanticModels) = RoslynHelpers.GetSymbolsAndSemanticModels(compilation, rootCommand );
            var symbol = symbols.FirstOrDefault();
            return symbol is null
                   ? null
                   : RoslynDescriptorFactory.GetCliDescriptor( symbol, semanticModels[symbol]);
        }

        internal static OptionDescriptor CreateOptionDescriptor(string name, Type type)
        {
            var option = new OptionDescriptor(null, name, RawInfo.DummyProperty);
            option.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(type), null, name, RawInfo.DummyProperty));
            return option;
        }

        internal static CliDescriptor WrapInCliDescriptor(this CommandDescriptor commandDescriptor)
            => new CliDescriptor
            {
                CommandDescriptor = commandDescriptor
            };

        internal static CliDescriptor WrapInCliDescriptor(this OptionDescriptor optionDescriptor)
        {
            var clidDescriptor = new CliDescriptor()
            {
                CommandDescriptor = new CommandDescriptor(null, "MyClass", RawInfo.DummyClass)

            };
            clidDescriptor.CommandDescriptor.Options.Add(optionDescriptor);
            return clidDescriptor;
        }

        internal static CliDescriptor WrapInCliDescriptor(this ArgumentDescriptor argumentDescriptor)
        {
            var clidDescriptor = new CliDescriptor()
            {
                CommandDescriptor = new CommandDescriptor(null, "MyClass", RawInfo.DummyClass),
                GeneratedComandSourceNamespace = "MyNamespace"
            };
            clidDescriptor.CommandDescriptor.Arguments.Add(argumentDescriptor);
            return clidDescriptor;
        }
        #endregion

        #region Create Roslyn elements
        internal static CSharpCompilation GetCompilation(SyntaxTree syntaxTree)
        {
            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            return CSharpCompilation.Create("foo", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        internal static  SemanticModel GetSemanticModel(string fileName)
        {
            var code = File.ReadAllText($"{fileName}");
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = GetCompilation(tree);
            var model = compilation.GetSemanticModel(tree);
            return  model;
        }

        #endregion

    }
}
