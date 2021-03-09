using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentDom;
using FluentDom.Generator;

namespace StarFruit2.Generate
{
    [Generator(LanguageNames.CSharp )]
    public class CSharpGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new CSharpSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                //Debugger.Launch();
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("KD0001", "Generator entered", "", "KDGenerator", DiagnosticSeverity.Info, true),
                    null));

                if (context.SyntaxReceiver is not CSharpSyntaxReceiver receiver)
                    return;

                var (symbols, semanticModels) = RoslynHelpers.GetSymbolsAndSemanticModels(context.Compilation, receiver.Candidates.ToArray());

                var cliDescriptors = symbols.Select(symbol => RoslynDescriptorFactory.GetCliDescriptor( symbol!, semanticModels[symbol!]))
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
                var source = GeneratorBase.Generator(LanguageNames.CSharp).Generate(code);
                if (source is not null)
                {
                    context.AddSource(fileName, source);
                }
            }

        }
    }
}
