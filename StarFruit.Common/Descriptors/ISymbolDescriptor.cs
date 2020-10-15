using System.Collections.Generic;

namespace StarFruit2.Common.Descriptors
{
    public interface ISymbolDescriptor
    {
        SymbolType SymbolType { get; }
        object? Raw { get; }
        List<string> Aliases { get; }
        string? Description { get; }
        string? Name { get; }
        string? CliName { get; }
        string OriginalName { get; }
        bool IsHidden { get; set; }
        string Report(int tabsCount, VerbosityLevel verbosity);
    }
}