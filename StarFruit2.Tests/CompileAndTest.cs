using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestData;
using Xunit;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Tests.Utility;
using System.Reactive.Linq;
using Microsoft.DotNet.Interactive.Events;
using System.CommandLine;

namespace StarFruit2.Tests
{
    public class CompileAndTest
    {
        private readonly Assembly testAssembly;
        public CompileAndTest()
        {
            //testAssembly = CreateAssemblyFromCode();
        }

        [Theory]
        [InlineData(typeof(EmptyTestData))]
        public void Generated_code_builds_for_Empty(Type testDataType)
        {
            var testData = Activator.CreateInstance(testDataType) as BaseTestData;
            var type = testAssembly.GetTypes()
                        .Where(x => x.Name == testDataType.Name)
                        .FirstOrDefault();
            type.Should().NotBeNull("The expected type was not found in the TestCompilation");
            var instance = Activator.CreateInstance(type) as ICommandSource;
            instance.Should().NotBeNull("The expected type was found but could not be instantiated or was not an ICommandSource");

            var command = instance.GetCommand();
            testData.TestAction(command);
        }

        [Fact]
        public async Task Demo_dotnet_interactive_test()
        {
            string x = @"using System.CommandLine;
public class Foo
{ 
  public Command Bar() 
  {
    return new Command(""Fred"");
  }
}";

            using var kernel = new CSharpKernel();
            var result = await kernel.SubmitCodeAsync(x);
            result.KernelEvents.ToSubscribedList().Should().NotContainErrors();
            var result2 = await kernel.SubmitCodeAsync("new Foo().Bar()");
            result2.KernelEvents.ToSubscribedList().Should().NotContainErrors();
            var returnValue = await result2.KernelEvents.OfType<ReturnValueProduced>().SingleAsync();
            var foo = returnValue.Value;
            var cmd = foo as Command;
            cmd.Name.Should().Be("Fred");

        }



        private Assembly CreateAssemblyFromCode()
        {
            var dllName = "TestCompilation";
            var testDirectory = $"../../../../../CompilationTests";
            CreateAndWriteProject(testDirectory);

            CreateAndWriteClass(new EmptyTestData(), testDirectory, "Empty");
            // More classes
            var processInfo = new ProcessStartInfo("dotnet.exe")
            {
                WorkingDirectory = new FileInfo(testDirectory).FullName,
                Arguments = $"build"
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Build Failed: {process.ProcessName}");
            }
            var assemblyFileInfo = new FileInfo($"{testDirectory}/{dllName}.dll");
            var assembly = Assembly.LoadFile(assemblyFileInfo.FullName);
            Console.WriteLine(assembly.Location);
            return assembly;
        }

        private void CreateAndWriteProject(string testDirectory)
        {
            var code = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
	  <TargetFramework>netcoreapp3.1</TargetFramework>
	  <LangVersion>8.0</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""System.CommandLine"" Version=""2.0.0-beta1.20371.2""/>   
  </ItemGroup>

</Project>";
            var fileInfo = new FileInfo($"{testDirectory}/TestCompilation.csproj");
            File.WriteAllText(fileInfo.FullName, code);
        }

        private void CreateAndWriteClass(BaseTestData testData, string testDirectory, string fileName)
        {
            var fileInfo = new FileInfo($"{testDirectory}/{fileName}.cs");
            File.WriteAllText(fileInfo.FullName, testData.GeneratedSource);
        }
    }
}
