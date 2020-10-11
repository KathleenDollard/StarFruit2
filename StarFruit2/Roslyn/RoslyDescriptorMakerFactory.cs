using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common.Descriptors;
using System;

namespace Starfruit2_B
{
    public static class RoslyDescriptorMakerFactory
    {
        public static CliDescriptor CreateCliDescriptor<T>(T source, CSharpCompilation?compilation )
            where T : Microsoft.CodeAnalysis.SyntaxNode
        => source switch
        {
            // The common case is a class declaration. If a method declaration is used, any properties in the contained class are ignored. 
            // Is this so confusing we should we even expose the method approach - or is it important for static methods? 
            ClassDeclarationSyntax classDeclaration => new ClassSyntaxCommandMaker().CreateCliDescriptor(null, classDeclaration, compilation),
            MethodDeclarationSyntax methodDeclaration => new MethodSyntaxCommandMaker().CreateCliDescriptor(null, methodDeclaration, compilation),
            _ => throw new InvalidOperationException("Unexpected syntax type")
        };

        //public static CommandDescriptor CreateCommandDescriptor(SyntaxNode source, Compilation compilation)
        //{
        //    return compilation switch
        //    {
        //        CSharpCompilation c => CreateCommandDescriptor(source, c),
        //        _ => throw new NotImplementedException()
        //    };
        //}

        //private static CommandDescriptor CreateCommandDescriptor(SyntaxNode source, CSharpCompilation compilation)
        //{
        //    return source switch
        //    {
        //        ClassDeclarationSyntax classDeclaration => CreateCommandDescriptor(null, classDeclaration, compilation),
        //        MethodDeclarationSyntax methodDeclaration => CreateCommandDescriptor(null, methodDeclaration, compilation),
        //        CompilationUnitSyntax compUnitSyntax => CreateCommandDescriptor(null, compUnitSyntax, compilation),
        //        _ => throw new InvalidOperationException("Unexpected syntax type")
        //    };
        //}

        //private static CommandDescriptor CreateCommandDescriptor(object p, CompilationUnitSyntax source, CSharpCompilation compilation)
        //{
        //    SemanticModel symbol = compilation.GetSemanticModel(source.SyntaxTree);
        //    throw new NotImplementedException();
        //}

        //private static CommandDescriptor CreateCommandDescriptor(object p, MethodDeclarationSyntax source, CSharpCompilation compilation)
        //{
        //    SemanticModel symbol = compilation.GetSemanticModel(source.SyntaxTree);
        //    throw new NotImplementedException();
        //}

        //private static CommandDescriptor CreateCommandDescriptor(object p, ClassDeclarationSyntax source, CSharpCompilation compilation)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
