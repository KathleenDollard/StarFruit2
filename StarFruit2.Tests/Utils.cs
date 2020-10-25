using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Starfruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generator;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace StarFruit2.Tests
{
    internal static class Utils
    {
        private const string testNamespace = "StarFruit2.Tests";

        // From StackOverflow user11523568 with modification
        // This is a bit of a hack. 
        internal static string Normalize(string value)
        {
            if (value is null)
            {
                throw new InvalidOperationException("You cannot normalize strings that are null");
            }
            // normalize unicode, combining characters, diacritics
            value = value.Normalize(NormalizationForm.FormC);
            value = value.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = value.Split("\n");
            value = string.Join("\n", lines.Select(x => x.Trim()));
            value = ReplaceWithRepeating(value, "\n\n", "\n");
            value = ReplaceWithRepeating(value, "  ", " ");

            return value.Trim();
        }

        private static string ReplaceWithRepeating(string value, string oldValue, string newValue)
        {
            var len = value.Length + 1; // plus one forces this to run at least once
            while (len != value.Length)
            {
                len = value.Length;
                value = value.Replace(oldValue, newValue);
            }

            return value;
        }

        internal static CliDescriptor GetCliFromFile(string fileName)
            => GetCli(File.ReadAllText($"TestData/{fileName}"));

        internal static CliDescriptor GetCli(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = GetCompilation(tree);
            var rootCommand = tree.GetRoot().DescendantNodes()
                               .OfType<ClassDeclarationSyntax>()
                               .FirstOrDefault();
            if (rootCommand is null)
            {
                return null;
            }
            var cli = RoslyDescriptorMakerFactory.CreateCliDescriptor(rootCommand, compilation);
            return cli;
        }

        internal static CSharpCompilation GetCompilation(SyntaxTree tree)
        {
            const string longName = "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            var systemCollectionsAssembly = Assembly.Load(longName);

            MetadataReference fixcCS0012 =
                       MetadataReference.CreateFromFile(systemCollectionsAssembly.Location);
            MetadataReference mscorlib =
                       MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference starFruit2Common =
                       MetadataReference.CreateFromFile(typeof(RequiredAttribute).Assembly.Location);
            MetadataReference systemRuntime =
                        MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location);
            MetadataReference[] references = { mscorlib, starFruit2Common, systemRuntime, fixcCS0012 };

            var compilation = CSharpCompilation.Create("TransformationCS",
                                            new SyntaxTree[] { tree },
                                            references,
                                            new CSharpCompilationOptions(
                                                    OutputKind.DynamicallyLinkedLibrary));
            return compilation;
        }

        internal static CliDescriptor WrapInCliDescriptor(this CommandDescriptor commandDescriptor)
            => new CliDescriptor
            {
                CommandDescriptor = commandDescriptor
            };

        internal static CliDescriptor WrapInCliDescriptor(this OptionDescriptor optionDescriptor)
        {
            var clidDescriptor = new CliDescriptor()
            {
                CommandDescriptor = new CommandDescriptor(null, "MyClass", null)

            };
            clidDescriptor.CommandDescriptor.Options.Add(optionDescriptor);
            return clidDescriptor;
        }

        internal static CliDescriptor WrapInCliDescriptor(this ArgumentDescriptor argumentDescriptor)
        {
            var clidDescriptor = new CliDescriptor()
            {
                CommandDescriptor = new CommandDescriptor(null, "MyClass", null),
                GeneratedComandSourceNamespace = "MyNamespace"
            };
            clidDescriptor.CommandDescriptor.Arguments.Add(argumentDescriptor);
            return clidDescriptor;
        }

        internal static string WrapInClassAndNamespace(string className)
        {
            throw new NotImplementedException();
        }

        internal static OptionDescriptor CreateOptionDescriptor(string name, Type type)
        {
            var option = new OptionDescriptor(null, name, null);
            option.Arguments.Add(new ArgumentDescriptor(new ArgTypeInfoRoslyn(type), null, name, null));
            return option;
        }

        internal static string WrapInStandardClass(this string code)
       => code.WrapInClass("MyClass")
              .WrapInNamespace("MyNamespace")
              .PrefaceWithUsing("StarFruit2");

        internal static string WrapInStandardNamespace(this string code)
        => code.WrapInNamespace("MyNamespace")
               .PrefaceWithUsing("StarFruit2");
    }
}
