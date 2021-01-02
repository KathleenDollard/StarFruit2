using FluentAssertions;
using FluentDom.Generator;
using System;
using System.Collections.Generic;
using Xunit;

namespace FluentDom.Tests
{
    public class GeneralTests
    {

        [Theory]
        [InlineData(typeof(Boolean), "bool", "Boolean")]
        [InlineData(typeof(Byte), "byte", "Byte")]
        [InlineData(typeof(SByte), "sbyte", "SByte")]
        [InlineData(typeof(Char), "char", "Char")]
        [InlineData(typeof(Decimal), "decimal", "Decimal")]
        [InlineData(typeof(Double), "double", "Double")]
        [InlineData(typeof(Single), "float", "Single")]
        [InlineData(typeof(Int32), "int", "Integer")]
        [InlineData(typeof(UInt32), "uint", "UInteger")]
        [InlineData(typeof(Int64), "long", "Long")]
        [InlineData(typeof(UInt64), "ulong", "ULong")]
        [InlineData(typeof(Int16), "short", "Short")]
        [InlineData(typeof(UInt16), "ushort", "UShort")]
        [InlineData(typeof(Object), "object", "Object")]
        [InlineData(typeof(String), "string", "String")]
        [InlineData(typeof(List<int>), "List<int>", "List(Of Integer)")]
        [InlineData(typeof(List<Dictionary<int, string>>), "List<Dictionary<int, string>>", "List(Of Dictionary(Of Integer, String))")]
        [InlineData(typeof(Dictionary<int, List<int>>), "Dictionary<int, List<int>>", "Dictionary(Of Integer, List(Of Integer))")]
        public void TypeRep_from_Type_outputs_typestring(Type type, string cSharpExpected, string vbExpected)
        {
            var typeRep = new TypeRep(type);

            var cSharpActual = typeRep.CSharpString();
            var vbActual = typeRep.VBString();

            cSharpActual.Should().Be(cSharpExpected);
            vbActual.Should().Be(vbExpected);
        }

        [Fact]
        public void Single_parameter_output()
        {
            var parameterName = "Abc";
            var ctor = new Constructor(TypeRep.Object)
                        .Parameter(parameterName, typeof(string));
            var cSharpExpected = $"string {parameterName}";
            var vbExpected = $"{parameterName} As String";

            var actualCSharp = ctor.ParameterStore.CSharpString();
            var actualVB = ctor.ParameterStore.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Multiple_parameter_output()
        {
            var ctor = new Constructor(TypeRep.Object)
                        .Parameter("Abc", typeof(string))
                        .Parameter("Def", typeof(int));
            var cSharpExpected = "string Abc, int Def";
            var vbExpected = "Abc As String, Def As Integer";

            var actualCSharp = ctor.ParameterStore.CSharpString();
            var actualVB = ctor.ParameterStore.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);

        }

        [Fact]
        public void Single_argument_output()
        {
            var parameterName = "Abc";
            var methodCall = new MethodCall("Dummy", Expression.Value(parameterName));
            var cSharpExpected = $@"""{parameterName}""";
            var vbExpected = cSharpExpected;

            var actualCSharp = methodCall.ArgumentStore.CSharpString();
            var actualVB = methodCall.ArgumentStore.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);
        }

        [Fact]
        public void Multiple_argument_output()
        {
            var methodCall = new MethodCall("Dummy", Expression.Value("Abc"), Expression.Value("Def"));
            var cSharpExpected = @"""Abc"", ""Def""";
            var vbExpected = cSharpExpected;

            var actualCSharp = methodCall.ArgumentStore.CSharpString();
            var actualVB = methodCall.ArgumentStore.VBString();

            actualCSharp.Should().Be(cSharpExpected);
            actualVB.Should().Be(vbExpected);

        }
    }
}
