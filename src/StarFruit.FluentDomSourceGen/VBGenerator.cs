using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentDom;
using FluentDom.Generator;

namespace StarFruit2.Generate
{
    [Generator(LanguageNames.VisualBasic)]
    public class VBGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new VBSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                //Debugger.Launch();
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0001", "Generator entered", "", "KDGenerator", DiagnosticSeverity.Info, true),
                    null));

                if (context.SyntaxReceiver is not VBSyntaxReceiver receiver)
                    return;

                Dictionary<ISymbol, SemanticModel> semanticModels = new();
                // semanticModels updated in GetSymbol
                var symbols = receiver.Candidates
                                      .Select(x => RoslynHelpers.GetSymbol(x, context.Compilation, semanticModels))
                                      .Where(symbol => symbol is not null)
                                      .Distinct();

                var cliDescriptors = symbols.Select(symbol => RoslynDescriptorFactory.GetCliDescriptor( symbol!, semanticModels[symbol!]))
                                            .ToList();

                foreach (var cliDescriptor in cliDescriptors)
                {
                    OutputCode(new GenerateCommandSource().CreateCode(cliDescriptor, receiver.Usings),
                               context,
                               $"{cliDescriptor.CommandDescriptor.OriginalName}CommandSource.generated.vb");
                    OutputCode(new GenerateCommandSourceResult().CreateCode(cliDescriptor, receiver.Usings),
                               context,
                               $"{cliDescriptor.CommandDescriptor.OriginalName}CommandSourceResult.generated.vb");
                }
                var x = semanticModels.Count();
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0002", "Generator failed", ex.ToString(), "KDGenerator", DiagnosticSeverity.Error, true),
                    null));
            }

            static void OutputCode(Code code, GeneratorExecutionContext context, string fileName)
            {
                var source = GeneratorBase.Generator(LanguageNames.VisualBasic ).Generate(code);
                if (source is not null)
                {
                    context.AddSource(fileName, source);
                }
            }

        }
    }
}
