using FluentAssertions;
using Microsoft.CodeAnalysis;
//using CSharpSyntax = Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace RoslynSourceGenSupport.Tests
{
    public class RoslynTestUtilitiesTests
    {
        private readonly Dictionary<(bool UseVB, string CodeName), SyntaxTree> syntaxTrees;

        private SyntaxNode PickSyntaxTreeRoot(bool useVB, string codeName)
            => PickSyntaxTree(useVB, codeName).GetRoot();

        private SyntaxTree PickSyntaxTree(bool useVB, string codeName)
            => syntaxTrees[(useVB, codeName)].CheckedTree();

        private readonly string cSharpSimpleCode = @$"
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

        private readonly string cSharpComplexCode = @$"
// Aliases are not currently supported
using A;
using A.B;

public class X : IMine, IMineToo
{{
   public string Y {{ get ; }}

   public int Z(int a1, string a2)
   {{
       var c1 = a1;
       var d1 = b1();
       void b1()
          => a1;
   }}
   public void ZZ(int a1) 
   {{}}

   public class XX 
   {{
       public void ZZ(int a1)
       {{}}
   }}
}}

public class Y : IMineToo
{{ 
    public X F_Y()
    {{
        var x1 = new X();
        var a = x1.Z(42, string.Empty);
        var b = x1.ZZ(43);
        var c = F_Y();
}}
}}

public interface IMine {{ }}

public interface IMineToo {{ }}";

        private readonly string cSharpManyClassesCode = @$"
namespace ns1
{{
    public class X
    {{
       public string a {{ get ; }}
    }}
}}

namespace ns2
{{
    public class X
    {{ }}

    public class Y
    {{
        public class X {{ }}
    }}
}}
";

        private readonly string cSharpGenericCode = @$"
public class X
{{
   public string Y<T>() {{}}
   public string Y2<T, T2>() {{}}
   public string Y0() {{}}

   public int Z(int a1)
   {{
        var x = Y<int>();
        var x2 = Y2<int, string>();
        var x0 = Y0();
   }}
}}
";

        public RoslynTestUtilitiesTests()
        {
            var cSharpUtililties = RoslynUtilitiesBase.CSharpRoslynUtilities;
            syntaxTrees = new Dictionary<(bool UseVB, string CodeName), SyntaxTree>()
            {
                [(false, "Simple")] = cSharpUtililties.ParseToSyntaxTree(cSharpSimpleCode),
                [(false, "Complex")] = cSharpUtililties.ParseToSyntaxTree(cSharpComplexCode),
                [(false, "ManyClasses")] = cSharpUtililties.ParseToSyntaxTree(cSharpManyClassesCode),
                [(false, "Generic")] = cSharpUtililties.ParseToSyntaxTree(cSharpGenericCode),
            };
        }

        [Theory]
        [InlineData(false)]
        public void Can_find_tokens_by_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Simple");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var identifiers = utilities.TokensWithName(syntaxRoot, "a1");

            identifiers.Should().HaveCount(3);
        }


        [Theory]
        [InlineData(false)]
        public void ClassesWithName_can_find_single_class_by_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Simple");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithName(syntaxRoot, "X");

            classDeclarations.Should().HaveCount(1);
            classDeclarations.Should().AllBeClasses();
        }
        [Theory]
        [InlineData(false)]
        public void ClassesWithName_can_find_multiple_and_nested_classes_by_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "ManyClasses");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithName(syntaxRoot, "X");

            classDeclarations.Should().HaveCount(3);
            classDeclarations.Should().AllBeClasses();
        }
        [Theory]
        [InlineData(false)]
        public void ClassesWithName_returns_empty_list_when_class_not_found(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Simple");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithName(syntaxRoot, "ABC");

            classDeclarations.Should().HaveCount(0);
        }
        [Theory]
        [InlineData(false)]
        public void ClassesWithName_returns_empty_list_when_passed_null(bool useVB)
        {
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithName(null, "x");

            classDeclarations.Should().HaveCount(0);
        }


        [Theory]
        [InlineData(false)]
        public void MethodsWithName_can_find_single_method_by_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var methodDeclarations = utilities.MethodsWithName(syntaxRoot, "Z");

            methodDeclarations.Should().HaveCount(1);
            methodDeclarations.Should().AllBeMethods();
        }
        [Theory]
        [InlineData(false)]
        public void MethodsWithName_can_find_methods_on_nested_classes_by_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var methodDeclarations = utilities.MethodsWithName(syntaxRoot, "ZZ");

            methodDeclarations.Should().HaveCount(2);
            methodDeclarations.Should().AllBeMethods();
        }
        [Theory]
        [InlineData(false)]
        public void MethodsWithName_returns_empty_list_when_method_not_found(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var methodDeclarations = utilities.MethodsWithName(syntaxRoot, "ABC");

            methodDeclarations.Should().HaveCount(0);
        }
        [Theory]
        [InlineData(false)]
        public void MethodsWithName_returns_empty_list_when_passed_null(bool useVB)
        {
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var methodDeclarations = utilities.MethodsWithName(null, "gfd");

            methodDeclarations.Should().HaveCount(0);
        }


        [Theory]
        [InlineData(false)]
        public void ClassesWithBaseOrInterfaceNamed_can_find_single_class_with_interface(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithBaseOrInterfaceNamed(syntaxRoot, "IMine");

            classDeclarations.Should().HaveCount(1);
            classDeclarations.Should().AllBeClasses();
        }
        [Theory]
        [InlineData(false)]
        public void ClassesWithBaseOrInterfaceNamed_can_find_multiple_classes_with_interface(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithBaseOrInterfaceNamed(syntaxRoot, "IMineToo");

            classDeclarations.Should().HaveCount(2);
            classDeclarations.Should().AllBeClasses();
        }
        [Theory]
        [InlineData(false)]
        public void ClassesWithBaseOrInterfaceNamed_returns_empty_list_when_none_found(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithBaseOrInterfaceNamed(syntaxRoot, "IMineXXX");

            classDeclarations.Should().HaveCount(0);
        }
        [Theory]
        [InlineData(false)]
        public void ClassesWithBaseOrInterfaceNamed_returns_emplty_list_when_null_passed_null(bool useVB)
        {
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classDeclarations = utilities.ClassesWithBaseOrInterfaceNamed(null, "");

            classDeclarations.Should().HaveCount(0);
        }


        [Theory]
        [InlineData(false)]
        public void ClassName_can_find_class_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var syntaxNodes = utilities.ClassesWithName(syntaxRoot, "X");
            syntaxNodes.Should().HaveCount(1);
            var typeName = utilities.TypeName(syntaxNodes.First());

            typeName.Should().Be("X");
        }
        [Theory]
        [InlineData(false)]
        public void ClassName_empty_when_syntaxNode_isnt_a_type(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var syntaxNodes = utilities.ClassesWithName(syntaxRoot, "IMine");
            syntaxNodes.Should().HaveCount(0);
            var typeName = utilities.TypeName(syntaxNodes.FirstOrDefault());

            typeName.Should().BeNull();
        }
        [Theory]
        [InlineData(false)]
        public void ClassName_empty_when_syntaxNode_is_null(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var syntaxNodes = utilities.ClassesWithName(syntaxRoot, "asdf");
            syntaxNodes.Should().HaveCount(0);
            var typeName = utilities.TypeName(syntaxNodes.FirstOrDefault());
            typeName.Should().BeNull();
        }
        [Theory]
        [InlineData(false)]
        public void ClassName_can_find_base_type_name(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var methodNode = utilities.MethodsWithName(syntaxRoot, "F_Y").FirstOrDefault();
            methodNode.Should().NotBeNull();
            var returnType = utilities.GetReturnType(methodNode);
            returnType!.Should().NotBeNull();
            var typeName = utilities.TypeName(returnType!);
            typeName.Should().Be("X");
        }


        [Theory]
        [InlineData(false)]
        public void GetUsingNames_found_when_present_on_compilation(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var names = utilities.GetUsingNames(syntaxRoot);

            names.Should().HaveCount(2);
            names.First().Should().Be("A");
            names.Last().Should().Be("A.B");
        }
        [Theory]
        [InlineData(false)]
        public void GetUsingNames_found_when_present_on_ancestor(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var classNode = utilities.ClassesWithName(syntaxRoot, "X").FirstOrDefault();
            classNode.Should().NotBeNull();
            var names = utilities.GetUsingNames(classNode);

            names.Should().HaveCount(2);
            names.First().Should().Be("A");
            names.Last().Should().Be("A.B");
        }
        [Theory]
        [InlineData(false)]
        public void GetUsingNames_empty_when_no_usings(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Simple");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var syntaxNodes = utilities.GetUsingNames(syntaxRoot);

            syntaxNodes.Should().HaveCount(0);
        }
        [Theory]
        [InlineData(false)]
        public void GetUsingNames_empty_when_passed_null(bool useVB)
        {
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var syntaxNodes = utilities.GetUsingNames(null);

            syntaxNodes.Should().HaveCount(0);
        }

        [Theory]
        [InlineData(false)]
        public void GetGenericArguments_finds_type_arg_on_invocation(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Generic");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var invocationSyntax = InvocationWithName(syntaxRoot, "Y", utilities);
            invocationSyntax!.Should().NotBeNull();

            var genericNames = utilities.GetGenericArguments(invocationSyntax);

            genericNames.Should().HaveCount(1);
            genericNames.Should().AllBeTypeUsage();
        }
        [Theory]
        [InlineData(false)]
        public void GetGenericArguments_finds_multiple_type_arg_on_invocation(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Generic");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var invocationSyntax = InvocationWithName(syntaxRoot, "Y2", utilities);
            invocationSyntax!.Should().NotBeNull();

            var genericNames = utilities.GetGenericArguments(invocationSyntax);

            genericNames.Should().HaveCount(2);
            genericNames.Should().AllBeTypeUsage();
        }
        [Theory]
        [InlineData(false)]
        public void GetGenericArguments_empty_when_no_type_args(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Generic");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var invocationSyntax = InvocationWithName(syntaxRoot, "Y0", utilities);
            invocationSyntax!.Should().NotBeNull();

            var genericNames = utilities.GetGenericArguments(invocationSyntax);

            genericNames.Should().HaveCount(0);
        }
        [Theory]
        [InlineData(false)]
        public void GetGenericArguments_empty_when_passed_null(bool useVB)
        {
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var genericNames = utilities.GetGenericArguments(null);

            genericNames.Should().HaveCount(0);
        }


        [Theory]
        [InlineData(false)]
        public void GetMethodArguments_finds_single_arg_on_invocation(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var invocationSyntax = InvocationWithName(syntaxRoot, "ZZ", utilities);
            invocationSyntax!.Should().NotBeNull();

            var methodArgs = utilities.MethodArguments  (invocationSyntax);

            methodArgs.Should().HaveCount(1);
        }
        [Theory]
        [InlineData(false)]
        public void GetMethodArguments_finds_multiple_args_on_invocation(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var invocationSyntax = InvocationWithName(syntaxRoot, "Z", utilities);
            invocationSyntax!.Should().NotBeNull();

            var methodArgs = utilities.MethodArguments(invocationSyntax);

            methodArgs.Should().HaveCount(2);
        }
        [Theory]
        [InlineData(false)]
        public void GetMethodArguments_empty_when_no_type_args(bool useVB)
        {
            var syntaxRoot = PickSyntaxTreeRoot(useVB, "Complex");
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var invocationSyntax = InvocationWithName(syntaxRoot, "F_Y", utilities);
            invocationSyntax!.Should().NotBeNull();

            var methodArgs = utilities.MethodArguments(invocationSyntax);

            methodArgs.Should().HaveCount(0);
        }
        [Theory]
        [InlineData(false)]
        public void GetMethodArguments_empty_when_passed_null(bool useVB)
        {
            var utilities = RoslynUtilitiesBase.Pick(useVB);

            var methodArgs = utilities.MethodArguments(null);

            methodArgs.Should().HaveCount(0);
        }

        private SyntaxNode? InvocationWithName(SyntaxNode syntaxRoot, string name, RoslynUtilitiesBase utilities)
            => utilities.TokensWithName(syntaxRoot, name)
                        .Select(x => x.Parent?.Ancestors()
                                              .Where(x => utilities.IsMethodInvocation(x))
                                              .FirstOrDefault())
                        .FirstOrDefault(x => x is not null);
    }
}
