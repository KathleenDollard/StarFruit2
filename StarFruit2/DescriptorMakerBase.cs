using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;

namespace StarFruit2
{


    public abstract class DescriptorMakerBase<TConfigSource, TSource>
    {

        public abstract CommandDescriptor GetCommandDescriptor<T>(T commandSource);
        public abstract OptionDescriptor GetOptionDescriptor<T>(ISymbolDescriptor? parent, T optionSource);
        public abstract ArgumentDescriptor GetArgumentDescriptor<T>(ISymbolDescriptor? parent, T argumentSource);
        public abstract IEnumerable<CommandDescriptor> GetSubCommandSources<T>(ISymbolDescriptor? parent, T commandSource);
        public abstract bool IsArgument<T>(T argumentSource);
        public abstract bool IsOption<T>(T argumentSource);

        public abstract string SourceToOptionName(string sourceName);
        public abstract string SourceToArgumentName(string sourceName);
        public abstract string SourceToCommandName(string sourceName);
    }

    //public abstract class CommandDescriptorMaker<TCommandSource, TOptionArgSource, TSubCommandSource>
    //{
    //    public abstract CommandDescriptor GetCommandDescriptor(TCommandSource commandSource);
    //    public abstract OptionDescriptor GetOptionDescriptor(ISymbolDescriptor? parent, TOptionArgSource optionSource);
    //    public abstract ArgumentDescriptor GetArgumentDescriptor(ISymbolDescriptor? parent, TOptionArgSource argumentSource);
    //    public abstract IEnumerable<CommandDescriptor> GetSubCommandSources(ISymbolDescriptor? parent, TSubCommandSource commandSource);
    //    public abstract bool IsArgument(TOptionArgSource argumentSource);
    //    public abstract bool IsOption(TOptionArgSource argumentSource);
    //}

    //public static class CommandDescriptorMakerFactory
    //{
    //    public CommandDe
    //}
}
