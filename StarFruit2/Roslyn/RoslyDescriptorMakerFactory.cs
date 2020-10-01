﻿using Microsoft.CodeAnalysis;
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
            ClassDeclarationSyntax classDeclaration => new CommandMakerClassSyntax().CreateCliDescriptor(null, classDeclaration, compilation),
            MethodDeclarationSyntax methodDeclaration => new CommandMakerMethodSyntax().CreateCliDescriptor(null, methodDeclaration, compilation),
            _ => throw new InvalidOperationException("Unexpected syntax type")
        };

        public static CommandDescriptor CreateCommandDescriptor(SyntaxNode source, Compilation compilation)
        {
            return compilation switch
            {
                CSharpCompilation c => CreateCommandDescriptor(source, c),
                _ => throw new NotImplementedException()
            };
        }

        private static CommandDescriptor CreateCommandDescriptor(SyntaxNode source, CSharpCompilation compilation)
        {
            return source switch
            {
                ClassDeclarationSyntax classDeclaration => CreateCommandDescriptor(null, classDeclaration, compilation),
                MethodDeclarationSyntax methodDeclaration => CreateCommandDescriptor(null, methodDeclaration, compilation),
                CompilationUnitSyntax compUnitSyntax => CreateCommandDescriptor(null, compUnitSyntax, compilation),
                _ => throw new InvalidOperationException("Unexpected syntax type")
            };
        }

        private static CommandDescriptor CreateCommandDescriptor(object p, CompilationUnitSyntax source, CSharpCompilation compilation)
        {
            SemanticModel symbol = compilation.GetSemanticModel(source.SyntaxTree);
            throw new NotImplementedException();
        }

        private static CommandDescriptor CreateCommandDescriptor(object p, MethodDeclarationSyntax source, CSharpCompilation compilation)
        {
            SemanticModel symbol = compilation.GetSemanticModel(source.SyntaxTree);
            throw new NotImplementedException();
        }

        private static CommandDescriptor CreateCommandDescriptor(object p, ClassDeclarationSyntax source, CSharpCompilation compilation)
        {
            throw new NotImplementedException();
        }
    }
}