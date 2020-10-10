using Microsoft.CodeAnalysis;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarFruit2.Generator
{
    public static class CodeGeneratorExtensions
    {
        public static string WrapIfFlagged(this string code,
                                           CodeGenerator.Include match,
                                           CodeGenerator.Include include,
                                           Func<string, string> wrapOperation) 
            => include.HasFlag(match)
                    ? wrapOperation(code)
                    : code;
        public static string PrefaceWithUsing(this string code)
            =>  $"using System.CommandLine;\n{code}";

        public static string WrapInNamespace(this string code, string namespaceName)
            =>  $"\nnamespace {namespaceName}\n{{\n{code}\n}}";

        public static string WrapInClass(this string code, string className)
           =>  $"\npublic class {className}\n{{\n{code}\n}}";

        public static string WrapInMethod(this string code, string methodName)
           =>   $"\npublic Command {methodName}()\n{{\n{code}\n}}";
    }

    public static class CodeGenerator
    {
        [Flags]
        public enum Include
        {
            Default = 0,
            CommandCode = 0b1,
            Method = 0b10,
            Class = 0b100,
            Namespace = 0b1000,
            Usings = 0b1000_0,
            All = 0b1111_1111
        }

        /// <summary>
        /// Get the source code generated as a class within a namespace with the required using
        /// </summary>
        /// <param name="cliDescriptor">The CliDescriptor that contains the definition of the command</param>
        /// <returns>The source code ready for a source generator or other use</returns>
        public static string GetSourceCode(CliDescriptor cliDescriptor) 
            => GetSourceCode(cliDescriptor, Include.All);

        /// <summary>
        /// Get the source code generated as a class with the specified wrapping. This can be used to create the
        /// generated source without a namespace for use in .NET Interactive, or to limit wrapping for easier tests.
        /// </summary>
        /// <param name="cliDescriptor">The CliDescriptor that contains the definition of the command</param>
        /// <returns>The source code ready for a source generator, the .NET Interactive kernel or other use</returns>
        public static string GetSourceCode(CliDescriptor cliDescriptor, Include include) 
            => GetCommandCode(cliDescriptor, include)
                .WrapIfFlagged(Include.Method, include, c => c.WrapInMethod("GetCommand"))
                .WrapIfFlagged(Include.Class, include, c => c.WrapInClass(cliDescriptor.GeneratedCommandSourceClassName))
                .WrapIfFlagged(Include.Namespace, include, c => c.WrapInNamespace(cliDescriptor.GeneratedComandSourceNamespace))
                .WrapIfFlagged(Include.Usings, include, c => c.PrefaceWithUsing());

        private static string GetCommandCode(CliDescriptor cliDescriptor, Include include)
        {
            var commandDescriptor = cliDescriptor.CommandDescriptor;
            return !include.HasFlag(Include.CommandCode)
                ? ""
                : $@"
            var command = new Command(""{commandDescriptor.Name}"", ""{commandDescriptor.Description}"");
            {AddArguments(commandDescriptor)}
            {AddOptions(commandDescriptor)}
            return command;";
        }

        private static string AddArguments(CommandDescriptor commandDescriptor)
        {
            var ret = new List<string>();
            foreach (var argument in commandDescriptor.Arguments)
            {
                var arg = GetArgument(argument);
                //ret.Add($@"Command.Arguments.Add(new Argument({argument.Description})
                //                {{ArgumentType = typeof({argument.ArgumentType.TypeAsString()}}});");
                ret.Add($@"command.Arguments.Add({arg});");
            }
            return string.Join("\n", ret);
        }

        private static string GetArgument(ArgumentDescriptor argument)
        {
            return $@"GetArg<{argument.ArgumentType.TypeAsString()}>(""{argument.CommandLineName}"", ""{argument.Description}"", {argument.DefaultValue.CodeRepresentation})";
        }

        private static string AddOptions(CommandDescriptor commandDescriptor)
        {
            var ret = new List<string>();
            foreach (var option in commandDescriptor.Options)
            {

                ret.Add($@"Command.Options.Add(new Option({option.Name}, {option.Description})
                              Argument = new Argument({option.Name})
                                 {{ArgumentType = typeof({option.Arguments.First().ArgumentType.TypeAsString()}}});");
            }
            return string.Join("\n", ret);
        }
    }
}
