using FluentAssertions;
using FluentDom.Generator;
using Xunit;

namespace FluentDom.Tests
{
    public class MemberTests
    {
        private const string PropertyName = "PropertyName";
        private const string MethodName = "MethodName";

        [Fact]
        public void Generated_auto_property_is_correct()
        {
            var property = new Property(PropertyName, "string");

            var expected = @"public string PropertyName { get; set; }
";
            var actual = new CSharpGenerator().OutputProperty(property).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_readonly_auto_property_is_correct()
        {
            var property = new Property(PropertyName, "string", readOnly: true);

            var expected = @"public string PropertyName { get; }
";
            var actual = new CSharpGenerator().OutputProperty(property).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_expression_body_property_is_correct()
        {
            var property = new Property(PropertyName, "string", readOnly: true)
                                .GetterReturn(Expression.Value(42));

            var expected = @"public string PropertyName
=> 42;

";
            var actual = new CSharpGenerator().OutputProperty(property).GetOutput().NormalizeWhitespace();

            actual.Should().Be(expected.NormalizeWhitespace());
        }

        [Fact]
        public void Generated_full_property_is_correct()
        {
            var property = new Property(PropertyName, "string")
                                .GetterStatements(Expression.AssignVar("y", "int", Expression.Value(11)))
                                .GetterReturn(Expression.Value(5))
                                .SetterStatements(Expression.AssignVar("x", "int", Expression.Value(7)));
            var expected = @"public string PropertyName
{
   get
   {
      int y = 11;
      return 5;
   }
   set
   {
      int x = 7;
   }
}
";
            var actual = new CSharpGenerator().OutputProperty(property).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_full_property_auto_getter_is_correct()
        {
            var property = new Property(PropertyName, "string")
                                .GetterReturn(Expression.Value(5))
                                .SetterStatements(Expression.AssignVar("x", "int", Expression.Value(7)));
            var expected = @"public string PropertyName
{
   get
   {
      return 5;
   }
   set
   {
      int x = 7;
   }
}
";
            var actual = new CSharpGenerator().OutputProperty(property).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_full_property_auto_setter_is_correct()
        {
            var property = new Property(PropertyName, "string")
                                .GetterReturn(Expression.Value(5))
                                .SetterStatements(Expression.AssignVar("x", "int", Expression.Value(7)));
            var expected = @"public string PropertyName
{
   get
   {
      return 5;
   }
   set
   {
      int x = 7;
   }
}
";
            var actual = new CSharpGenerator().OutputProperty(property).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_constructor_is_correct()
        {
            var ctor = new Constructor(new TypeRep("ClassName"));

            var expected = @"public ClassName()
{
}
";
            var actual = new CSharpGenerator().OutputConstructor(ctor).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_constructor_with_base_is_correct()
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .BaseCall();

            var expected = @"public Abc()
: base()
{
}
";
            var actual = new CSharpGenerator().OutputConstructor(ctor).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_constructor_with_this_is_correct()
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .ThisCall();

            var expected = @"public Abc()
: this()
{
}
";
            var actual = new CSharpGenerator().OutputConstructor(ctor).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_constructor_with_base_and_argument_is_correct()
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .BaseCall(Expression.Value(42));

            var expected = @"public Abc()
: base(42)
{
}
";
            var actual = new CSharpGenerator().OutputConstructor(ctor).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_constructor_with_this_and_argument_is_correct()
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .ThisCall(Expression.Value(42));

            var expected = @"public Abc()
: this(42)
{
}
";
            var actual = new CSharpGenerator().OutputConstructor(ctor).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_method_is_correct()
        {
            var method = new Method(MethodName);

            var expected = @"public void MethodName()
{
}
";
            var actual = new CSharpGenerator().OutputMethod(method).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_method_is_correct_with_returnType()
        {
            var method = new Method(MethodName)
                            .ReturnType("string");

            var expected = @"public string MethodName()
{
}
";
            var actual = new CSharpGenerator().OutputMethod(method).GetOutput();

            actual.Should().Be(expected);
        }
 
        [Fact]
        public void Generated_method_with_parameters_is_correct()
        {
            var method = new Method(MethodName)
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));

            var expected = @"public void MethodName(int value, string s)
{
}
";
            var actual = new CSharpGenerator().OutputMethod(method).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_method_with_returnType_and_parameters_is_correct()
        {
            var method = new Method(MethodName)
                            .ReturnType("string")
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));

            var expected = @"public string MethodName(int value, string s)
{
}
";
            var actual = new CSharpGenerator().OutputMethod(method).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_method_with_scope_is_correct()
        {
            var method = new Method(MethodName, scope: Scope.Protected)
                            .ReturnType("string")
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));

            var expected = @"protected string MethodName(int value, string s)
{
}
";
            var actual = new CSharpGenerator().OutputMethod(method).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Generated_method_with_modifiers_is_correct()
        {
            var method = new Method(MethodName, modifiers: MemberModifiers.Partial | MemberModifiers.Override)
                            .ReturnType("string")
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));

            var expected = @"public partial override string MethodName(int value, string s)
{
}
";
            var actual = new CSharpGenerator().OutputMethod(method).GetOutput();

            actual.Should().Be(expected);
        }


        [Fact]
        public void Generated_constructor_with_parameters_is_correct()
        {
            var ctor = new Constructor(new TypeRep("ClassName"))
                         .Parameter("abc", typeof(string))
                         .Parameter("def", typeof(int));

            var expected = @"public ClassName(string abc, int def)
{
}
";
            var actual = new CSharpGenerator().OutputConstructor(ctor).GetOutput();

            actual.Should().Be(expected);
        }

        [Fact(Skip = "TBD")]
        public void Generated_field_is_correct()
        {
        }

        [Fact(Skip = "TBD")]
        public void Generated_field_with_initializer_is_correct()
        {
        }
    }
}
