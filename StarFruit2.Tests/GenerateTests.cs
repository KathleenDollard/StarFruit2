using System.Collections.Generic;
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

            var result = generate.Class(scope, isPartial, name, null, classBody);

            result.Should().BeEquivalentTo(expectedData);
        }

        // ADD COVERAGE FOR INHERITANCE
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

            var result = Utils.Normalize(generate.Class(scope, isPartial, name, null, classBody));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Property_ReturnsPropertyDeclarationWithPublicSetter()
        {
            var scope = Scope.Public;
            var type = "Option<int>";
            var name = "PropName";
            var setterScope = Scope.Public;
            var expectedData = $"public Option<int> {name} {{ get; set; }}";

            var result = generate.Property(scope, type, name, setterScope);

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Property_ReturnsPropertyDeclarationWithPrivateSetter()
        {
            var scope = Scope.Internal;
            var type = "Argument<string>";
            var name = "PropName";
            var setterScope = Scope.Private;
            var expectedData = $"internal Argument<string> {name} {{ get; private set; }}";

            var result = generate.Property(scope, type, name, setterScope);

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Constructor_ReturnsCtorWithNoArgsAndNoBaseArgs()
        {
            var className = "ClassName";
            var ctorBody = new List<string> { "Something = true;" };
            var expectedData = new List<string>
            {
                $"public {className}()",
                ": base()",
                "{"
            };
            expectedData.AddRange(ctorBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Constructor(className, null, null, ctorBody));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Constructor_ReturnsCtorWithArgsAndNoBaseArgs()
        {
            var className = "ClassName";
            var ctorBody = new List<string> { "Something = true;" };
            var ctorArgs = new List<string> { "42", "true" };
            var expectedData = new List<string>
            {
                $"public {className}(42, true)",
                ": base()",
                "{"
            };
            expectedData.AddRange(ctorBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Constructor(className, ctorArgs, null, ctorBody));

            result.Should().BeEquivalentTo(expectedData);
        }


        [Fact]
        public void Constructor_ReturnsCtorWithNoArgsAndBaseArgs()
        {
            var className = "ClassName";
            var ctorBody = new List<string> { "Something = true;" };
            var baseArgs = new List<string> { "42", "true" };
            var expectedData = new List<string>
            {
                $"public {className}()",
                ": base(42, true)",
                "{"
            };
            expectedData.AddRange(ctorBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Constructor(className, null, baseArgs, ctorBody));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Constructor_ReturnsCtorWithArgsAndBaseArgs()
        {
            var className = "ClassName";
            var ctorBody = new List<string> { "Something = true;" };
            var ctorArgs = new List<string> { "42", "true" };
            var baseArgs = new List<string> { "\"fizz\"", "false" };
            var expectedData = new List<string>
            {
                $"public {className}(42, true)",
                ": base(\"fizz\", false)",
                "{"
            };
            expectedData.AddRange(ctorBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Constructor(className, ctorArgs, baseArgs, ctorBody));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Constructor_ReturnsCtorWithNonDefaultScope()
        {
            var className = "ClassName";
            var scope = Scope.Private;
            var ctorBody = new List<string> { "Something = true;" };
            var expectedData = new List<string>
            {
                $"private {className}()",
                ": base()",
                "{"
            };
            expectedData.AddRange(ctorBody);
            expectedData.Add("}");

            var result = Utils.Normalize(generate.Constructor(className, null, null, ctorBody, scope));

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

            var result = generate.Parameter(paramType, paramName);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void MakeGenericType_WithSingleTypeReturnsType()
        {
            var type = "string";

            var result = generate.GenericType(type);

            result.Should().Be(type);
        }

        [Fact]
        public void MakeGenericType_WithTypeAndSingleGenericArg()
        {
            var baseType = "List";
            var generic = "string";
            var expectedData = "List<string>";

            var result = generate.GenericType(baseType, generic);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void MakeGenericType_WithTypeAndMultipleGenericArgs()
        {
            var baseType = "Dictionary";
            var firstGeneric = "string";
            var secondGeneric = "Object";
            var expectedData = "Dictionary<string, Object>";

            var result = generate.GenericType(baseType, firstGeneric, secondGeneric);

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

            var result = Utils.Normalize(generate.Method(Scope.Public, methodName, methodBody, returnType, false, false, argument));

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

            var result = Utils.Normalize(generate.Method(Scope.Public, methodName, methodBody, returnType, false, false, argument, otherArgument));

            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void Assign_WithNoOpGivenReturnsAssignment()
        {
            var leftHand = "string fizz";
            var rightHand = @"""fizz""";
            var expectedData = $"{leftHand} = {rightHand}";

            var result = generate.Assign(leftHand, rightHand);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Assign_WithOpGivenReturnsOperation()
        {
            var leftHand = "string fizz";
            var rightHand = @"""fizz""";
            var op = "+=";
            var expectedData = $"{leftHand} += {rightHand}";

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
            var returnValue = new List<string> { "SomeMethod(arg1,", "arg2)" };
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

        [Fact]
        public void ObjectInit_ReturnsCorrectObjectInitialization()
        {
            var objName = "Object";
            var ctorParams = new List<string> { "42", "\"fizz\"" };
            var properties = new List<string> { "SomeProp = true", "OtherProp = 42" };
            var expectedData = new List<string>
            {
                $"new {objName}({string.Join(", ", ctorParams)})",
                "{"
            };
            expectedData.AddRange(properties.Select(prop => $"{prop},"));
            expectedData.Add("};");

            var result = generate.NewObjectWithInit(objName, ctorParams, properties);

            result.Should().BeEquivalentTo(expectedData);

        }

        [Fact]
        public void Lambda_ReturnsLambdaWithZeroArgsAndOneStatement()
        {
            var statement = new List<string> { "true" };
            var expectedData = "() => true;";

            var result = generate.Lambda(null, statement);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Lambda_ReturnsLambdaWithOneArgAndOneStatement()
        {
            var args = new List<string> { "opt" };
            var statement = new List<string> { "true" };
            var expectedData = "(opt) => true;";

            var result = generate.Lambda(args, statement);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Lambda_ReturnsLambdaWithOneArgAndMultipleStatements()
        {
            var args = new List<string> { "opt" };
            var statement = new List<string> { "true", "return 1" };
            var expectedData = "(opt) => { true; return 1; }";

            var result = generate.Lambda(args, statement);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void Lambda_ReturnsLambdaWithMultipleArgsAndMultipleStatements()
        {
            var args = new List<string> { "opt", "arg" };
            var statement = new List<string> { "true", "return 1" };
            var expectedData = "(opt, arg) => { true; return 1; }";

            var result = generate.Lambda(args, statement);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void FormattedArgs_ReturnsEmptyStringWhenArgsAreNull()
        {
            var expectedData = "";
            var result = generate.FormattedArgs(null);

            result.Should().Be(expectedData);
        }

        [Fact]
        public void FormattedArgs_ReturnsCommaDelimitedArgsWhenArgsArePresent()
        {
            var args = new List<string> { "42", "true" };
            var expectedData = "42, true";
            var result = generate.FormattedArgs(args);

            result.Should().Be(expectedData);
        }
    }
}
