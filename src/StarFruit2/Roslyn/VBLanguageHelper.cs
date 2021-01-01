﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Linq;

namespace StarFruit2
{
    public class VBLanguageHelper : LanguageHelper
    {
        internal VBLanguageHelper()
        { }

        public override string? GetDefaultValue(IPropertySymbol propertySymbol)
        {
            var declaration = propertySymbol.DeclaringSyntaxReferences.First();
            var propertyDeclaration = declaration.GetSyntax() as PropertyStatementSyntax;
            if (propertyDeclaration?.Initializer is null)
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

        protected override string ArgTypeInfoAsStringInternal(object? typeRepresentation)
            => typeRepresentation switch
            {
                Type t => t.Name,
                INamedTypeSymbol t => t.Name,
                PredefinedTypeSyntax p => p.ToString(),
                IdentifierNameSyntax i => i.Identifier.ToString(),
                _ => typeRepresentation?.ToString() ?? ""
            };
    }

}