using System.CommandLine.Parsing;
using StarFruit2.Common.Descriptors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using StarFruit2;
using StarFruit2.Common;
using System.Diagnostics;

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
    // * SubCommands                   Will do - done and tested for methods, not tested for derived classes, but probably done

    // * Duplicate testing for methods/parameters  DONE

    // Add ordering for params
    // Mark async
    // For options/args mark as parameter, ctor param or property

    // * Explore putting unique name for fields into descripriptor

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

        protected virtual IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent,
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
            var command = new CommandDescriptor(parent, symbol.Name, symbol)
            {
                Name = config.CommandNameToName(symbol.Name),
                CliName = config.CommandNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                IsHidden = config.GetIsHidden(symbol),
                TreatUnmatchedTokensAsErrors = config.GetTreatUnmatchedTokensAsErrors(symbol),
            };
            command.Aliases.AddRange(config.GetAliases(symbol));
            command.AddArguments(GetArguments(command, symbol));
            command.AddOptions(GetOptions(command, symbol));
            if (symbol is INamedTypeSymbol s)
            {
                IEnumerable<CommandDescriptor> subCommands = GetSubCommands(command, s);
                command.AddCommands(subCommands);
            }
            return command;
        }

        protected virtual ArgumentDescriptor CreateArgumentDescriptor<TMemberSymbol>(ISymbolDescriptor parent,
                                                                      TMemberSymbol symbol)
        where TMemberSymbol : class, ISymbol
            // ** How to find syntax: var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.Single().GetSyntax() as PropertyDeclarationSyntax;
        {
            var argType = config.GetArgTypeInfo(symbol);
            Assert.NotNull(argType, "argType");
            var arg = new ArgumentDescriptor(argType, parent, symbol.Name, symbol)
            {
                Name = config.ArgumentNameToName(symbol.Name),
                CliName = config.ArgumentNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                Required = config.GetIsRequired(symbol),
                IsHidden = config.GetIsHidden(symbol),
                DefaultValue = config.GetDefaultValue(symbol),
            };
            arg.AllowedValues.AddRange(config.GetAllowedValues(symbol));
            return arg;
        }


        protected virtual OptionDescriptor CreateOptionDescriptor<TMemberSymbol>(ISymbolDescriptor parent,
                                                                   TMemberSymbol symbol)
        where TMemberSymbol : class, ISymbol
        {
            var option = new OptionDescriptor(parent, symbol.Name, symbol)
            {
                Name = config.OptionNameToName(symbol.Name),
                CliName = config.OptionNameToCliName(symbol.Name),
                Description = config.GetDescription(symbol) ?? "",
                Required = config.GetIsRequired(symbol),
                IsHidden = config.GetIsHidden(symbol),
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
                                             symbol)
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

        protected IEnumerable<OptionDescriptor> GetOptions<TCommandSymbol>(ISymbolDescriptor parent,
                                                           TCommandSymbol parentSymbol)
           where TCommandSymbol : class, ISymbol
            => parentSymbol switch
            {
                INamedTypeSymbol s => s.GetMembers().OfType<IPropertySymbol>()
                                         .Where(p => config.IsOption(p.Name, p))
                                         .Select(p => CreateOptionDescriptor(parent, p)),
                IMethodSymbol s => s.Parameters.OfType<IParameterSymbol>()
                                         .Where(p => config.IsOption(p.Name, p))
                                         .Select(p => CreateOptionDescriptor(parent, p)),
                _ => throw new NotImplementedException()
            };

        protected IEnumerable<ArgumentDescriptor> GetArguments<TCommandSymbol>(ISymbolDescriptor parent,
                                                               TCommandSymbol parentSymbol)
          where TCommandSymbol : class, ISymbol
          => parentSymbol switch
          {
              INamedTypeSymbol s => s.GetMembers().OfType<IPropertySymbol>()
                                       .Where(p => config.IsArgument(p.Name, p))
                                       .Select(p => CreateArgumentDescriptor(parent, p)),
              IMethodSymbol s => s.Parameters.OfType<IParameterSymbol>()
                                       .Where(p => config.IsArgument(p.Name, p))
                                       .Select(p => CreateArgumentDescriptor(parent, p)),
              _ => throw new NotImplementedException()
          }; 
        
    }
}
