using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorSupport
{
    // fake for early testing
    public class SymbolDescriptor
    {
        public SymbolDescriptor(string originalName)
        {
            OriginalName = originalName;
        }

        public string Name { get; }
        public string OriginalName { get; }

        public string CliName { get; }

        public string Description { get; }
    }

    public class CommandDescriptor : SymbolDescriptor
    {
        public CommandDescriptor(string originalName, bool isRoot)
            : base(originalName)
        {
            IsRoot = isRoot;
        }

        public bool IsRoot { get; }
        public CommandDescriptor Root { get; }
        public CommandDescriptor ParentSymbolDescriptorBase { get; }

        public IEnumerable<ArgumentDescriptor> Arguments { get; } = new List<ArgumentDescriptor>();
        public IEnumerable<OptionDescriptor> Options { get; } = new List<OptionDescriptor>();
        public IEnumerable<CommandDescriptor> SubCommands { get; } = new List<CommandDescriptor>();
    }
    public class ArgumentDescriptor : SymbolDescriptor
    {
        public ArgumentDescriptor(string originalName)
              : base(originalName)
        {
            OriginalName = originalName;
        }
        public string OriginalName { get; }
        public string CliName { get; }
        public string Description { get; }

    }

    public class OptionDescriptor : SymbolDescriptor
    {
        public OptionDescriptor(string originalName)
              : base(originalName)
        {
            OriginalName = originalName;
        }
        public string OriginalName { get; }
        public string CliName { get; }
        public string Description { get; }

    }

}
