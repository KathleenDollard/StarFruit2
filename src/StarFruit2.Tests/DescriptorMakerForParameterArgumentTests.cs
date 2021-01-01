using FluentAssertions;
using FluentAssertions.Execution;
using StarFruit.Common;
using System;
using System.Linq;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForParameterArgumentTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Argument_names_are_as_expected(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

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
                public int MyMethod(int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void ArgumentType_is_bool_when_int_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(bool myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Boolean) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Required] int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Required> myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_true_when_Required_attribute_with_true_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Required(true)] int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Required(true)> myParamArg As Integer) As Integer
                    Return 0
                End Function";


            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.Required.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_not_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Required_is_false_when_Required_attribute_with_false_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Required(false)] int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Required(false)> myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.Required.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Hidden] int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Hidden> myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Hidden(true)] int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Hidden(true)> myParamArg As Integer) As Integer
                    Return 0
                End Function";


            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.IsHidden.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_not_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod([Hidden(false)] int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<Hidden(false)> myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.IsHidden.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Description_from_xml_on_argument(Type utilType)
        {
            var desc = "This is a nice param desc";
            var cSharpCode = @$"
                /// <summary>
                /// 
                /// </summary>
                /// <param name=""myParamArg"">{desc}</param>
                public int MyMethod(int myParamArg)
                {{ return 0; }}";
            var vBasicCode = @$"
                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name=""myParamArg"">{desc}</param>
                Public Function MyMethod(myParamArg As Integer) As Integer 
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_from_parameter_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParamArg=42)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(Optional myParamArg As Integer = 42) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("42");
        }


        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_is_null_If_not_given_as_parameter_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.DefaultValue.Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void DefaultValue_as_parameter_from_parameter_initialize_on_argument(Type utilType)
        {
            var cSharpCode = @"
                public void MyMethod(int myParamArg=System.Int32.MinValue) {} ";
            var vBasicCode = @"
                Public Function MyMethod(Optional myParamArg As Integer = System.Int32.MinValue) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.DefaultValue.CodeRepresentation.Should().Be("-2147483648");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Allowed_values_from_attribute_on_argument(Type utilType)
        {
            var cSharpCode = @"                       
                public int MyMethod([AllowedValues(1,3,5,7,11,13)] int myParamArg=System.Int32.MinValue)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(<AllowedValues(1, 3, 5, 7, 11, 13)> Optional myParamArg As Integer = System.Int32.MinValue) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.AllowedValues.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Parameter_position_correct_for_arguments_and_options(Type utilType)
        {
            var cSharpCode = @"                       
                public int MyMethod(int first, string secondArg, int third)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(first As Integer, secondArg As String, third As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual1 = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();
            var actual2 = actualCli.CommandDescriptor.SubCommands.First().Options.First();
            var actual3 = actualCli.CommandDescriptor.SubCommands.First().Options.Skip(1).First();

            using var x = new AssertionScope();
            actual1.Position.Should().Be(1);
            actual2.Position.Should().Be(0);
            actual3.Position.Should().Be(2);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void RawInfo_correct_for_argument(Type utilType)
        {
            var cSharpCode = @"
                public int MyMethod(int myParamArg)
                { return 0; }";
            var vBasicCode = @"
                Public Function MyMethod(myParamArg As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Arguments.First();

            actual.RawInfo.Should().BeOfType<RawInfoForMethodParameter>();
        }

    }
}
