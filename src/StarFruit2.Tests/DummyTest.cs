﻿using System.Collections.Generic;
using StarFruit2.Common.Descriptors;
using StarFruit2.Common;

namespace StarFruit2.Tests
{
    public class DummyTest
    {
        private CliDescriptor GetCliDescriptor()
        {
            var commandDescriptor = new CommandDescriptor(null, "MyClass", RawInfo.DummyClass)
            {
                Name = "my-class",
               // Parent = null,
            };
           // commandDescriptor.Root = commandDescriptor;
            var testName = "testName";
            var GeneratedNamespace = "StarFruit2.Tests.TestSampleData" + testName;
            var GeneratedSourceClassName = testName + "CommandSource";
            string originalName = "SomeOption";
            string commandLineName = "some-option";
            string description = "desc";
            var defaultValue = new DefaultValueDescriptor("abc");

            var option = new OptionDescriptor(commandDescriptor, originalName, RawInfo.DummyProperty)
            {
                Name = originalName,
                CliName = commandLineName,
                Description = description,
            };
            option.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(typeof(string)), null, originalName, RawInfo.DummyProperty)
            {
                Name = originalName,
                CliName = commandLineName,
                Description = description,
                DefaultValue = defaultValue,
            });

            commandDescriptor.AddOptions(options:
                new List<OptionDescriptor>()
                {
                     option
                }
            );

            return new CliDescriptor
            {
                GeneratedComandSourceNamespace = GeneratedNamespace,
                GeneratedCommandSourceClassName = GeneratedSourceClassName,
                CommandDescriptor = commandDescriptor,
            };
        }

    }
}
