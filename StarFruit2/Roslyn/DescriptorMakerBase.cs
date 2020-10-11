using System.CommandLine.Parsing;
using StarFruit2.Common.Descriptors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using StarFruit2;
using StarFruit2.Common;

namespace Starfruit2_B
{

    public class DescriptorMakerBase
    {
        protected readonly MakerConfigurationBase config;
        protected readonly SemanticModel semanticModel;

        public DescriptorMakerBase(MakerConfigurationBase config, SemanticModel semanticModel)
        {
            this.config = config;
            this.semanticModel = semanticModel;
        }

        protected internal string SourceToOptionName(string sourceName)
        => $"--{sourceName.ToKebabCase()}";

        protected internal string SourceToArgumentName(string sourceName)
        {
            if (sourceName.EndsWith("Arg"))
            {
                sourceName = sourceName[..^3];
            }
            return sourceName.ToKebabCase();
        }

        protected internal string SourceToCommandName(string sourceName)
            => sourceName.ToKebabCase();

    }

    public abstract class DescriptorMakerBase<TCommandSymbol, TMemberSymbol> : DescriptorMakerBase
        where TCommandSymbol : class, ISymbol
        where TMemberSymbol : class, ISymbol
    {
        public DescriptorMakerBase(MakerConfigurationBase config, SemanticModel semanticModel)
           : base(config, semanticModel)
        { }

        protected abstract OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
                                                                   TMemberSymbol symbol);
        protected abstract ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
                                                                       TMemberSymbol symbol);
        protected abstract IEnumerable<TMemberSymbol> GetMembers(TCommandSymbol parentSymbol);

        protected abstract IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent,
                                                                         TCommandSymbol parentSymbol);

        protected internal virtual CliDescriptor CreateCliDescriptor(ISymbolDescriptor? parent,
                                                                     TCommandSymbol symbol)
        {
            var cliDesriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = symbol.ContainingNamespace.ToString(),
                CommandDescriptor = CreateCommandDescriptor(parent, symbol)
            };
            return cliDesriptor;
        }
        protected CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                            TCommandSymbol symbol)
        {
            Assert.NotNull(symbol);
            var command = new CommandDescriptor(parent, symbol.Name, symbol)
            {
                Name = SourceToCommandName(symbol.Name),
            };
            command.AddArguments(GetArguments(command, symbol));
            command.AddOptions(GetOptions(command, symbol));
            command.AddCommands(GetSubCommands(command, symbol));
            return command;
        }

        protected IEnumerable<OptionDescriptor> GetOptions(ISymbolDescriptor parent,
                                                           TCommandSymbol parentSymbol)
        => GetMembers(parentSymbol).OfType<TMemberSymbol>()
                  .Where(p => config.IsOption(p.Name, p.GetAttributes()))
                  .Select(p => CreateOptionDescriptor(parent, p));

        protected IEnumerable<ArgumentDescriptor> GetArguments(ISymbolDescriptor parent,
                                                               TCommandSymbol parentSymbol)
         => GetMembers(parentSymbol).OfType<TMemberSymbol>()
                   .Where(p => config.IsArgument(p.Name, p.GetAttributes()))
                   .Select(p => CreateArgumentDescriptor(parent, p));
    }
}
