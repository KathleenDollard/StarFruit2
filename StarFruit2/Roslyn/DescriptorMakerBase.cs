using StarFruit2.Common.Descriptors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using StarFruit2;
using StarFruit2.Common;
using StarFruit.Common;

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
    // * Name/CliName/OriginalName:         DONE
    // * Description:                       DONE
    // * IsHidden:                          DONE
    // * Aliases:                           DONE
    // * ArgumentType:                      DONE
    // * Arity:                             Will only support as descriptive scenarios when understood. May need backdoor for cases not generally supported like this.
    // * AllowedValues:                     DONE
    // * DefaultValue:                      DONE
    // * Required:                          DONE

    // Comamnds                           
    // * Name/CliName/OriginalName:         DONE
    // * Description:                       DONE
    // * IsHidden:                          DONE
    // * Aliases:                           DONE
    // * TreatUnmatchedTokensAsErrors       DONE
    // * SubCommands                        Will do - done and tested for methods, not tested for derived classes, but probably done

    // * Test methods/parameters            DONE

    // Add ordering for params              DONE
    // Mark async                           DONE
    // Code element (prop/parm)             DONE
    // Constructors                         ToDo (late stage validation not possible)
    // Validation                           ToDo
    // ExtraData/ExtraAttributes            ToDo

    // Unique fieldname in descripriptor    Probably to do

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

    public abstract class DescriptorMaker : DescriptorMakerBase
    //where TCommandSymbol : class, ISymbol
    //where TMemberSymbol : class, ISymbol
    {
        public DescriptorMaker(MakerConfiguration config, SemanticModel semanticModel)
           : base(config, semanticModel)
        { }

        protected virtual IEnumerable<CommandDescriptor> GetSubCommands(CommandDescriptor parent,
                                                                        INamedTypeSymbol parentSymbol)
        {
            IEnumerable<ISymbol> members = config.GetSubCommandMembers(parentSymbol);
            return members.Select(s => s switch
                {
                    IMethodSymbol x => CreateCommandDescriptor(parent, x),
                    INamedTypeSymbol x => CreateCommandDescriptor(parent, x),
                    _ => throw new NotImplementedException()
                });
        }

        protected virtual CommandDescriptor CreateCommandDescriptor<TCommandSymbol>(ISymbolDescriptor? parent,
                                                                                    TCommandSymbol symbol)
            where TCommandSymbol : class, ISymbol
        {
            Assert.NotNull(symbol);
            var command = new CommandDescriptor(parent, symbol.Name, symbol, symbol.OriginalElementTypeFromSymbol(parent))
            {
                Name = config.CommandNameToName(symbol.Name),
                CliName = config.CommandNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                IsHidden = config.GetIsHidden(symbol),
                TreatUnmatchedTokensAsErrors = config.GetTreatUnmatchedTokensAsErrors(symbol),
                IsAsync = config.GetAsync(symbol)
            };
            command.Aliases.AddRange(config.GetAliases(symbol));
            var itemCandidates = GetItemCandidates(symbol);
            command.AddArguments(GetArguments(command, itemCandidates));
            command.AddOptions(GetOptions(command, itemCandidates));
            if (symbol is INamedTypeSymbol s)
            {
                IEnumerable<CommandDescriptor> subCommands = GetSubCommands(command, s);
                command.AddCommands(subCommands);
            }
            return command;
        }

        protected virtual ArgumentDescriptor CreateArgumentDescriptor<TMemberSymbol>(ISymbolDescriptor parent,
                                                                                     TMemberSymbol symbol,
                                           string originalElementType,
                                                                                     int position)
        where TMemberSymbol : class, ISymbol
            // ** How to find syntax: var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
        {
            var argType = config.GetArgTypeInfo(symbol);
            Assert.NotNull(argType, "argType");
            var arg = new ArgumentDescriptor(argType,
                                             parent,
                                             symbol.Name,
                                             symbol,
                                             symbol.OriginalElementTypeFromSymbol(parent))
            {
                Name = config.ArgumentNameToName(symbol.Name),
                CliName = config.ArgumentNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                Required = config.GetIsRequired(symbol),
                IsHidden = config.GetIsHidden(symbol),
                DefaultValue = config.GetDefaultValue(symbol),
                Position = position,
            };
            arg.AllowedValues.AddRange(config.GetAllowedValues(symbol));
            return arg;
        }


        protected virtual OptionDescriptor CreateOptionDescriptor<TMemberSymbol>(ISymbolDescriptor parent,
                                                                                 TMemberSymbol symbol,
                                                                                 string originalElementType,
                                                                                 int position)
            where TMemberSymbol : class, ISymbol
        {
            var option = new OptionDescriptor(parent,
                                              symbol.Name,
                                              symbol,
                                              originalElementType)
            {
                Name = config.OptionNameToName(symbol.Name),
                CliName = config.OptionNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                Required = config.GetIsRequired(symbol),
                IsHidden = config.GetIsHidden(symbol),
                Position = position,
            };

            option.Aliases.AddRange(config.GetAliases(symbol));
            option.Arguments.Add(CreateOptionArgumentDescriptor(option, symbol));
            return option;
        }

        protected virtual ArgumentDescriptor CreateOptionArgumentDescriptor<TMemberSymbol>(ISymbolDescriptor parent,
                                                                            TMemberSymbol symbol)
        where TMemberSymbol : class, ISymbol
        {
            var argType = config.GetArgTypeInfo(symbol)
                            ?? new ArgTypeInfoRoslyn(typeof(bool));
            var arg = new ArgumentDescriptor(argType,
                                             parent,
                                             symbol.Name,
                                             symbol,
                                             parent.OriginalElementType)
            {
                Name = symbol.Name,
                CliName = config.OptionArgumentNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                DefaultValue = config.GetDefaultValue(symbol),
                Required = config.GetIsRequired(symbol),
                IsHidden = config.GetIsHidden(symbol),
            };
            arg.AllowedValues.AddRange(config.GetAllowedValues(symbol));
            return arg;
        }

        protected internal virtual CliDescriptor CreateCliDescriptor<TCommandSymbol>(ISymbolDescriptor? parent,
                                                                     TCommandSymbol symbol)
        where TCommandSymbol : class, ISymbol
        {
            var cliDesriptor = new CliDescriptor
            {
                GeneratedComandSourceNamespace = symbol.ContainingNamespace.ToString(),
                CommandDescriptor = CreateCommandDescriptor(parent, symbol)
            };
            return cliDesriptor;
        }

        private IEnumerable<(ISymbol Symbol, int Position, string OriginalElementType)> GetItemCandidates
                <TCommandSymbol>(TCommandSymbol parentSymbol)
            where TCommandSymbol : class, ISymbol
        {
            return parentSymbol switch
            {
                INamedTypeSymbol s => GetPropertyCaandidates(s).Concat(GetCtorParameterCandidates(s)),
                IMethodSymbol s => s.Parameters.Select((symbol, position)
                                         => GetTuple((ISymbol)symbol, position, OriginalElementType.MethodParameter)),
                _ => throw new NotImplementedException()
            };

            static IEnumerable<(ISymbol Symbol, int Position, string OriginalElementType)> GetPropertyCaandidates(INamedTypeSymbol parentSymbol)
                => parentSymbol.GetMembers()
                        .Where(s => s.Kind == SymbolKind.Property)
                        .Select((Symbol, Position) => GetTuple((ISymbol)Symbol, Position, OriginalElementType.Property));

            static IEnumerable<(ISymbol Symbol, int Position, string OriginalElementType)> GetCtorParameterCandidates(INamedTypeSymbol parent)
                // KAD: Options need to be duplicated so the raw values are correct later. This is intended
                // and creation of System.CommandLine options may need to remove dupes and result handling
                // can use Raw to recreate constructors. Yes, this is messy.
                => parent.Constructors.SelectMany(c => c.Parameters)
                                      .Select((symbol, position) => ((ISymbol)symbol, position, OriginalElementType.CtorParameter));

            static (ISymbol Symbol, int Position, string OriginalElementType) GetTuple(ISymbol symbol, int position, string originalElementType)
                  => (symbol, position, originalElementType);
        }

        protected IEnumerable<OptionDescriptor> GetOptions(ISymbolDescriptor parent,
                                                           IEnumerable<(ISymbol Symbol, int Position, string OriginalElementType)> candidates)
            => candidates.Where(t => config.IsOption(t.Symbol.Name, t.Symbol))
                         .Select(t => CreateOptionDescriptor(parent,
                                                             t.Symbol,
                                                             t.OriginalElementType,
                                                             t.Position));

        protected IEnumerable<ArgumentDescriptor> GetArguments(ISymbolDescriptor parent,
                                                               IEnumerable<(ISymbol Symbol, int Position, string OriginalElementType)> candidates)
            => candidates.Where(t => config.IsArgument(t.Symbol.Name, t.Symbol))
                         .Select(t => CreateArgumentDescriptor(parent,
                                                               t.Symbol,
                                                               t.OriginalElementType,
                                                               t.Position));

    }
}
