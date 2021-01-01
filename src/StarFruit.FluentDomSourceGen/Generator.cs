using Microsoft.CodeAnalysis;
using Starfruit2;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentDom;
using FluentDom.Generator;

namespace StarFruit2.Generate
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new CommandSourceSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                //Debugger.Launch();
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0001", "Generator entered", "", "KDGenerator", DiagnosticSeverity.Info, true),
                    null));

                if (context.SyntaxReceiver is not CommandSourceSyntaxReceiver receiver)
                    return;

                Dictionary<ISymbol, SemanticModel> semanticModels = new();
                // semanticModels updated in GetSymbol
                var symbols = receiver.Candidates
                                      .Select(x => RoslynHelpers.GetSymbol(x, context.Compilation, semanticModels))
                                      .Where(symbol => symbol is not null)
                                      .Distinct();

                var cliDescriptors = symbols.Select(symbol => RoslynCSharpDescriptorFactory.GetCliDescriptor(LanguageNames.CSharp, symbol!, semanticModels[symbol!]))
                                            .ToList();

                foreach (var cliDescriptor in cliDescriptors)
                {
                    OutputCode(new GenerateCommandSource().CreateCode(cliDescriptor, receiver.Usings),
                               context,
                               $"{cliDescriptor.CommandDescriptor.OriginalName}CommandSource.generated.cs");
                    OutputCode(new GenerateCommandSourceResult().CreateCode(cliDescriptor, receiver.Usings),
                               context,
                               $"{cliDescriptor.CommandDescriptor.OriginalName}CommandSourceResult.generated.cs");
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0002", "Generator failed", ex.ToString(), "KDGenerator", DiagnosticSeverity.Error, true),
                    null));
            }

            static void OutputCode(Code code, GeneratorExecutionContext context, string fileName)
            {
                var source = new CSharpGenerator().Generate(code);
                if (source is not null)
                {
                    context.AddSource(fileName, source);
                }
            }

        }
    }
}
