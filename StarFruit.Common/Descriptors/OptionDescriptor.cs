using StarFruit.Common;
using System.Collections.Generic;

namespace StarFruit2.Common.Descriptors
{
    public class OptionDescriptor : SymbolDescriptor
    {
        public OptionDescriptor(ISymbolDescriptor parentSymbolDescriptorBase, 
                                string originalName,
                                object? raw)
            : base(parentSymbolDescriptorBase, originalName, raw, SymbolType.Option) { }

        // I think I have to put a set on this, or put it into constructor
        public List<ArgumentDescriptor> Arguments { get; set; } = new List<ArgumentDescriptor>();
        public bool Required { get; set; }
        public string? Prefix { get; set; }

        public override string ReportInternal(int tabsCount, VerbosityLevel verbosity)
        {
            string whitespace = CommonExtensions.NewLineWithTabs(tabsCount);
            return $"{whitespace}Required:{Required}";
        }

    }
}
