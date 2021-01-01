using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2.Tests
{
    public class VBLanguageUtils : LanguageUtils
    {
        public override string PrefaceWithUsing(string code, params string[] usingNamespaces)
            => string.Join("\n", usingNamespaces.Select(x => $"Imports {x}")) + $"\n{code}";

        public override string WrapInClass(string code, string className = StandardClassName, string namespaceName = StandardNamespace)
           => @$"
Namespace {namespaceName}
    Public Class {className}
        {code}
    End Class
End Namespace
";

        public override string WrapInNamespace(string code, string namespaceName)
            => @$"
Namespace {namespaceName}
    {code}
End Namespace
";

        public override SyntaxTree GetSyntaxTree(string code)
           => VisualBasicSyntaxTree.ParseText(code);

        public override Compilation GetCompilation(SyntaxTree syntaxTree)
            => VisualBasicCompilation.Create("compilationForTesting",
                                        new SyntaxTree[] { syntaxTree },
                                        GetAssemblyReferences(),
                                        new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        public override IEnumerable<SyntaxNode> GetClassNodes(SyntaxTree syntaxTree)
        {
            return syntaxTree.GetRoot()
                              .DescendantNodes()
                              .OfType<ClassStatementSyntax>();
        }

        public override IEnumerable<SyntaxNode> GetMethodNodes(SyntaxTree syntaxTree)
        {
            return syntaxTree.GetRoot()
                              .DescendantNodes()
                              .OfType<MethodStatementSyntax>();
        }
    }
}
