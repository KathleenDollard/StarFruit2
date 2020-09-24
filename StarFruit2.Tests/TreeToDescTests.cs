//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Starfruit2_B;
//using System.Linq;
//using Xunit;

//namespace StarFruit2.Tests
//{
//    public class TreeToDescTests
//    {

//        [Theory]
//        [InlineData("Empty")]
//        [InlineData("SingleSimpleArgument")]
//        [InlineData("SingleOptionWithArgument")]
//        [InlineData("SeveralOptionsAndArguments")]
//        public void Parse_class(string id)
//        {
//            var sample = Sample.Data[id];
//            var tree = CSharpSyntaxTree.ParseText(sample.Code);
//            var maker = new SyntaxTreeDescriptorMaker();

//            var command = maker.GetCommandDescriptor(tree.GetRoot().DescendantNodes()
//                               .OfType<ClassDeclarationSyntax>()
//                               .Single());

//            command.Should().Match(sample);
//        }

//        [Theory]
//        [InlineData("Empty")]
//        [InlineData("SingleSimpleArgument")]
//        [InlineData("SingleOptionWithArgument")]
//        [InlineData("SeveralOptionsAndArguments")]
//        public void Parse_single_class_StarFruit2(string id)
//        {
//            var sample = Sample.Data[id];
//            var tree = CSharpSyntaxTree.ParseText(sample.Code);

//            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
//            var compilation = CSharpCompilation.Create("MyCompilation",
//                syntaxTrees: new[] { tree }, references: new[] { mscorlib });
//            var command = RoslyDescriptorMakerFactory.CreateCommandDescriptor(tree.GetRoot(), compilation);

//            command.Should().Match(sample);
//        }

//    }
//}
