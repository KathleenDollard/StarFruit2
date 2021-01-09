using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Utils = StarFruit.FluentDomSourceGen.Tests.SourceGeneratorUtilities;

[assembly: UseReporter(typeof(VisualStudioReporter))]

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class GenerateFromCodeTests
    {
        private const string commandSource = "CommandSource";
        private const string commandSourcesult = "CommandSourceResult";

        // These using choices are deliberate.
        // * StarFruit2.Common is redundant with default, 
        // * System.Threading.Tasks may or may not be in the defualt generation,
        // * System.IO is not in the generation
        private const string csharpClassSource = @"
using StarFruit2;
using StarFruit2.Common;
using System.Threading.Tasks;
using System.IO;

namespace Tests
{
    public class Program
    {
       static int Main(string[] args)
            => CommandSource.Run<CliRoot>(args);
    }
    public class CliRoot
    {
        // Usage: > cli-root <int> --ctor-param --string-property Fred --string-option Flintstone --bool-option
        private bool ctorParam;
        public CliRoot(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public async Task<int> InvokeAsync(int intArg, string stringOption, bool boolOption)
        { return await Task.FromResult(0); }

    }
}";

        private const string vbClassSource = @"
Imports StarFruit2
Imports StarFruit2.Common
Imports System.Threading.Tasks
Imports System.IO

Namespace Tests
    Public Class Program
        Public Shared Function Main(args As String()) As Integer
            Return CommandSource.Run(Of CliRoot)(args)
        End Function
    End Class

    Public Class CliRoot
        ' Usage: > cli-root <int> --ctor-param --string-property Fred --string-option Flintstone --bool-option
        Private ctorParam As Boolean
        Public Sub New(ctorParam As Boolean)
            Me.ctorParam = ctorParam
        End Sub

        Public Property StringProperty As String
        Public Async Function InvokeAsync(intArg As Integer, stringOption As String, boolOption As Boolean) As Task(Of Integer)
            Return Await Task.FromResult(0)
        End Function
    End Class
End Namespace";

        private const string csMultipleGenericAndInterfaceIndicators = @"
using StarFruit2;
using StarFruit2.Common;
using System.Threading.Tasks;
using System.IO;

namespace Tests
{
    public class Program
    {
       static int Main(string[] args)
       {
          var x = CommandSource.Create<CliRoot>();
          CommandSource.Run<CliRoot>(args);
          return 0;
       }
    }
    public class CliRoot : ICliRoot
    {
        private bool ctorParam;
        public CliRoot(bool ctorParam)
            => this.ctorParam = ctorParam;

        public string StringProperty { get; set; }

        public async Task<int> InvokeAsync(int intArg, string stringOption, bool boolOption)
        { return await Task.FromResult(0); }

    }
}";

        private const string vbMultipleGenericAndInterfaceIndicators = @"
Imports StarFruit2
Imports StarFruit2.Common
Imports System.Threading.Tasks
Imports System.IO

Namespace Tests
    Public Class Program
        Public Shared Function Main(args As String()) As Integer
            Dim x = CommandSource.Create(Of CliRoot)()
            CommandSource.Run(Of CliRoot)(args)
            Return 0
        End Function
    End Class

    Public Class CliRoot
        Implements ICliRoot
        Private ctorParam As Boolean
        Public Sub New(ctorParam As Boolean)
            Me.ctorParam = ctorParam
        End Sub

        Public Property StringProperty As String
        Public Async Function InvokeAsync(intArg As Integer, stringOption As String, boolOption As Boolean) As Task(Of Integer)
            Return Await Task.FromResult(0)
        End Function
    End Class
End Namespace
";

        [Theory]
        [InlineData(true, commandSource)]
        [InlineData(true, commandSourcesult)]
        [InlineData(false, commandSource)]
        [InlineData(false, commandSourcesult)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void CommandSource_generator_produces_correct_code(bool useVB, string generatedCodeName)
        {
            var source = useVB
                       ? vbClassSource
                       : csharpClassSource;
            generatedCodeName = $"CliRoot{generatedCodeName}";

            var output = Utils.LanguageUtils(useVB).GenerateAndTestSource(generatedCodeName, source, null);

            NamerFactory.AdditionalInformation = $"{Utils.LanguageName(useVB)}-{Utils.ShortenedGeneratedCodeName(generatedCodeName)}";
            Approvals.Verify(output);
        }

        [Theory]
        [InlineData(true, commandSource)]
        [InlineData(true, commandSourcesult)]
        [InlineData(false, commandSource)]
        [InlineData(false, commandSourcesult)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void CliRoot_creates_a_single_CommandSource_set_for_multiple_indicators_of_root(bool useVB, string generatedCodeName)
        {
            generatedCodeName = $"CliRoot{generatedCodeName}";
            var extension = SourceGeneratorUtilities.Extension(useVB );

            var sourceCode = useVB
                                ? vbMultipleGenericAndInterfaceIndicators
                                : csMultipleGenericAndInterfaceIndicators;
            var cliRootCompilation = Utils.LanguageUtils(useVB).GetCliRootCompilation(sourceCode )
                ?? throw new InvalidOperationException();

            var outputPairs = Utils.LanguageUtils(useVB).Generate(cliRootCompilation, out var outputCompilation, out var generationDiagnostics);

            var matchingPairs = outputPairs.Where(x => x.compilationName.EndsWith($"{generatedCodeName}.generated.{extension}"));
            matchingPairs.Should().HaveCount(1);
            generationDiagnostics.Should().NotHaveErrors($"{generatedCodeName} - Generation diagnostics");
            outputCompilation.Should().NotHaveErrors($"{generatedCodeName} - Generation compilation");
        }
    }
}
