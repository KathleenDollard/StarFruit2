using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using StarFruit2.Generate;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class VBGeneratorUtilities : SourceGeneratorUtilities
    {
        public override IEnumerable<SyntaxNode>? StatementsForMethod(string source, string methodName)
        {
            Compilation cliRootCompilation = GetCliRootCompilation(source)
                                                   ?? throw new InvalidOperationException();
            var tree = cliRootCompilation.SyntaxTrees.First().GetRoot();
            var methodBlock = tree.DescendantNodes()
                                 .OfType<MethodBlockBaseSyntax>()
                                 // .Where(x => x.Identifier.ValueText == methodName)
                                 .First()
                                 /// .Body
                                 ?.Statements;
            return methodBlock;
        }

        public override IEnumerable<SyntaxNode> ClassDeclarations(string source)
        {
            Compilation cliRootCompilation = GetCliRootCompilation(source)
                                                   ?? throw new InvalidOperationException();
            var tree = cliRootCompilation.SyntaxTrees.First().GetRoot();
            return tree.DescendantNodes()
                                 .OfType<ClassBlockSyntax>();
        }

        public override SyntaxTree ParseToSyntaxTree(string source)
            => VisualBasicSyntaxTree.ParseText(source);

        public override Compilation CreatCompilation(string compilationName, SyntaxTree syntaxTree, IEnumerable<MetadataReference> references, OutputKind outputKind)
            => VisualBasicCompilation.Create(compilationName, new SyntaxTree[] { syntaxTree }, references, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


        public override GeneratorDriver CreateGeneratorDriver(ISourceGenerator generator)
            => VisualBasicGeneratorDriver.Create(ImmutableArray.Create(generator));

        public override ISourceGenerator CreateGenerator()
            => new VBGenerator();
    }
}
