using FluentAssertions;
using FluentAssertions.Execution;
using StarFruit.Common;
using System.Linq;
using Xunit;
using System;

namespace StarFruit2.Tests
{
    public class MyClass
    {
        public MyClass(int myParamArg)
        { }
    }

    public class DescriptorMakerForParameterCtorArgumentTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Argument_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.OriginalName.Should().Be("myParamArg");
            actual.Name.Should().Be("myParam");
            actual.CliName.Should().Be("my-param");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void ArgumentType_is_int_when_int_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void ArgumentType_is_bool_when_int_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(bool myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Boolean)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass([Required] int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<Required> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass([Required(true)] int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<Required(True)> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_not_on_argument(Type utilType)
        {
            var cSharpCode = @"
                 public class MyClass
                {
                    public MyClass(int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass([Required(false)] int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<Required(False)> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass([Hidden] int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<Hidden> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass([Hidden(true)] int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<Hidden(True)> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass([Hidden(false)] int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<Hidden(False)> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Description_from_xml_on_argument(Type utilType)
        {
            var desc = "This is a nice param desc";
            var cSharpCode = @$"
                public class MyClass
                {{
                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name=""myParamArg"">{desc}</param> 
                    public MyClass([Hidden(false)] int myParamArg)
                    {{}}
                }}";
            var vBasicCode = @$"
                Public Class [MyClass]
                    ''' <summary>
                    ''' 
                    ''' </summary>
                    ''' <param name=""myParamArg"">{desc}</param> 
                    Public Sub New(<Hidden(False)> myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_from_parameter_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg=42)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(Optional myParamArg As Integer = 42)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("42");
        }


        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_is_null_If_not_given_as_parameter_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Integer)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_parameter_from_parameter_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg=System.Int32.MinValue)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(Optional myParamArg As Integer = System.Int32.MinValue)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("-2147483648");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Allowed_values_from_attribute_on_argument(Type utilType)
        {
            var cSharpCode = @"                       
                public class MyClass
                {
                    public MyClass([AllowedValues(1,3,5,7,11,13)] int myParamArg=System.Int32.MinValue)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(<AllowedValues(1, 3, 5, 7, 11, 13)> Optional myParamArg As Integer = System.Int32.MinValue)
                    End Sub
                End Class";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Parameter_position_correct_for_arguments_and_options(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int first, string secondArg, int third)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(first As Integer, secondArg As String, third As Integer)
                    End Sub
                End Class
                ";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual1 = actualCli.CommandDescriptor.Arguments.First();
            var actual2 = actualCli.CommandDescriptor.Options.First();
            var actual3 = actualCli.CommandDescriptor.Options.Skip(1).First();

            using var x = new AssertionScope();
            actual1.Position.Should().Be(1);
            actual2.Position.Should().Be(0);
            actual3.Position.Should().Be(2);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void MemberSource_correct_for_argument(Type utilType)
        {
            var cSharpCode = @"
                public class MyClass
                {
                    public MyClass(int myParamArg)
                    {}
                }";
            var vBasicCode = @"
                Public Class [MyClass]
                    Public Sub New(myParamArg As Integer)
                    End Sub
                End Class
                ";
            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInNamespace()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.Arguments.First();

            actual.RawInfo.Should().BeOfType<RawInfoForCtorParameter>();
        }

    }
}
