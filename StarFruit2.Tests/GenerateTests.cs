using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace StarFruit2.Tests
{
    public class GenerateTests
    {
        private readonly Generate generate;
        public GenerateTests()
        {
            generate = new Generate();
        }

        [Fact]
        public void Usings_ReturnsCollectionOfUsingStatements()
        {
            var nspacesToUse = new string[] { "SweetLibrary", "AnotherSweetLibrary" };
            var expectedData = new List<string> { "using SweetLibrary;", "using AnotherSweetLibrary;" };

            var result = generate.Usings(nspacesToUse);

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Namespace_ReturnsNamespaceDeclarationAndBody()
        {
            var name = "ThatsASweetNamespace";
            var classBody = new List<string> { "// this is inside the namespace", "var fizz = 42;" };
            var expectedData = new List<string> { $"namespace {name}", "{" };
            expectedData.AddRange(classBody);
            expectedData.Add("}");

            var result = generate.Namespace(name, classBody);

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Class_ReturnsPartialClassDeclarationAndBody()
        {
            var scope = Scope.Public;
            var isPartial = true;
            var name = "SweetClassName";
            var classBody = new List<string> { "SomeFunction();", "var fizz = 42;" };
            var expectedData = new List<string> { $"public partial class {name}", "{" };
            expectedData.AddRange(classBody);
            expectedData.Add("}");

            var result = generate.Class(scope, isPartial, name, classBody);

            result.Should().BeEquivalentTo(expectedData);
        }
        // look at just testing first line/single line where useful
        [Fact]
        public void Class_ReturnsClassDeclarationAndBody()
        {
            var scope = Scope.Internal;
            var isPartial = false;
            var name = "SweetClassName";
            var classBody = new List<string> { "SomeFunction();", "var fizz = 42;" };
            var expectedData = new List<string> { $"internal class {name}", "{" };
            expectedData.AddRange(classBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Class(scope, isPartial, name, classBody));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Property_ReturnsPropertyDeclarationWithPublicSetter()
        {
            var scope = Scope.Public;
            var type = "Option";
            var genericType = "int";
            var name = "PropName";
            var setterScope = Scope.Public;
            var expectedData = $"public Option<int> {name} {{ get; set; }}";

            var result = generate.Property(scope, type, genericType, name, setterScope);

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Property_ReturnsPropertyDeclarationWithPrivateSetter()
        {
            var scope = Scope.Internal;
            var type = "Argument";
            var genericType = "string";
            var name = "PropName";
            var setterScope = Scope.Private;
            var expectedData = $"internal Argument<string> {name} {{ get; private set; }}";

            var result = generate.Property(scope, type, genericType, name, setterScope);

            result.Should().BeEquivalentTo(expectedData);
        }

        // TODO: could be whitespace concerns with indentation of : base for inheritance
        [Fact]
        public void Constructor_ReturnsConstructorInheritingFromBase()
        {
            var name = "SweetClassName";
            var cliName = "sweet-class-name";
            var ctorBody = new List<string> { "SomeFunction();", "var fizz = 42;" };
            var expectedData = new List<string> { $"public {name}", $": base(new Command({cliName}))", "{" };
            expectedData.AddRange(ctorBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Constructor(name, cliName, ctorBody));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void OptionDeclarationForCtor_ReturnsOptDeclaration()
        {
            var name = "SomeName";
            var cliName = "some-name";
            var optType = "boolean";
            var expectedData = @$"{name} = new Option<{optType}>(""{cliName}"");";

            var result = generate.OptionDeclarationForCtor(name, cliName, optType);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void ArgDeclarationForCtor_ReturnsArgDeclaration()
        {
            var name = "SomeName";
            var cliName = "some-name";
            var optType = "boolean";
            var expectedData = @$"{name} = new Argument<{optType}>(""{cliName}"");";

            var result = generate.ArgDeclarationForCtor(name, cliName, optType);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void AddToCommand_ReturnsMethodWithArgChainedOnCommand()
        {
            var methodName = "AddCommand";
            var argumentName = "GetSomeCommand()";
            var expectedData = $"Command.{methodName}({argumentName});";

            var result = generate.AddToCommand(methodName, argumentName);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void MakeParam_ReturnsStringFormattedParam()
        {
            var paramName = "someParam";
            var paramType = "Object";
            var expectedData = "Object someParam";

            var result = generate.MakeParam(paramType, paramName);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void MakeGenericType_WithSingleTypeReturnsType()
        {
            var type = "string";

            var result = generate.MakeGenericType(type);

            result.Should().Be(type);
        }

        [Fact]
        public void MakeGenericType_WithTypeAndSingleGenericArg()
        {
            var baseType = "List";
            var generic = "string";
            var expectedData = "List<string>";

            var result = generate.MakeGenericType(baseType, generic);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void MakeGenericType_WithTypeAndMultipleGenericArgs()
        {
            var baseType = "Dictionary";
            var firstGeneric = "string";
            var secondGeneric = "Object";
            var expectedData = "Dictionary<string, Object>";

            var result = generate.MakeGenericType(baseType, firstGeneric, secondGeneric);

            result.Should().Be(expectedData);
        }


        [Fact]
        public void Method_ReturnsCorrectMethodWithNoArguments()
        {
            var methodBody = new List<string>
            {
                "var SomeThing = 42;",
                "var SomeOtherThing = true;",
                "return SomeThing;"
            };
            var methodName = "SomeMethod";
            var returnType = "int";
            var expectedData = new List<string>
            {
                $"public {returnType} {methodName}()",
                "{"
            };
            expectedData.AddRange(methodBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Method(Scope.Public, methodName, methodBody, returnType));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Method_ReturnsCorrectAsyncMethodWithNoArguments()
        {
            var methodBody = new List<string>
            {
                "var SomeThing = 42;",
                "var SomeOtherThing = true;",
                "return SomeThing;"
            };
            var methodName = "SomeMethod";
            var returnType = "Task<int>";
            var expectedData = new List<string>
            {
                $"public async {returnType} {methodName}()",
                "{"
            };
            expectedData.AddRange(methodBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Method(Scope.Public, methodName, methodBody, returnType, true));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Method_ReturnsCorrectMethodWithOneArgument()
        {
            var methodBody = new List<string>
            {
                "var SomeThing = 42;",
                "var SomeOtherThing = true;",
                "return SomeThing;"
            };
            var methodName = "SomeMethod";
            var returnType = "Task<int>";
            var argument = "Object someArgument";
            var expectedData = new List<string>
            {
                $"public {returnType} {methodName}({argument})",
                "{"
            };
            expectedData.AddRange(methodBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Method(Scope.Public, methodName, methodBody, returnType, false, argument));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Method_ReturnsCorrectMethodWithMultipleArguments()
        {
            var methodBody = new List<string>
            {
                "var SomeThing = 42;",
                "var SomeOtherThing = true;",
                "return SomeThing;"
            };
            var methodName = "SomeMethod";
            var returnType = "Task<int>";
            var argument = "Object someArgument";
            var otherArgument = "String str";
            var expectedData = new List<string>
            {
                $"public {returnType} {methodName}({argument}, {otherArgument})",
                "{"
            };
            expectedData.AddRange(methodBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Method(Scope.Public, methodName, methodBody, returnType, false, argument, otherArgument));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Assign_WithNoOpGivenReturnsAssignment()
        {
            var leftHand = "string fizz";
            var rightHand = @"""fizz""";
            var expectedData = $"{leftHand} = {rightHand};";

            var result = generate.Assign(leftHand, rightHand);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Assign_WithOpGivenReturnsOperation()
        {
            var leftHand = "string fizz";
            var rightHand = @"""fizz""";
            var op = "+=";
            var expectedData = $"{leftHand} += {rightHand};";

            var result = generate.Assign(leftHand, rightHand, op);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Return_WhenGivenSingleStringAndAwaitReturnsCorrectString()
        {
            var returnValue = "SomeMethod()";
            var expectedData = "return await SomeMethod();";

            var result = generate.Return(returnValue, true);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Return_WhenGivenSingleStringReturnsCorrectString()
        {
            var returnValue = "SomeMethod()";
            var expectedData = "return SomeMethod();";

            var result = Utils.Normalize(generate.Return(returnValue, false));

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Return_WhenGivenMultipleStringListReturnsCorrectStringList()
        {
            var returnValue = new List<string> { "new Object()", "{", "SomeProp = 42", "}" };
            var expectedData = new List<string> { "return new Object()", "{", "SomeProp = 42", "};" };

            var result = Utils.Normalize(generate.Return(returnValue));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Return_WhenGivenTwoStringListReturnsCorrectStringList()
        {
            var returnValue = new List<string> { "SomeMethod(arg1,", "arg2)"};
            var expectedData = new List<string> { "return SomeMethod(arg1,", "arg2);" };

            var result = Utils.Normalize(generate.Return(returnValue));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Return_WhenGivenSingleStringListReturnsCorrectStringList()
        {
            var returnValue = new List<string> { "new Object()" };
            var expectedData = new List<string> { "return new Object();" };

            var result = Utils.Normalize(generate.Return(returnValue));

            result.Should().BeEquivalentTo(expectedData);
        }
    }
}
