using System;

namespace StarFruit2.Common
{
    [Flags]
    public enum SymbolType
    {
        Unknown = 0,
        Command = 0b0001,
        Option = 0b0010,
        Argument = 0b0100,
        All = 0b0111
    }
}
