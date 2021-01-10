using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

[assembly: UseReporter(typeof(VisualStudioReporter))]

namespace RoslynSourceGenSupport.Tests
{
    public class CSharpExtensionTests
    {
        public CSharpExtensionTests()
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
"

;
        }
 
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CommandSource_generator_produces_correct_code(bool useVB)
        {
           
        }

     }
}
