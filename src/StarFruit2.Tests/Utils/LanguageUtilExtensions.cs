using Microsoft.CodeAnalysis;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2.Tests
{
    public static class LanguageUtilExtensions
    {
        internal static (LanguageUtils utils, string code) WrapInClass(this (LanguageUtils utils, string code) utilsAndCode,
                                                                       string className = LanguageUtils.StandardClassName,
                                                                       string namespaceName = LanguageUtils.StandardNamespace,
                                                                       params string[] usings)
        {
            var utils = utilsAndCode.utils;
            var code = utilsAndCode.code;
            code = utils.WrapInClass(code, className, namespaceName);
            code = utils.PrefaceWithUsing(code, CombineUsings(usings, "StarFruit2","StarFruit.Common"));
            return (utils, code);
        }

        internal static (LanguageUtils utils, string code) WrapInNamespace(this (LanguageUtils utils, string code) utilsAndCode,
                                                                           string namespaceName = LanguageUtils.StandardNamespace,
                                                                           params string[] usings)
        {
            var utils = utilsAndCode.utils;
            var code = utilsAndCode.code;
            code = utils.WrapInNamespace(code, namespaceName);
            code = utils.PrefaceWithUsing(code, CombineUsings(usings, "StarFruit2", "StarFruit.Common"));
            return (utils, code);
        }

        private static string[] CombineUsings(IEnumerable<string> usings, params string[] moreUsings)
        {
            var list = usings.ToList();
            list.AddRange(moreUsings);
            return list.ToArray();
        }

        internal static CliDescriptor CliDescriptorForClass(this (LanguageUtils utils, string inputCode) utilsAndCode, out string code)
        {
            var descriptor = GetCliDescriptor(utilsAndCode, out string innerCode, (utils, syntaxTree) => utils.GetClassNodes(syntaxTree));
            code = innerCode;
            return descriptor;
        }

        internal static CliDescriptor CliDescriptorForMethod(this (LanguageUtils utils, string inputCode) utilsAndCode, out string code)
        {
            var descriptor = GetCliDescriptor(utilsAndCode, out string innerCode, (utils, syntaxTree) => utils.GetMethodNodes(syntaxTree));
            code = innerCode;
            return descriptor;
        }

        private static CliDescriptor GetCliDescriptor(this (LanguageUtils utils, string inputCode) utilsAndCode, out string code, Func<LanguageUtils, SyntaxTree,IEnumerable<SyntaxNode>> syntaxNodeSelector)
        {
            var utils = utilsAndCode.utils;
            code = utilsAndCode.inputCode;
            var syntaxTree = utils.GetSyntaxTree(code);
            _ = syntaxTree ?? throw new InvalidOperationException("SyntaxTree could not be created");

            var compilation = utils.GetCompilation(syntaxTree);
            var warningsAndErrors = compilation.GetDiagnostics().Where(x => x.Severity.HasFlag(DiagnosticSeverity.Error) || x.Severity.HasFlag(DiagnosticSeverity.Warning));
            _ = warningsAndErrors.Any()
                        ? throw new InvalidOperationException($"Compilation could not be created{DiagnosticList(warningsAndErrors)}")
                        : true;

            var cliSyntax = syntaxNodeSelector(utils, syntaxTree)
                                 .FirstOrDefault();
            _ = cliSyntax ?? throw new InvalidOperationException("No matching syntax node found");

            var semanticModels = new Dictionary<ISymbol, SemanticModel>();
            var symbol = RoslynHelpers.GetSymbol(cliSyntax, compilation, semanticModels);
            _ = symbol ?? throw new InvalidOperationException("No matching symbol found");

            return RoslynDescriptorFactory.GetCliDescriptor( symbol, semanticModels[symbol]);

            static string DiagnosticList(IEnumerable<Diagnostic> diagnostics)
            {
                return string.Join("\n\t", diagnostics.Select(x => x.ToString().Replace("{", "{{")));
            }
        }

    }
}
