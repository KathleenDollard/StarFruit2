using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StarFruit2;
using System;
using System.CommandLine;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StarFruit.FluentDomSourceGen.Tests
{
    internal static class Extensions
    {
        internal static string NormalizeLineEndings(this string value)
        {
            // normalize unicode, combining characters, diacritics
            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        internal static string NormalizeWhitespace(this string value)
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

        public static Compilation GetStarFruitCompilation(this SyntaxTree sytanxTree)
        {

            const string longName = "System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            var systemCollectionsAssembly = Assembly.Load(longName);

            MetadataReference fixcCS0012 =
                       MetadataReference.CreateFromFile(systemCollectionsAssembly.Location);
            MetadataReference mscorlib =
                       MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference netStandard =
                      MetadataReference.CreateFromFile(typeof(MethodInfo).Assembly.Location);
            MetadataReference starFruit2Common =
                       MetadataReference.CreateFromFile(typeof(RequiredAttribute).Assembly.Location);
            MetadataReference starFruit2 =
                      MetadataReference.CreateFromFile(typeof(CommandSource).Assembly.Location);
            MetadataReference systemCommandLine =
                      MetadataReference.CreateFromFile(typeof(Command).Assembly.Location);
            MetadataReference systemRuntime =
                        MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location);

            MetadataReference[] references =
                {
                    mscorlib,
                    netStandard,
                    starFruit2,
                    starFruit2Common,
                    systemCommandLine,
                    systemRuntime,
                    fixcCS0012
                };

            var compilation = CSharpCompilation.Create("TransformationCS",
                                            new SyntaxTree[] { sytanxTree },
                                            references,
                                            new CSharpCompilationOptions(
                                                    OutputKind.DynamicallyLinkedLibrary));
            return compilation;
        }
    }

}
