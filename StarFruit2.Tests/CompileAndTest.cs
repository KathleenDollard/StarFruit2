﻿using FluentAssertions;
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
using StarFruit2.Generator;

namespace StarFruit2.Tests
{
    public class CompileAndTest
    {

        [Theory]
        [InlineData(typeof(EmptyTestData))]
        public async Task Compile_succeeds_for_generated_code(Type testDataType)
        {
            // arrange
            var className = "MyClass";
            var methodName = "GetCommand";
            var testData = Activator.CreateInstance(testDataType) as BaseTestData;
            // TODO: Add null check
            var generatedSource = testData.GeneratedSource
                                    .WrapInMethod(methodName)
                                    .WrapInClass(className)
                                    .PrefaceWithUsing();

            using var kernel = new CSharpKernel();

            // act
            var result = await kernel.SubmitCodeAsync(generatedSource);

            // assert
            result.KernelEvents.ToSubscribedList().Should().NotContainErrors();
        }

        [Theory]
        [InlineData(typeof(EmptyTestData))]
        public async Task Compiled_command_has_expect_values(Type testDataType)
        {
            // arrange
            var className = "MyClass";
            var methodName = "GetCommand";
            var testData = Activator.CreateInstance(testDataType) as BaseTestData;
            // TODO: Add null check
            var generatedSource = testData.GeneratedSource
                                    .WrapInMethod(methodName)
                                    .WrapInClass(className)
                                    .PrefaceWithUsing();

            using var kernel = new CSharpKernel();

            var result = await kernel.SubmitCodeAsync(generatedSource);
            // While assert in arrange is unusual, if this goes bad, the test is toast. 
            result.KernelEvents.ToSubscribedList().Should().NotContainErrors();

            // act
            var resultWithInstance = await kernel.SubmitCodeAsync($"new {className}().{methodName}()");
            resultWithInstance.KernelEvents.ToSubscribedList().Should().NotContainErrors();
 
            // assert
            var returnValue = await resultWithInstance.KernelEvents.OfType<ReturnValueProduced>().SingleAsync();
            var foo = returnValue.Value;
            var cmd = foo as Command;
            cmd.Name.Should().Be("my-class");

        }
    }
}
