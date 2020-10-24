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
            var classBody = new List<string> { "this is inside the namespace;", "var fizz = 42;" };
            var expectedData = new List<string> { $"namespace {name}", "{"};
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
            var expectedData = new List<string> { $"public partial class {name}", "{"};
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
    }
}
