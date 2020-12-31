using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Microsoft.CodeAnalysis;
using StarFruit2.Generate;
using System.Collections.Generic;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class SyntaxReceiverTests
    {
        private const string entrySource = @"
using StarFruit2;
using StarFruit2.Common;

namespace Tests
{
    public class Program
    {
        static int Main(string[] args)
        { 
            CommandSource.Run<CliRoot>(args);
            CommandSource.Create(nameof(RunThisMethod));
            CommandSource.Create(nameof(A.RunThatMethod));
            return 0;
        }
        public static void RunThisMethod() {}
    }
    public class A 
    {
        public static void RunThatMethod() {}
    }
    public class CliRoot : ICliRoot
    { }
}";


        [Fact]
        public void SyntaxReceiver_finds_root_class_via_interface()
        {
            var classDeclarations = GetClassDeclarations(entrySource);
            CommandSourceSyntaxReceiver receiver = new();

            foreach (var classDeclaration in classDeclarations)
            {
                receiver.OnVisitSyntaxNode(classDeclaration);
            }

            receiver.Candidates.Should().HaveCount(1);
            var pocoType = receiver.Candidates.First() as ClassDeclarationSyntax ;
            pocoType.Should().NotBeNull();
            pocoType!.Identifier.ValueText.Should().Be("CliRoot");
        }


        [Fact]
        public void SyntaxReceiver_finds_root_class_in_generic()
        {
            var methodBlock = GetMainMethodStatements(entrySource).GetValueOrDefault();
            methodBlock.Should().NotBeNull();
            CommandSourceSyntaxReceiver receiver = new();

            receiver.OnVisitSyntaxNode(methodBlock.First().ChildNodes().First());

            receiver.Candidates.Should().HaveCount(1);
            var pocoType = receiver.Candidates.First() as IdentifierNameSyntax;
            pocoType.Should().NotBeNull();
            pocoType!.Identifier.ValueText.Should().Be("CliRoot");
        }


        [Fact]
        public void SyntaxReceiver_finds_entry_method_in_class()
        {
            var methodBlock = GetMainMethodStatements(entrySource).GetValueOrDefault();
            methodBlock.Should().NotBeNull();
            CommandSourceSyntaxReceiver receiver = new();

            receiver.OnVisitSyntaxNode(methodBlock.Skip(1).First().ChildNodes().First());

            receiver.Candidates.Should().HaveCount(1);
            var methodType = receiver.Candidates.First() as IdentifierNameSyntax;
            methodType.Should().NotBeNull();
            methodType!.Identifier.ValueText.Should().Be("RunThisMethod");
        }

        [Fact]
        public void SyntaxReceiver_finds_entry_method_in_another_class()
        {
            var methodBlock = GetMainMethodStatements(entrySource).GetValueOrDefault();
            methodBlock.Should().NotBeNull();
            CommandSourceSyntaxReceiver receiver = new();

            receiver.OnVisitSyntaxNode(methodBlock.Skip(2).First().ChildNodes().First());

            receiver.Candidates.Should().HaveCount(1);
            var methodType = receiver.Candidates.First() as MemberAccessExpressionSyntax;
            methodType.Should().NotBeNull();
            var classIdentifier = methodType!.Expression as IdentifierNameSyntax;
            var methodIdentifier = methodType.Name as IdentifierNameSyntax;
            classIdentifier.Should().NotBeNull();
            methodIdentifier.Should().NotBeNull();
            classIdentifier!.Identifier.ValueText.Should().Be("A");
            methodIdentifier!.Identifier.ValueText.Should().Be("RunThatMethod");
        }

        private static SyntaxList<StatementSyntax>? GetMainMethodStatements(string source)
        {
            CSharpCompilation cliRootCompilation = SourceGeneratorUtilities.GetCliRootCompilation(source)
                                                   ?? throw new InvalidOperationException();
            var tree = cliRootCompilation.SyntaxTrees.First().GetRoot();
            var methodBlock = tree.DescendantNodes()
                                 .OfType<MethodDeclarationSyntax>()
                                 .Where(x => x.Identifier.ValueText == "Main")
                                 .First()
                                 .Body
                                 ?.Statements;
            return methodBlock;
        }


        private static IEnumerable<ClassDeclarationSyntax> GetClassDeclarations(string source)
        {
            CSharpCompilation cliRootCompilation = SourceGeneratorUtilities.GetCliRootCompilation(source)
                                                   ?? throw new InvalidOperationException();
            var tree = cliRootCompilation.SyntaxTrees.First().GetRoot();
            return tree.DescendantNodes()
                                 .OfType<ClassDeclarationSyntax>();
        }
    }
}
