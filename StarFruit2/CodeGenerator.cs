using StarFruit2.Common.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2
{
    public static class CodeGenerator
    {
        public static string GetSourceCode(CliDescriptor cliDescriptor)
        {
            var commandDescriptor = cliDescriptor.CommandDescriptor;
            return $@"
using System.CommandLine;

namespace {cliDescriptor.GeneratedComandSourceNamespace}
{{
   public class {cliDescriptor.GeneratedCommandSourceClassName} : ICommandSource
   {{
        public Command GetCommand()
        {{{GetCommand(commandDescriptor)}
        }}
   }}
}}
  
";

         
        }
        private static string GetCommand(CommandDescriptor commandDescriptor )
        {
            return $@"
            var command = new Command(""{commandDescriptor.Name}"", ""{commandDescriptor.Description}"");
            {AddArguments(commandDescriptor)}
            {AddOptions(commandDescriptor)}
            return command;";
        }

        public static string AddArguments(CommandDescriptor commandDescriptor)
        {
            var ret = new List<string>();
            foreach (var argument in commandDescriptor.Arguments )
            {
                ret.Add($@"Command.Arguments.Add(new Argument({argument.Name})
                                {{ArgumentType = typeof({argument.ArgumentType.GetArgumentType<string>()}}});");
            }
            return string.Join("\n", ret);
        }

        public static string AddOptions(CommandDescriptor commandDescriptor)
        {
            var ret = new List<string>();
            foreach (var option in commandDescriptor.Options)
            {

                ret.Add($@"Command.Options.Add(new Option({option.Name}, {option.Description})
                              Argument = new Argument({option.Name})
                                 {{ArgumentType = typeof({option.Arguments.First().ArgumentType.GetArgumentType<string>()}}});");
            }
            return string.Join("\n", ret);
        }
    }
}
