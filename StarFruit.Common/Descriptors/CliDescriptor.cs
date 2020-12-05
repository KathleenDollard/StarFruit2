using StarFruit.Common;
using System;
using System.Collections.Generic;

namespace StarFruit2.Common.Descriptors
{
    public class CliDescriptor
    {
        private string? generatedCommandSourceClassName;

        public string? GeneratedComandSourceNamespace { get; set; }

        // don't need this any more
        public string? GeneratedCommandSourceClassName
        {
            get
                => generatedCommandSourceClassName
                    ?? $"{GeneratedComandSourceNamespace?.AfterLast(".")}CommandSource";

            set => generatedCommandSourceClassName = value;
        }

        // TODO: This should be a constructor parameter, but the change will be painful
        public CommandDescriptor CommandDescriptor { get; set; }

        public IEnumerable<ISymbolDescriptor> Descendants
        {
            get
            {
                return GetDescendants(new List<ISymbolDescriptor>(), CommandDescriptor);
            }
        }

        private IEnumerable<ISymbolDescriptor> GetDescendants(List<ISymbolDescriptor> descendants, CommandDescriptor commandDescriptor)
        {
            descendants.AddRange(commandDescriptor.Options);
            descendants.AddRange(commandDescriptor.Arguments);
            foreach (var subCommand in commandDescriptor.SubCommands)
            {
                descendants.Add(subCommand);
                GetDescendants(descendants, subCommand);
            }

            return descendants;
        }
    }
}
