using FluentAssertions;
using StarFruit.Common;
using System.Linq;
using Xunit;
using System;
using StarFruit2.Common.Descriptors;

namespace StarFruit2.Tests
{
    public class DescriptorMakerForParameterTests
    {
        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Option_names_are_as_expected(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod({name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            SymbolDescriptor actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First()
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.OriginalName.Should().Be(name);
            actual.Name.Should().Be("myParam");
            actual.CliName.Should().Be(useArg ? "my-param" : "--my-param");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Option_ArgumentType_is_int_when_int(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod({name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First()
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Int32");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Option_ArgumentType_is_bool_when_int(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(bool {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod({name} As Boolean) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First()
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Arguments.First();

            actual.ArgumentType.TypeAsString().Should().Be("Boolean");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_true_when_Required_attribute_used(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([Required] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Required> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().Required
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Required;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_true_when_Required_attribute_with_true_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([Required(true)] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Required(true)> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().Required
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Required;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_false_when_Required_attribute_not(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod({name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().Required
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Required;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Required_is_false_when_Required_attribute_with_false_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([Required(false)] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Required(false)> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().Required
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Required;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_true_when_Hidden_attribute_used(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([Hidden] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Hidden> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().IsHidden
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().IsHidden;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_true_when_Hidden_attribute_with_true_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([Hidden(true)] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Hidden(true)> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().IsHidden
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().IsHidden;

            actual.Should().Be(true);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_false_when_Hidden_attribute_not(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod({name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().IsHidden
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().IsHidden;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Hidden_is_false_when_Hidden_attribute_with_false_value_used(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([Hidden(false)] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Hidden(false)> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().IsHidden
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().IsHidden;

            actual.Should().Be(false);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Single_alias_via_attribute(Type utilType)
        {
            var name = "myParam";
            var cSharpCode = @$"
                public int MyMethod([Aliases(""x"")] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Aliases(""x"")> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(1);
            actual.Aliases.First().Should().Be("x");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void Multiple_aliases_via_attribute(Type utilType)
        {
            var name = "myParam";
            var cSharpCode = @$"
                public int MyMethod([Aliases(""x"",""y"")] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Aliases(""x"", ""y"")> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(2);
            actual.Aliases.First().Should().Be("x");
            actual.Aliases.Last().Should().Be("y");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils))]
        [InlineData(typeof(VBLanguageUtils))]
        public void No_alias_when_none_given(Type utilType)
        {
            var name = "myParam";
            var cSharpCode = @$"
                public int MyMethod([Aliases()] int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<Aliases()> {name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Aliases.Count().Should().Be(0);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Description_from_xml(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var desc = "This is a nice desc";
            var cSharpCode = @$"
                                       /// <summary>
                                       /// 
                                       /// </summary>
                                       /// <param name=""{name}"">{desc}</param>
                                       public int MyMethod(int {name})
                                       {{ return 0; }}
                            ";
            var vBasicCode = @$"
                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name=""{name}"">{desc}</param>
                Public Function MyMethod({name} As Integer) As Integer 
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            SymbolDescriptor actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First()
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.Description.Should().Be(desc);
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void DefaultValue_from_property_initialize(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(int {name}=42)
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(Optional {name} As Integer = 42) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().DefaultValue
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Arguments.First().DefaultValue; ;

            actual.Should().NotBeNull();
            actual!.CodeRepresentation.Should().Be("42");
        }


        // new object test not included because parameter defaults must be compile time constants


        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void DefaultValue_as_property_from_property_initialize(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod(int {name}=System.Int32.MinValue)
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(Optional {name} As Integer = System.Int32.MinValue) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().DefaultValue
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Arguments.First().DefaultValue; ;

            actual.Should().NotBeNull();
            actual!.CodeRepresentation.Should().Be("-2147483648");
        }

        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void Allowed_values_from_attribute(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"
                public int MyMethod([AllowedValues(1,3,5,7,11,13)] int {name}=System.Int32.MinValue)
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod(<AllowedValues(1, 3, 5, 7, 11, 13)> Optional {name} As Integer = System.Int32.MinValue) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            var actual = useArg
                        ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First().AllowedValues
                        : actualCli.CommandDescriptor.SubCommands.First().Options.First().Arguments.First().AllowedValues; ;

            actual.Should().BeEquivalentTo(new int[] { 1, 3, 5, 7, 11, 13 });
        }


        [Theory]
        [InlineData(typeof(CSharpLanguageUtils), true)]
        [InlineData(typeof(VBLanguageUtils), true)]
        [InlineData(typeof(CSharpLanguageUtils), false)]
        [InlineData(typeof(VBLanguageUtils), false)]
        public void RawInfo_correct_for_option(Type utilType, bool useArg)
        {
            var name = useArg ? "myParamArg" : "myParam";
            var cSharpCode = @$"public int MyMethod(int {name})
                {{ return 0; }}";
            var vBasicCode = @$"
                Public Function MyMethod({name} As Integer) As Integer
                    Return 0
                End Function";

            var actualCli = LanguageUtils.Choose(utilType, cSharpCode, vBasicCode)
                .WrapInClass()
                .CliDescriptorForClass(out var code);

            SymbolDescriptor actual = useArg
                         ? actualCli.CommandDescriptor.SubCommands.First().Arguments.First()
                         : actualCli.CommandDescriptor.SubCommands.First().Options.First();

            actual.RawInfo.Should().BeOfType<RawInfoForMethodParameter>();
        }
    }
}
