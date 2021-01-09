using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using StarFruit2.Generate;
using System.Collections.Generic;
using CSharp = Microsoft.CodeAnalysis.CSharp.Syntax;
using VB = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Utils = StarFruit.FluentDomSourceGen.Tests.SourceGeneratorUtilities;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class SyntaxReceiverTests
    {
        private const string cSharpSource = @"
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

        private const string vbSource = @"
Imports StarFruit2
Imports StarFruit2.Common

Namespace Tests
    Public NotInheritable Class Program
        Public Shared Function Main(args As String()) As Integer
            CommandSource.Run(Of CliRoot)(args)
            CommandSource.Create(NameOf(RunThisMethod))
            CommandSource.Create(NameOf(A.RunThatMethod))
            Return 0
        End Function
        Public Shared Sub RunThisMethod()
        End Sub
    End Class

    Public NotInheritable Class A
        Public Shared Sub RunThatMethod()
        End Sub
    End Class

    Public Class CliRoot
        Implements ICliRoot
    End Class
End Namespace";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void SyntaxReceiver_finds_root_class_via_interface(bool useVB)
        {
            var (source, expectedType) = useVB
                     ? (vbSource, typeof(VB.ClassBlockSyntax))
                     : (cSharpSource, typeof(CSharp.ClassDeclarationSyntax));
            var receiver = Utils.Receiver(useVB);
            var classDeclarations = Utils.ClassDeclarations(useVB, source);

            receiver = VisitSyntaxNodes(receiver, classDeclarations);

            receiver.Candidates.Should().HaveCount(1);
            var poco = receiver.Candidates.First();
            poco.Should().BeOfType(expectedType);
            GetName(useVB, poco).Should().Be("CliRoot");

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void SyntaxReceiver_finds_root_class_in_generic(bool useVB)
        {
            var (source, expectedType) = useVB
                     ? (vbSource, typeof(VB.IdentifierNameSyntax))
                     : (cSharpSource, typeof(CSharp.IdentifierNameSyntax));
            var receiver = Utils.Receiver(useVB);
            var statementList = Utils.StatementsForMethod(useVB,source, "Main");
            statementList.Should().NotBeNull();
            var statements = statementList.ToArray();

            receiver.OnVisitSyntaxNode(statements[0].ChildNodes().First());

            receiver.Candidates.Should().HaveCount(1);
            var poco = receiver.Candidates.First();
            poco.Should().BeOfType(expectedType);
            GetName(useVB, poco).Should().Be("CliRoot");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void SyntaxReceiver_finds_entry_method_in_class(bool useVB)
        {
            var (source, expectedType) = useVB
                     ? (vbSource, typeof(VB.IdentifierNameSyntax))
                     : (cSharpSource, typeof(CSharp.IdentifierNameSyntax));
            var receiver = Utils.Receiver(useVB);
            var statementList = Utils.StatementsForMethod(useVB, source, "Main");
            statementList.Should().NotBeNull();
            var statements = statementList.ToArray();

            receiver.OnVisitSyntaxNode(statements[1].ChildNodes().First());

            receiver.Candidates.Should().HaveCount(1);
            var poco = receiver.Candidates.First();
            poco.Should().BeOfType(expectedType);
            GetName(useVB, poco).Should().Be("RunThisMethod");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void SyntaxReceiver_finds_entry_method_in_another_class(bool useVB)
        {
            var (source, expectedType) = useVB
                     ? (vbSource, typeof(VB.ClassBlockSyntax))
                     : (cSharpSource, typeof(CSharp.ClassDeclarationSyntax));
            var receiver = Utils.Receiver(useVB);
            var statementList = Utils.StatementsForMethod(useVB, source, "Main");
            statementList.Should().NotBeNull();
            var statements = statementList.ToArray();

            receiver.OnVisitSyntaxNode(statements[2].ChildNodes().First());

            receiver.Candidates.Should().HaveCount(1);
            if (useVB)
            {
                var methodType = receiver.Candidates.First() as VB.MemberAccessExpressionSyntax;
                methodType.Should().NotBeNull();
                var classIdentifier = methodType!.Expression as VB.IdentifierNameSyntax;
                var methodIdentifier = methodType.Name as VB.IdentifierNameSyntax;
                classIdentifier.Should().NotBeNull();
                methodIdentifier.Should().NotBeNull();
                classIdentifier!.Identifier.ValueText.Should().Be("A");
                methodIdentifier!.Identifier.ValueText.Should().Be("RunThatMethod");
            }
            else
            {
                var syntax = receiver.Candidates.First();
                var methodType = syntax as CSharp.MemberAccessExpressionSyntax;
                methodType.Should().NotBeNull();
                var classIdentifier = methodType!.Expression as CSharp.IdentifierNameSyntax;
                var methodIdentifier = methodType.Name as CSharp.IdentifierNameSyntax;
                classIdentifier.Should().NotBeNull();
                methodIdentifier.Should().NotBeNull();
                classIdentifier!.Identifier.ValueText.Should().Be("A");
                methodIdentifier!.Identifier.ValueText.Should().Be("RunThatMethod");
            }
        }


        private static SyntaxReceiverBase VisitSyntaxNodes(SyntaxReceiverBase receiver, IEnumerable<SyntaxNode> classDeclarations)
        {
            foreach (var classDeclaration in classDeclarations)
            {
                receiver.OnVisitSyntaxNode(classDeclaration);
            }
            return receiver;
        }

        private string GetName(bool useVB, SyntaxNode pocoAsSyntaxNode)
            => (useVB, pocoAsSyntaxNode) switch
            {
                (true, VB.ClassBlockSyntax s) => s.ClassStatement.Identifier.ValueText,
                (true, VB.IdentifierNameSyntax s) => s.Identifier.ValueText,
                (false, CSharp.ClassDeclarationSyntax s) => s.Identifier.ValueText,
                (false, CSharp.IdentifierNameSyntax s) => s.Identifier.ValueText,
                _ => throw new NotImplementedException(),
            };



    }
}
