using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarFruit2
{
    public class ArgTypeInfoFromSyntax : ArgTypeInfo
    {
        public ArgTypeInfoFromSyntax(SyntaxNode syntaxNode)
            : base(syntaxNode)
        { }

        public override string TypeAsString()
        => TypeRepresentation switch
        {
            PredefinedTypeSyntax p => p.ToString(),
            IdentifierNameSyntax i => i.Identifier.ToString(),
            _ => throw new InvalidOperationException()
        };
    }
}
