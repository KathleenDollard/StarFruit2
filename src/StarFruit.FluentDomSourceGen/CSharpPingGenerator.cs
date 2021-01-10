using Microsoft.CodeAnalysis;

namespace StarFruit2.Generate
{
    [Generator]
    public class CSharpPingGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        { }

        public void Execute(GeneratorExecutionContext context)
        {
            var source = $"\npublic class TempPing4{{}}\n";
            context.AddSource("generated.cs", source);
        }
    }
}
