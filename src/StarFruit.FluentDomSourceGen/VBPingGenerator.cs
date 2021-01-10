using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace StarFruit2.Generate
{
    [Generator(LanguageNames.VisualBasic)]
    public class VBPingGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        { }

        public void Execute(GeneratorExecutionContext context)
        {          var source = $"\npublic class TempPing4{{}}\n";
            context.AddSource("generated.cs", source);
        }
    }
}
