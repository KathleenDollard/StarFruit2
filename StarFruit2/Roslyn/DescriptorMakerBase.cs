using System.CommandLine.Parsing;
using StarFruit2.Common.Descriptors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using StarFruit2;
using StarFruit2.Common;

namespace Starfruit2
{
    // Implementation notes:
    // Arguments:
    // * Name/CliName/OriginalName:    DONE
    // * Description:                  DONE
    // * IsHidden:                     DONE
    // * ArgumentType:                 DONE
    // * Arity:                        Will only support as descriptive scenarios when understood. May need backdoor for cases not generally supported like this.
    // * AllowedValues:                DONE
    // * DefaultValue:                 DONE
    // * Required:                     DONE

    // Options:                      
    // * Name/CliName/OriginalName:    DONE
    // * Description:                  DONE
    // * IsHidden:                     DONE
    // * Aliases:                      DONE
    // * ArgumentType:                 DONE
    // * Arity:                        Will only support as descriptive scenarios when understood. May need backdoor for cases not generally supported like this.
    // * AllowedValues:                DONE
    // * DefaultValue:                 DONE
    // * Required:                     DONE

    // Comamnds                      
    // * Name/CliName/OriginalName:    DONE
    // * Description:                  DONE
    // * IsHidden:                     DONE
    // * Aliases:                      DONE
    // * TreatUnmatchedTokensAsErrors  DONE

    // Explore putting unique name for fields into descripriptor

    public class DescriptorMakerBase
    {
        protected readonly MakerConfiguration config;
        protected readonly SemanticModel semanticModel;

        public DescriptorMakerBase(MakerConfiguration config, SemanticModel semanticModel)
        {
            this.config = config ?? new MakerConfiguration(new CSharpLanguageHelper());
            this.semanticModel = semanticModel;
        }
    }

    public abstract class DescriptorMakerBase<TCommandSymbol, TMemberSymbol> : DescriptorMakerBase
        where TCommandSymbol : class, ISymbol
        where TMemberSymbol : class, ISymbol
    {
        public DescriptorMakerBase(MakerConfiguration config, SemanticModel semanticModel)
           : base(config, semanticModel)
        { }

        protected abstract OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
                                                                   TMemberSymbol symbol);
        protected abstract ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
                                                                       TMemberSymbol symbol);
        protected abstract IEnumerable<TMemberSymbol> GetMembers(TCommandSymbol parentSymbol);

        protected abstract IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent,
                                                                         TCommandSymbol parentSymbol);

        protected virtual CommandDescriptor CreateCommandDescriptor(ISymbolDescriptor? parent,
                                                                    TCommandSymbol symbol)
        {
            Assert.NotNull(symbol);
            var command = new CommandDescriptor(parent, symbol.Name, symbol)
            {
                Name = config.CommandNameToName(symbol.Name),
                CliName = config.CommandNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                IsHidden = config.GetIsHidden(symbol),
                TreatUnmatchedTokensAsErrors=config.GetTreatUnmatchedTokensAsErrors(symbol),
            };
            command.Aliases.AddRange(config.GetAliases(symbol));
            command.AddArguments(GetArguments(command, symbol));
            command.AddOptions(GetOptions(command, symbol));
            command.AddCommands(GetSubCommands(command, symbol));
            return command;
        }

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

        protected IEnumerable<OptionDescriptor> GetOptions(ISymbolDescriptor parent,
                                                           TCommandSymbol parentSymbol)
            => GetMembers(parentSymbol).OfType<TMemberSymbol>()
                                       .Where(p => config.IsOption(p.Name, p))
                                       .Select(p => CreateOptionDescriptor(parent, p));

        protected IEnumerable<ArgumentDescriptor> GetArguments(ISymbolDescriptor parent,
                                                               TCommandSymbol parentSymbol)
            => GetMembers(parentSymbol).OfType<TMemberSymbol>()
                                       .Where(p => config.IsArgument(p.Name, p))
                                       .Select(p => CreateArgumentDescriptor(parent, p));
    }
}
