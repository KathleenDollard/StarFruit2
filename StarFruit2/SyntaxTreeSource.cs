using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StarFruit2
{
    public class SyntaxTreeSource
    {
        public ClassDeclarationSyntax ClassDeclaration { get; set; }
        public PropertyDeclarationSyntax Properties { get; set; }
        public PropertyDeclarationSyntax Methods { get; set; }
        public PropertyDeclarationSyntax Parameters { get; set; }
    }
}
