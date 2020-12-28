using StarFruit.Common;
using System.Collections.Generic;

namespace StarFruit2.Common.Descriptors
{
    public class ArgumentDescriptor : SymbolDescriptor
    {
        public ArgumentDescriptor(ArgTypeInfoBase argumentTypeInfo, 
                                  ISymbolDescriptor parentSymbolDescriptorBase,
                                  string originalName,
                                  RawInfoBase rawInfo)
            : base(parentSymbolDescriptorBase, originalName, rawInfo, SymbolType.Argument)
        {
            ArgumentType = argumentTypeInfo;
        }

        public ArityDescriptor? Arity { get; set; }
        public List<object> AllowedValues { get; } = new List<object>();
        public ArgTypeInfoBase ArgumentType { get; }
        public DefaultValueDescriptor? DefaultValue { get; set; }
        public bool Required { get; set; }

        public override string ReportInternal(int tabsCount, VerbosityLevel verbosity)
        {
            string whitespace = CommonExtensions.NewLineWithTabs(tabsCount);
            return $"{whitespace}Arity:{Arity}" +
                   $"{whitespace}AllowedValues:{Name}" +
                   $"{whitespace}ArgumentType:{ArgumentType}" +
                   $"{whitespace}DefaultValue:{DefaultValue}" +
                   $"{whitespace}Required:{Required}";
        }
    }
}
