using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common;
using System;
using System.Linq;

namespace StarFruit2
{
    public abstract class LanguageHelper
    {
        public abstract string GetDefaultValue(IPropertySymbol propertySymbol);
    }

    public class CSharpLanguageHelper : LanguageHelper
    {
        public override string? GetDefaultValue(IPropertySymbol propertySymbol)
        {
            var declaration = propertySymbol.DeclaringSyntaxReferences.First();
            var propertyDeclaration = declaration.GetSyntax() as PropertyDeclarationSyntax;
            if( propertyDeclaration?.Initializer is null)
            {
                return null;
            }
            return propertyDeclaration.Initializer.Value switch
            {
                LiteralExpressionSyntax s => s.Token.ValueText,
                ObjectCreationExpressionSyntax s => s.ToString(),
                MemberAccessExpressionSyntax s => s.ToString(),
                _ => $"Type not handled: {propertyDeclaration.Initializer.Value.GetType()}"
            };
        }
    }

    public class VBLanguageHelper : LanguageHelper
    {
        public override string GetDefaultValue(IPropertySymbol propertySymbol)
        {
            throw new System.NotImplementedException();
        }
    }
}