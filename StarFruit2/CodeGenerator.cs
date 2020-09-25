using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StarFruit2
{
    public class CodeGenerator
    {
        public string GetSourceCode(CommandDescriptor commandDescriptor)
        {
            return $@"
   internal static class CommandSource
   {{
        public Command GetCommand()
        {{
            {GetCommand(commandDescriptor)}
        }}
   }}
  
";

         
        }
        private string GetCommand(CommandDescriptor commandDescriptor )
        {
            return $@"
            var command = new Command({commandDescriptor.Name}, {commandDescriptor.Description});
            {AddArguments(commandDescriptor)}
            {AddOptions(commandDescriptor)};
            return command;         
";
        }

        public string AddArguments(CommandDescriptor commandDescriptor)
        {
            var ret = new List<string>();
            foreach (var argument in commandDescriptor.Arguments )
            {
                ret.Add($@"Command.Arguments.Add(new Argument({argument.Name})
                                {{ArgumentType = typeof({argument.ArgumentType.GetArgumentType<string>()}}});");
            }
            return string.Join("\n", ret);
        }

        public string AddOptions(CommandDescriptor commandDescriptor)
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
