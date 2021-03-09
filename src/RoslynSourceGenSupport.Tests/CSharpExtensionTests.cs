using FluentAssertions;
using Microsoft.CodeAnalysis;
using MsCSharp = Microsoft.CodeAnalysis.CSharp;
using CSharpSyntax = Microsoft.CodeAnalysis.CSharp.Syntax;
using VBSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using RoslynSourceGenSupport.CSharp;
using RoslynSourceGenSupport.VisualBasic;
using System.Collections.Generic;
using System.Linq;
using Xunit;


// We can't easily use the Theory approach for extension methods. Thus
// the VB/C# tests are repeated.
namespace RoslynSourceGenSupport.Tests
{
    public class CSharpExtensionTests
    {
        public RoslynUtilitiesBase TestUtilities(bool useVB)
           => useVB
                 ? new CSharpRoslynUtilities()
                 : new VBRoslynUtilities();

        public IEnumerable<SyntaxToken> FindIdentifierTokens(bool useVB, SyntaxTree syntaxTree, string name)
            => TestUtilities(useVB).TokensWithName(SyntaxTreeForTest(useVB).GetRoot(), name);

        public SyntaxNode? FindClassDeclarationWithIdentifier(bool useVB, SyntaxTree syntaxTree, string name)
            => FindIdentifierTokens(useVB, syntaxTree, name)
                    .Select(x => x.Parent)
                    .Where(x => IsClassDeclaration(useVB, x))
                    .FirstOrDefault();

        public SyntaxNode? FindMethodDeclarationWithIdentifier(bool useVB, SyntaxTree syntaxTree, string name)
        {
            var nodes = FindIdentifierTokens(useVB, syntaxTree, name).Select(x => x.Parent).ToList();
            var methods = nodes.Where(x => IsMethodDeclaration(useVB, x)).ToList();
            return methods.FirstOrDefault();
        }

        public bool IsClassDeclaration(bool useVB, SyntaxNode? syntaxNode)
            => syntaxNode is null
                ? false
                : useVB
                    ? syntaxNode is VBSyntax.ClassStatementSyntax
                    : syntaxNode is CSharpSyntax.ClassDeclarationSyntax;

        public bool IsMethodDeclaration(bool useVB, SyntaxNode? syntaxNode)
            => syntaxNode is null
                ? false
                : useVB
                    ? syntaxNode is VBSyntax.MethodStatementSyntax
                    : syntaxNode is CSharpSyntax.MethodDeclarationSyntax;

        private readonly SyntaxTree cSharpSyntaxTree;
        private readonly SyntaxTree vbSyntaxTree;

        public CSharpExtensionTests()
        {
            var cSharpCode = @$"
// Aliases are not currently supported
using A;
using A.B;

public class X
{{
   public string Y {{ get ; }}

   public static int Z(int a1)
   {{
       var c1 = a1;
       var d1 = b1();
       void b1()
          => a1;
   }}

   public int Z1(int a1)
   {{   }}
}}

public class XX
{{
    public void WW()
    {{
        var d = new X();
        var x = X.Z(42);
    }}

   public string YY {{ get; }}

   public int ZZ(int a1)
   {{
   }}
}}

public class Prog
{{
   public void Run()
   {{
      CommandSource.Run<XX>(null);
   }}

}}

";
            cSharpSyntaxTree = MsCSharp.CSharpSyntaxTree.ParseText(cSharpCode);
        }

        private SyntaxTree SyntaxTreeForTest(bool useVB)
            => useVB
                ? vbSyntaxTree
                : cSharpSyntaxTree;

        private IEnumerable<string> GetUsingsForTest(bool useVB, SyntaxToken syntaxToken)
            => useVB
                ? VBSourceGenExtensions.GetUsings(syntaxToken.Parent).Select(x => x.Name.ToString())
                : CSharpSourceGenExtensions.GetUsings(syntaxToken.Parent).Select(x => x.Name.ToString());

        private SyntaxNode? GetCallToMethodOnClassForTest(bool useVB, SyntaxNode syntaxNode, string className)
            => useVB
                ? VBSourceGenExtensions.IfCallToMethodOnClass(syntaxNode, className)
                : CSharpSourceGenExtensions.IfCallToStaticMethodOnClass(syntaxNode, className);


        [Theory]
        //[InlineData(true)]
        [InlineData(false)]
        public void Can_find_usings(bool useVB)
        {
            var usingList = new string[] { "A", "A.B" };
            var identifiers = FindIdentifierTokens(useVB, SyntaxTreeForTest(useVB), "a1");
 
            var usingsLists = identifiers.Select(x => GetUsingsForTest(useVB, x)).ToArray();

            usingsLists[0].Should().BeEquivalentTo(usingList);
            usingsLists[1].Should().BeEquivalentTo(usingList);
            usingsLists[2].Should().BeEquivalentTo(usingList);
        }

        [Theory]
        //[InlineData(true)]
        [InlineData(false)]
        public void IfCallToMethodOnClass_finds_method(bool useVB)
        {
            var syntaxTree = SyntaxTreeForTest(useVB);
            SyntaxNode? methodDeclarationNode = FindMethodDeclarationWithIdentifier(useVB, syntaxTree, "Z");
            methodDeclarationNode.Should().NotBeNull();
            SyntaxNode? otherClass = FindClassDeclarationWithIdentifier(useVB, syntaxTree, "XX");
            otherClass.Should().NotBeNull();
            SyntaxNode? methodInvocation = otherClass.DescendantNodes()
                                                     .OfType<CSharpSyntax.InvocationExpressionSyntax>()
                                                     .Where(x => x.Expression.ToString() == "X.Z")
                                                     .FirstOrDefault();
            methodInvocation.Should().NotBeNull();

            var foundNode = GetCallToMethodOnClassForTest(useVB, methodInvocation!, "X");

            foundNode.Should().NotBeNull();
        }

        [Theory]
        //[InlineData(true)]
        [InlineData(false)]
        public void CllToMethodOnClass_null_when_method_only_found_on_different_class(bool useVB)
        {
            SyntaxNode? syntaxNode = FindMethodDeclarationWithIdentifier(useVB, SyntaxTreeForTest(useVB), "ZZ");
            syntaxNode.Should().NotBeNull();

            var foundNode = GetCallToMethodOnClassForTest(useVB, syntaxNode!, "X");

            foundNode.Should().BeNull();
        }

        [Fact]
        public void CllToMethod_finds_method()
        { }

        [Fact]
        public void CllToMethod_null_when_method_not_found()
        { }

        [Fact]
        public void GenericArgumentsFromName_returns_single_type()
        { }

        [Fact]
        public void GenericArgumentsFromName_returns_multiple_types()
        { }

        [Fact]
        public void GenericArgumentsFromName_returns_nested_type()
        { }

        [Fact]
        public void GenericArgumentsFromName_empty_list_when_no_generic_args()
        { }

        [Fact]
        public void GenericArgumentsFromName_empty_list_when_null_or_wrong_type_passed()
        { }

        [Fact]
        public void ClassWithBaseOrInterface_returns_type()
        { }

        [Fact]
        public void ClassWithBaseOrInterface_null_when_none_found()
        { }

        [Fact]
        public void ArgumentsOnMethod_returns_nested_type()
        { }

        [Fact]
        public void ArgumentsOnMethod_empty_list_when_no_arguments_on_method()
        { }

        [Fact]
        public void ArgumentsOnMethod_empty_list_when_null_or_wrong_type_passed()
        { }
    }
}
