using StarFruit.Common;

namespace StarFruit2.Common.Descriptors
{
    public interface ISymbolDescriptor
    {
        SymbolType SymbolType { get; }
        public RawInfoBase RawInfo { get; }
        string? Description { get; }
        string? Name { get; }
        string? CliName { get; }
        string OriginalName { get; }
        bool IsHidden { get; set; }
        ISymbolDescriptor? ParentSymbolDescriptorBase { get; }

        string Report(int tabsCount, VerbosityLevel verbosity);
    }
}