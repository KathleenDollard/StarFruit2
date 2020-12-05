using StarFruit.Common;
using StarFruit.Common.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2.Common.Descriptors
{
    public class CommandDescriptor : IdentitySymbolDescriptor
    {
        public CommandDescriptor(ISymbolDescriptor? parentSymbolDescriptorBase,
                                 string originalName,
                                 object? raw)
            : base(parentSymbolDescriptorBase, originalName, raw, SymbolType.Command) 
        {        }

        public CommandDescriptorSource DescriptorSource { get; set; }
        public bool TreatUnmatchedTokensAsErrors { get; set; } = true;
        public List<ArgumentDescriptor> Arguments { get; } = new List<ArgumentDescriptor>();
        public List<OptionDescriptor> Options { get; } = new List<OptionDescriptor>();
        public InvokeMethodInfo? InvokeMethod { get; set; } // in Reflection models, this is a MethodInfo, in Roslyn it will be something else
        public List<CommandDescriptor> SubCommands { get; } = new List<CommandDescriptor>();
        public bool IsAsync { get; set; }
        public object baseClass { get; set; }

        public IEnumerable<SymbolDescriptor> GetChildren()
            => Arguments.OfType<SymbolDescriptor>().Union(Options).Union(SubCommands);

        public IEnumerable<SymbolDescriptor> GetOptionsAndArgs()
            => Arguments.OfType<SymbolDescriptor>().Union(Options);


        public override string ReportInternal(int tabsCount, VerbosityLevel verbosity)
        {
            string whitespace = CommonExtensions.NewLineWithTabs(tabsCount);
            return $"{whitespace}TreatUnmatchedTokensAsErrors:{TreatUnmatchedTokensAsErrors}" +
                   $"{whitespace}SubCommands:{string.Join("", SubCommands.Select(x => x.Report(tabsCount + 1, verbosity)))}" +
                   $"{whitespace}Options:{string.Join("", Options.Select(x => x.Report(tabsCount + 1, verbosity)))}" +
                   $"{whitespace}Arguments:{string.Join("", Arguments.Select(x => x.Report(tabsCount + 1, verbosity)))}";
        }
    }
}
