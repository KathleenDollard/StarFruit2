using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Xunit;

namespace RoslynSourceGenSupport.Tests
{
    public class CSharpTestUtilitiesTests
    {
        private SyntaxTree cSharpSyntaxTree;
        public CSharpTestUtilitiesTests()
        {
            var cSharpCode = @$"
// Aliases are not currently supported
using A;
using A.B;

public class X
{{
   public string Y {{ get ; }}

   public int Z(int a1)
   {{
       var c1 = a1;
       var d1 = b1();
       void b1()
          => a1;
   }}
}}
";
            cSharpSyntaxTree = CSharpSyntaxTree.ParseText(cSharpCode);
        }

        [Theory]
        [InlineData(false)]
        public void Can_find_tokens_by_name(bool useVB)
        {
            var syntaxTree = cSharpSyntaxTree;
            var utilities = new CSharpTestUtilities();

            var identifiers = utilities.TokensWithName(syntaxTree, "a1");

            identifiers.Should().HaveCount(3);
        }
    }
}
