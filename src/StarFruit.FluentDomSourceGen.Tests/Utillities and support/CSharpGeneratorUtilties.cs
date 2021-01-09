using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using StarFruit2.Generate;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class CSharpGeneratorUtilities : SourceGeneratorUtilities
    {

        public override IEnumerable<SyntaxNode>? StatementsForMethod(string source, string methodName)
        {
            Compilation cliRootCompilation = GetCliRootCompilation(source)
                                                 ?? throw new InvalidOperationException();
            var tree = cliRootCompilation.SyntaxTrees.First().GetRoot();
            var methodBlock = tree.DescendantNodes()
                                 .OfType<MethodDeclarationSyntax>()
                                 .Where(x => x.Identifier.ValueText == methodName)
                                 .First()
                                 .Body
                                 ?.Statements;
            return methodBlock;
        }

        public override IEnumerable<SyntaxNode> ClassDeclarations(string source)
        {
            Compilation cliRootCompilation = GetCliRootCompilation(source)
                                                   ?? throw new InvalidOperationException();
            var tree = cliRootCompilation.SyntaxTrees.First().GetRoot();
            return tree.DescendantNodes()
                                 .OfType<ClassDeclarationSyntax>();
        }

        public override SyntaxTree ParseToSyntaxTree(string source)
            => CSharpSyntaxTree.ParseText(source);

        public override Compilation CreatCompilation(string compilationName, SyntaxTree syntaxTree, IEnumerable<MetadataReference> references, OutputKind outputKind)
            => CSharpCompilation.Create(compilationName, new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        public override GeneratorDriver CreateGeneratorDriver(ISourceGenerator generator)
            => CSharpGeneratorDriver.Create(generator);

        public override ISourceGenerator CreateGenerator()
            => new CSharpGenerator();

    }
}
