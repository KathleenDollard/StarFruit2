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
        [InlineData(typeof(Boolean), "bool")]
        [InlineData(typeof(Byte), "byte")]
        [InlineData(typeof(SByte), "sbyte")]
        [InlineData(typeof(Char), "char")]
        [InlineData(typeof(Decimal), "decimal")]
        [InlineData(typeof(Double), "double")]
        [InlineData(typeof(Single), "float")]
        [InlineData(typeof(Int32), "int")]
        [InlineData(typeof(UInt32), "uint")]
        [InlineData(typeof(Int64), "long")]
        [InlineData(typeof(UInt64), "ulong")]
        [InlineData(typeof(Int16), "short")]
        [InlineData(typeof(UInt16), "ushort")]
        [InlineData(typeof(Object), "object")]
        [InlineData(typeof(String), "string")]
        [InlineData(typeof(List<int>), "List<int>")]
        [InlineData(typeof(List<Dictionary<int, string>>), "List<Dictionary<int, string>>")]
        [InlineData(typeof(Dictionary<int, List<int>>), "Dictionary<int, List<int>>")]
        public void TypeRep_from_Type_outputs_typestring(Type type, string expected)
        {
            var typeRep = new TypeRep(type);

            var actual = typeRep.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Single_parameter_output()
        {
            var parameterName = "Abc";
            var ctor = new Constructor(TypeRep.Object)
                        .Parameter(parameterName, typeof(string));
            var expected = $"string {parameterName}";

            var actual = ctor.ParameterStore.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Multiple_parameter_output()
        {
            var ctor = new Constructor(TypeRep.Object)
                        .Parameter("Abc", typeof(string))
                        .Parameter("Def", typeof(int));
            var expected = "string Abc, int Def";

            var actual = ctor.ParameterStore.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Single_argument_output()
        {
            var parameterName = "Abc";
            var methodCall = new MethodCall("Dummy", Expression.Value(parameterName));
            var expected = $@"""{parameterName}""";

            var actual = methodCall.ArgumentStore.CSharpString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void Multiple_argument_output()
        {
            var methodCall = new MethodCall("Dummy", Expression.Value("Abc"), Expression.Value("Def"));
            var expected = @"""Abc"", ""Def""";

            var actual = methodCall.ArgumentStore.CSharpString();

            actual.Should().Be(expected);
        }

      

    }
}
