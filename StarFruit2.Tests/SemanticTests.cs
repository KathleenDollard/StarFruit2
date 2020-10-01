using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StarFruit2.Tests
{
    public class SemanticTests
    {
        [Fact]
        public void Can_find_semantic_model()
        {
            var tree = CSharpSyntaxTree.ParseText(@"using System;
using System.Collections.Generic;
using System.Text;
 
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}");

            var compilation = CSharpCompilation.Create("Empty")
                        .AddSyntaxTrees(tree);

        }
    }
}
