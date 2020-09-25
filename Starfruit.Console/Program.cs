using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2;
using System.Linq;

namespace Starfruit.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText("");

            var maker = new SyntaxTreeDescriptorMaker();
            var command = maker.GetCommandDescriptor(tree.GetRoot().DescendantNodes()
                               .OfType<ClassDeclarationSyntax>()
                               .Single());
        }
    }
}
