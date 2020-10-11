using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;

namespace Starfruit2_B
{
    public static class RoslyDescriptorMakerFactory
    {
        public static CliDescriptor CreateCliDescriptor<T>(T source, CSharpCompilation? compilation)
            where T : Microsoft.CodeAnalysis.SyntaxNode
        {
            var semanticModel = GetSemanticModel(source, compilation);
            var config = new MakerConfigurationBase();
            return source switch
            {
                // The common case is a class declaration. If a method declaration is used, any properties in the contained class are ignored. 
                // Is this so confusing we should we even expose the method approach - or is it important for static methods? 
                ClassDeclarationSyntax classDeclaration => CliDescriptorFromClassDeclaration(config, semanticModel, classDeclaration  ),
                MethodDeclarationSyntax methodDeclaration => CliDescriptorFromMethodDeclaration(config, semanticModel, methodDeclaration),
                _ => throw new InvalidOperationException("Unexpected syntax type")
            };
        }

        private static CliDescriptor CliDescriptorFromClassDeclaration(MakerConfigurationBase config,
                                                                       SemanticModel semanticModel,
                                                                       ClassDeclarationSyntax source)
        {
            var maker = new ClassSyntaxCommandMaker(config, semanticModel);
            var typeSymbol = semanticModel.GetDeclaredSymbol(source);
            Assert.NotNull(typeSymbol);
            return maker.CreateCliDescriptor(null, typeSymbol);
        }

        private static CliDescriptor CliDescriptorFromMethodDeclaration(MakerConfigurationBase config,
                                                                        SemanticModel semanticModel,
                                                                        MethodDeclarationSyntax source)
        {
            var maker = new MethodSyntaxCommandMaker(config, semanticModel);
            var typeSymbol = semanticModel.GetDeclaredSymbol(source);
            Assert.NotNull(typeSymbol);
            return maker.CreateCliDescriptor(null, typeSymbol);
        }

        private static SemanticModel GetSemanticModel(SyntaxNode syntax,
                                                      Compilation? compilation)
        {
            if (compilation == null)
            {
                throw new NotImplementedException();
            }
            SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

            return semanticModel;
        }
    }
}
