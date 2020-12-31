using StarFruit.Common;
using System.Collections.Generic;

namespace StarFruit2.Common.Descriptors
{
    public class OptionDescriptor : IdentitySymbolDescriptor
    {
        public OptionDescriptor(ISymbolDescriptor parentSymbolDescriptorBase,
                                string originalName,
                                RawInfoBase rawInfo)
            : base(parentSymbolDescriptorBase, originalName, rawInfo, SymbolType.Option) { }
        public List<ArgumentDescriptor> Arguments { get; } = new List<ArgumentDescriptor>();
        public bool Required { get; set; }

        public override string ReportInternal(int tabsCount, VerbosityLevel verbosity)
        {
            string whitespace = CommonExtensions.NewLineWithTabs(tabsCount);
            return $"{whitespace}Required:{Required}";
        }

    }
}
