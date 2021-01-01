using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2.Generate
{
    public class RoslynHelpers
    {
        public static ISymbol? GetSymbol(SyntaxNode syntaxNode,
                                         Compilation compilation,
                                         Dictionary<ISymbol, SemanticModel> semanticModels)
        {
            var _ = compilation ?? throw new ArgumentException("Compilation cannot be null", "compilation");
            var semanticModel = compilation.GetSemanticModel(syntaxNode.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);
            symbol ??= SymbolFromIdentifier(syntaxNode, semanticModel);

            if (symbol is not null)
            {
                semanticModels[symbol] = semanticModel;
            }
            return symbol;

            static ISymbol? SymbolFromIdentifier(SyntaxNode syntaxNode, SemanticModel semanticModel)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(syntaxNode);
                return symbolInfo.Symbol is null
                        ? symbolInfo.CandidateSymbols.SingleOrDefault()
                        : symbolInfo.Symbol;
            }
        }

        //public static ISymbol? GetSymbol(SyntaxNode syntaxNode,
        //                                           Compilation compilation,
        //                                           Dictionary<ISymbol, SemanticModel> semanticModels)
        //{
        //    var _ = compilation ?? throw new ArgumentException("Compilation cannot be null", "compilation");
        //    var semanticModel = compilation.GetSemanticModel(syntaxNode.SyntaxTree);
        //    ISymbol? symbol = syntaxNode switch
        //    {
        //        ClassDeclarationSyntax classDeclaration => semanticModel.GetDeclaredSymbol(classDeclaration),
        //        MethodDeclarationSyntax methodDeclaration => semanticModel.GetDeclaredSymbol(methodDeclaration),
        //        IdentifierNameSyntax identifierName => SymbolFromIdentifier(identifierName, semanticModel),
        //        _ => throw new InvalidOperationException("Unexpected syntax type")
        //    };
        //    if (symbol is not null)
        //    {
        //        semanticModels[symbol] = semanticModel;
        //    }
        //    return symbol;

        //    static ISymbol? SymbolFromIdentifier(IdentifierNameSyntax identifierName, SemanticModel semanticModel)
        //    {
        //        var symbolInfo = semanticModel.GetSymbolInfo(identifierName);
        //        return symbolInfo.Symbol is null
        //                ? symbolInfo.CandidateSymbols.SingleOrDefault()
        //                : symbolInfo.Symbol;
        //    }
        //}
    }
}
