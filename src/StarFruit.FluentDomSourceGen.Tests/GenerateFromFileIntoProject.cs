using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using System.IO;
using Xunit;
using Utils = StarFruit.FluentDomSourceGen.Tests.SourceGeneratorUtilities;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class GenerateFromFileIntoProject
    {
        private const string commandSource = "CommandSource";
        private const string commandSourcesult = "CommandSourceResult";
        private const string outputRootPath = "../../../../TestOutput";

        [Theory]
        [InlineData(true, commandSource)]
        [InlineData(true, commandSourcesult)]
        [InlineData(false, commandSource)]
        [InlineData(false, commandSourcesult)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void CliRoot_creates_code_that_will_compile(bool useVB, string generatedCodeName)
        {
            Utils utils = Utils.LanguageUtils(useVB);
            var testProject = "Output";
            var input = $"CliRoot.{Utils.Extension(useVB)}";
            var outputPath = $"{Path.Combine(outputRootPath, testProject)}.{Utils.Extension(useVB)}";
            generatedCodeName = $"CliRoot{generatedCodeName}";

            var output = utils.GenerateAndTest(generatedCodeName, input, outputPath);

            output.Length.Should().BeGreaterThan(1000);
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
        public void CliFromMethod_creates_code_that_will_compile_to_run(bool useVB, string generatedCodeName)
        {
            Utils utils = Utils.LanguageUtils(useVB);
            var testProject = "FromMethodOutput";
            var outputPath = $"{Path.Combine(outputRootPath, testProject)}.{Utils.Extension(useVB)}";
            var input = @$"Program.{Utils.Extension(useVB)}";
            input = Path.Combine(outputPath, input);

            var output = utils.GenerateAndTest(generatedCodeName, input, outputPath, isExe: true);

            output.Length.Should().BeGreaterThan(1000);
            NamerFactory.AdditionalInformation = $"{Utils.LanguageName(useVB)}-{Utils.ShortenedGeneratedCodeName(generatedCodeName)}";
            Approvals.Verify(output);
        }



        // ****** Playspace for you :) *****
        // The intention of this test is for you to copy problematic input code into Temp.cs to allow generator debugging and access to output.
        // If this is resolving a bug, create a test for the simple part of the bug once identified.
        [Theory]
        [InlineData(true, commandSource)]
        [InlineData(true, commandSourcesult)]
        [InlineData(false, commandSource)]
        [InlineData(false, commandSourcesult)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Generate_CommandSource_for_debugging_when_needed(bool useVB, string generatedCodeName)
        {
            Utils utils = Utils.LanguageUtils(useVB);
            var testProject = "PlaySpace";
            var outputPath = $"{Path.Combine(outputRootPath, testProject)}.{Utils.Extension(useVB)}";
            var input = @$"PutYourCodeHere.{Utils.Extension(useVB)}";
            input = Path.Combine(outputPath, input);

            var output = utils.GenerateAndTest(generatedCodeName, input, outputPath);

            output.Length.Should().BeGreaterThan(1000);
        }

        //// The intention of this test is for you to copy problematic input code into Temp.cs to allow generator debugging and access to output
        //// If this is resolving a bug, create a test for the simple part of the bug once identified.
        //[Theory]
        //[InlineData(false)]
        //[InlineData(true)]
        //[UseApprovalSubdirectory("Approvals")]
        //[UseReporter(typeof(VisualStudioReporter))]
        //public void Generate_CommandSourceResult_for_debugging_when_needed(bool useVB)
        //{
        //    Utils.LanguageUtils(useVB).GenerateAndTest("CommandSourceResult", @$"..\..\..\Temp.{Utils.Extension(useVB)}", $"../../../../Temp");
        //}

        // The intention of this test is for you to copy problematic input code into Temp.cs to allow generator debugging and access to output
        // If this is resolving a bug, create a test for the simple part of the bug once identified.
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [UseApprovalSubdirectory("Approvals")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Test_SyntaxReceiver_for_debugging_when_needed(bool useVB)
        {
            var receiver = Utils.LanguageUtils(useVB).FindCandidatesWithSyntaxReceiver(@$"..\..\..\Temp.{Utils.Extension(useVB)}");
            receiver.Candidates.Should().NotBeNullOrEmpty();
            receiver.Candidates.Should().HaveCount(1);
        }
    }
}
