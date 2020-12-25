using StarFruit.Common;
using System.Collections.Generic;

namespace StarFruit2.Common.Descriptors
{
    public class OptionDescriptor : IdentitySymbolDescriptor
    {
        public OptionDescriptor(ISymbolDescriptor parentSymbolDescriptorBase, 
                                string originalName,
                                object? raw,
                                string originalElementType)
            : base(parentSymbolDescriptorBase, originalName, raw,originalElementType, SymbolType.Option) { }
        public List<ArgumentDescriptor> Arguments { get; } = new List<ArgumentDescriptor>();
        public bool Required { get; set; }

        public override string ReportInternal(int tabsCount, VerbosityLevel verbosity)
        {
            string whitespace = CommonExtensions.NewLineWithTabs(tabsCount);
            return $"{whitespace}Required:{Required}";
        }

    }
}
