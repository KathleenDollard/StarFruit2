using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Starfruit2;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2.Generate
{
    public static class RoslynCSharpDescriptorFactory
    {
        public static ISymbol? GetSymbol(SyntaxNode syntaxNode,
                                         Compilation compilation,
                                         Dictionary<ISymbol, SemanticModel> semanticModels)
        {
            var _ = compilation ?? throw new ArgumentException("Compilation cannot be null", "compilation");
            var semanticModel = compilation.GetSemanticModel(syntaxNode.SyntaxTree);
            ISymbol? symbol = syntaxNode switch
            {
                ClassDeclarationSyntax classDeclaration => semanticModel.GetDeclaredSymbol(classDeclaration),
                MethodDeclarationSyntax methodDeclaration => semanticModel.GetDeclaredSymbol(methodDeclaration),
                IdentifierNameSyntax identifierName => SymbolFromIdentifier(identifierName, semanticModel),
                _ => throw new InvalidOperationException("Unexpected syntax type")
            };
            if (symbol is not null)
            {
                semanticModels[symbol] = semanticModel;
            }
            return symbol;

            static ISymbol? SymbolFromIdentifier(IdentifierNameSyntax identifierName, SemanticModel semanticModel)
            {
                var symbolInfo= semanticModel.GetSymbolInfo(identifierName);
                return symbolInfo.Symbol is null
                        ? symbolInfo.CandidateSymbols.SingleOrDefault()
                        : symbolInfo.Symbol;
            }
        }

        public static CliDescriptor GetCliDescriptor(ISymbol symbol, SemanticModel semanticModel)
        {
            var config = new MakerConfiguration(new CSharpLanguageHelper());
            var maker = new ClassBasedDescriptorMaker(config, semanticModel);
            return maker.CreateCliDescriptor(null, symbol);
        }

        //public static CliDescriptor CreateCliDescriptor(SyntaxNode syntaxNode, CSharpCompilation? compilation)
        //{
        //    var _ = compilation ?? throw new ArgumentException("Compilation cannot be null", "compilation");
        //    var semanticModel = compilation.GetSemanticModel(syntaxNode.SyntaxTree);
        //    var config = new MakerConfiguration(new CSharpLanguageHelper());
        //    return syntaxNode switch
        //    {
        //        // The common case is a class declaration. If a method declaration is used, any properties in the contained class are ignored. 
        //        // Is this so confusing we should we even expose the method approach - or is it important for static methods? 
        //        ClassDeclarationSyntax classDeclaration => CliDescriptorFromClassDeclaration(config, semanticModel, classDeclaration  ),
        //        MethodDeclarationSyntax methodDeclaration => CliDescriptorFromMethodDeclaration(config, semanticModel, methodDeclaration),
        //        IdentifierNameSyntax identifierName => CliDescriptorFromIdentiferName(config, semanticModel, identifierName),
        //        _ => throw new InvalidOperationException("Unexpected syntax type")
        //    };
        //}

        //private static CliDescriptor CliDescriptorFromIdentiferName(MakerConfiguration config,
        //                                                            SemanticModel semanticModel,
        //                                                            IdentifierNameSyntax source)
        //{
        //    var maker = new ClassBasedDescriptorMaker(config, semanticModel);
        //    var typeSymbol = semanticModel.GetSymbolInfo(source).Symbol as INamedTypeSymbol;
        //    Assert.NotNull(typeSymbol);
        //    return maker.CreateCliDescriptor(null, typeSymbol);
        //}

        //private static CliDescriptor CliDescriptorFromClassDeclaration(MakerConfiguration config,
        //                                                               SemanticModel semanticModel,
        //                                                               ClassDeclarationSyntax source)
        //{
        //    var maker = new ClassBasedDescriptorMaker(config, semanticModel);
        //    var typeSymbol = semanticModel.GetDeclaredSymbol(source) as INamedTypeSymbol ;
        //    Assert.NotNull(typeSymbol);
        //    return maker.CreateCliDescriptor(null, typeSymbol);
        //}

        //private static CliDescriptor CliDescriptorFromMethodDeclaration(MakerConfiguration config,
        //                                                                SemanticModel semanticModel,
        //                                                                MethodDeclarationSyntax source)
        //{
        //    var maker = new MethodBasedDescriptorMaker(config, semanticModel);
        //    var typeSymbol = semanticModel.GetDeclaredSymbol(source);
        //    Assert.NotNull(typeSymbol);
        //    return maker.CreateCliDescriptor(null, typeSymbol);
        //}


    }
}
