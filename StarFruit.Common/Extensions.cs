using StarFruit2.Common.Descriptors;
using System.Collections.Generic;

namespace StarFruit2.Common
{
    public static class Extensions
    {
        public static void AddOptions(this CommandDescriptor command, IEnumerable<OptionDescriptor> options)
        {
            foreach (var option in options)
            {
                command.Options.Add(option);
            }
        }

        public static void AddCommands(this CommandDescriptor command, IEnumerable<CommandDescriptor> subCommands)
        {
            foreach (var subCommand in subCommands)
            {
                command.SubCommands.Add(subCommand);
            }
        }

        public static void AddArguments(this CommandDescriptor command, IEnumerable<ArgumentDescriptor> arguments)
        {
            foreach (var argument in arguments)
            {
                command.Arguments.Add(argument);
            }
        }

     }
}
