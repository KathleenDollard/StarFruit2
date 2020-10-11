using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StarFruit.Common;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starfruit2_B
{
    public class MethodSyntaxCommandMaker : DescriptorMakerBase<IMethodSymbol, IParameterSymbol>
    {
        private readonly MakerConfigurationBase config;
        private readonly SemanticModel semanticModel;

        public MethodSyntaxCommandMaker(MakerConfigurationBase config, SemanticModel semanticModel)
          : base(config, semanticModel)
        { }

        protected override OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
                                                                   IParameterSymbol parameterSymbol)
        {
            var option = new OptionDescriptor(parent, parameterSymbol.Name, parameterSymbol)
            {
                Name = SourceToOptionName(parameterSymbol.Name)
            };
            option.Arguments.Add(CreateOptionArgumentDescriptor(parent, parameterSymbol));
            return option;
        }

        protected override ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
                                                                       IParameterSymbol parameterSymbol)
        => new ArgumentDescriptor(new ArgTypeInfoRoslyn(parameterSymbol.Type), parent, parameterSymbol.Name, parameterSymbol)
        {
            Name = SourceToArgumentName(parameterSymbol.Name)
        };

        private ArgumentDescriptor CreateOptionArgumentDescriptor(ISymbolDescriptor parent,
                                                                  IParameterSymbol parameterSymbol)
        => new ArgumentDescriptor(new ArgTypeInfoRoslyn(parameterSymbol.Type), parent, parameterSymbol.Name, parameterSymbol.Name)
        {
            Name = SourceToArgumentName(parameterSymbol.Name)
        };

        protected override IEnumerable<IParameterSymbol> GetMembers(IMethodSymbol parentSymbol)
        => parentSymbol.Parameters;

        protected override IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent, IMethodSymbol parentSymbol) 
        => new List<CommandDescriptor> { };
    }
}
