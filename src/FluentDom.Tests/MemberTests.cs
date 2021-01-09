using FluentAssertions;
using FluentDom.Generator;
using Xunit;
using System;

namespace FluentDom.Tests
{
    public class MemberTests
    {
        private const string PropertyName = "PropertyName";
        private const string MethodName = "MethodName";

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_auto_property_is_correct(bool useVB)
        {
            var property = new Property(PropertyName, "string");
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @"public string PropertyName { get; set; }",
                VBGenerator => @"Public Property PropertyName As String",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputProperty(property).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_readonly_auto_property_is_correct(bool useVB)
        {
            var property = new Property(PropertyName, "string", readOnly: true);
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");
            var expected = generator switch
            {
                CSharpGenerator => @"public string PropertyName { get; }",
                VBGenerator => @"Public ReadOnly Property PropertyName As String",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputProperty(property).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(false)]
        public void Generated_expression_body_property_is_correct(bool useVB)
        {
            var property = new Property(PropertyName, "string", readOnly: true)
                                .GetterReturn(Expression.Value(42));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public string PropertyName
                                        => 42;",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputProperty(property).GetOutput().NormalizeWhitespace();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_full_property_is_correct(bool useVB)
        {
            var property = new Property(PropertyName, "string")
                                .GetterStatements(Expression.AssignVar("y", "int", Expression.Value(11)))
                                .GetterReturn(Expression.Value(5))
                                .SetterStatements(Expression.AssignVar("x", "int", Expression.Value(7)));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public string PropertyName
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
                                     }",
                VBGenerator => $@"Public Property PropertyName As String
                                      Get
                                          Dim y As Integer = 11
                                          Return 5
                                      End Get
                                  
                                      Set(Value As String)
                                          Dim x As Integer = 7
                                      End Set
                                  End Property",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputProperty(property).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_constructor_is_correct(bool useVB)
        {
            var ctor = new Constructor(new TypeRep("ClassName"));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public ClassName()
                                     {
                                     }",
                VBGenerator => @"Public Sub New()
                                 End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputConstructor(ctor).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_constructor_with_base_is_correct(bool useVB)
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .BaseCall();
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public Abc()
                                     : base()
                                     {
                                     }",
                VBGenerator => @"Public Sub New()
                                     MyBase.New()
                                 End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputConstructor(ctor).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_constructor_with_this_is_correct(bool useVB)
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .ThisCall();
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public Abc()
                                     : this()
                                     {
                                     }",
                VBGenerator => @"Public Sub New()
                                     Me.New()
                                 End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputConstructor(ctor).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_constructor_with_base_and_argument_is_correct(bool useVB)
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .BaseCall(Expression.Value(42));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public Abc()
                                     : base(42)
                                     {
                                     }",
                VBGenerator => @"Public Sub New()
                                     MyBase.New(42)
                                 End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputConstructor(ctor).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_constructor_with_this_and_argument_is_correct(bool useVB)
        {
            var ctor = new Constructor(new TypeRep("Abc"))
                       .ThisCall(Expression.Value(42));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @"public Abc()
                                     : this(42)
                                     {
                                     }",
                VBGenerator => @"Public Sub New()
                                     Me.New(42)
                                 End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputConstructor(ctor).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_method_is_correct(bool useVB)
        {
            var method = new Method(MethodName);
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"public void {MethodName}()
                                    {{
                                    }}",
                VBGenerator => $@"Public Sub {MethodName}()
                                  End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputMethod(method).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_method_is_correct_with_returnType(bool useVB)
        {
            var method = new Method(MethodName)
                            .ReturnType("string");
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"public string {MethodName}()
                                    {{
                                    }}",
                VBGenerator => $@"Public Function {MethodName}() As String
                                  End Function",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputMethod(method).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_method_with_parameters_is_correct(bool useVB)
        {
            var method = new Method(MethodName)
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"public void {MethodName}(int value, string s)
                                    {{
                                    }}",
                VBGenerator => $@"Public Sub {MethodName}(value As Integer, s As String)
                                  End Sub",

                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputMethod(method).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_method_with_returnType_and_parameters_is_correct(bool useVB)
        {
            var method = new Method(MethodName)
                            .ReturnType("string")
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"public string {MethodName}(int value, string s)
                                    {{
                                    }}",
                VBGenerator => $@"Public Function {MethodName}(value As Integer, s As String) As String
                                  End Function",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputMethod(method).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_method_with_scope_is_correct(bool useVB)
        {
            var method = new Method(MethodName, scope: Scope.Protected)
                            .ReturnType("string")
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"protected string {MethodName}(int value, string s)
                                    {{
                                    }}",
                VBGenerator => $@"Protected Function {MethodName}(value As Integer, s As String) As String
                                  End Function",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputMethod(method).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_method_with_modifiers_is_correct(bool useVB)
        {
            var method = new Method(MethodName, modifiers: MemberModifiers.Partial | MemberModifiers.Override)
                            .ReturnType("string")
                            .Parameter("value", typeof(int))
                            .Parameter("s", typeof(string));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"public partial override string {MethodName}(int value, string s)
                                    {{
                                    }}",
                VBGenerator => $@"Public Partial Overrides Function {MethodName}(value As Integer, s As String) As String
                                  End Function",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputMethod(method).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_constructor_with_parameters_is_correct(bool useVB)
        {
            var ctor = new Constructor(new TypeRep("ClassName"))
                         .Parameter("abc", typeof(string))
                         .Parameter("def", typeof(int));
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"public ClassName(string abc, int def)
                                    {{
                                    }}",
                VBGenerator => $@"Public Sub New(abc As String, def As Integer)
                                  End Sub",
                _ => throw new NotImplementedException()
            };

            var actual = generator.OutputConstructor(ctor).GetOutput();

            actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory(Skip = "TBD")]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_field_is_correct(bool useVB)
        {
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"",
                VBGenerator => $@"",
                _ => throw new NotImplementedException()
   
            };
            //var actual = generator.OutputConstructor(ctor).GetOutput();

            //actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }

        [Theory(Skip = "TBD")]
        [InlineData(true)]
        [InlineData(false)]
        public void Generated_field_with_initializer_is_correct(bool useVB)
        {
            var generator = GeneratorBase.Generator(useVB ? "VisualBasic" : "C#");;
            var expected = generator switch
            {
                CSharpGenerator => @$"",
                VBGenerator => $@"",
                _ => throw new NotImplementedException()

            };
           // var actual = generator.OutputConstructor(ctor).GetOutput();

          //  actual.NormalizeWhitespace().Should().Be(expected.NormalizeWhitespace());
        }
    }
}
