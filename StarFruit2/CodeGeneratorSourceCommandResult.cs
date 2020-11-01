using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarFruit2
{
    public class CodeGeneratorCommandSourceResult
    {
        private Generate generate;


        public CodeGeneratorCommandSourceResult(Generate generate)
        {
            this.generate = generate;
        }

        public List<string> GenerateSourceCode(CliDescriptor cliDescriptor)
        {
            var Libraries = new string[]
            {
                "StarFruit2",
                "System.CommandLine",
                "StarFruit2.Common",
                "System.CommandLine.Invocation",
                "System.CommandLine.Parsing"
            };

            var commandDescriptors = new List<CommandDescriptor>
            {
                cliDescriptor.CommandDescriptor
            };

            List<string> strCollection = new List<string>();
            strCollection.AddRange(generate.Usings(Libraries));
            strCollection.AddRange(generate.Namespace(cliDescriptor.GeneratedComandSourceNamespace,
                               commandDescriptors.SelectMany(c =>
                                            generate.Class(scope: Scope.Public,
                                              isPartial: true,
                                              className: $"{c.OriginalName}CommandSourceResult",
                                              baseClassName: GetBaseClassName(c),
                                              classBody: GeClassBody(c)))));

            return strCollection;

            string GetBaseClassName(CommandDescriptor commandDescriptor)
            {
                return commandDescriptor.IsRoot
                               ? generate.GenericType("CommandSourceResult", commandDescriptor.OriginalName)
                               : "CommandSourceResult";
            }
        }

        private List<string> GeClassBody(CommandDescriptor commandDescriptor)
        {
            var strCollection = new List<string> { };

            var commandSourceName = $"{commandDescriptor.OriginalName}CommandSourceResult";
            List<string> ctorArgs = GetCtorArgs(commandSourceName);
            List<string> baseArgs = GetBaseArgs(commandDescriptor, commandSourceName);

            // Jean: Constructor needs a scope
            strCollection.AddRange(generate.Constructor(commandSourceName,
                                                        ctorArgs,
                                                        baseArgs,
                                                        commandDescriptor.GetOptionsAndArgs()
                                                                         .Select(PropertyAssignment(generate))));
            strCollection.AddRange(Properties(commandDescriptor));
            strCollection.AddRange(CreateInstanceMethod(commandDescriptor));
            return strCollection;

            static List<string> GetCtorArgs(string commandSourceName, Generate generate)
            {
                return new List<string>
                {
                    generate.Parameter("ParseResult","parseResult"),
                    generate.Parameter(commandSourceName, commandSourceName.CamelCase())
                };
            }

            static List<string> GetBaseArgs(CommandDescriptor commandDescriptor, string commandSourceName)
            {
                var baseArgs = new List<string>
                {
                     "parseResult",
                };
                if (commandDescriptor.IsRoot)
                {
                    baseArgs.Add(commandSourceName.CamelCase());
                }

                return baseArgs;
            }

            static string ResultPropertyName(string name)
            {
                return $"{name}_Result";
            }

            static Func<SymbolDescriptor, string> PropertyAssignment(Generate generate)
            {
                return o => generate.Assign(ResultPropertyName(o.OriginalName),
                                                generate.MethodCall("CommandSourceMemberResult.Create", $"commandSource.{o.OriginalName }"));
            }
        }


    }
}
