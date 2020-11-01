using Microsoft.CodeAnalysis;
using StarFruit2;

namespace Starfruit2
{

    public class MethodSyntaxCommandMaker : DescriptorMaker
    {
        public MethodSyntaxCommandMaker(MakerConfiguration config, SemanticModel semanticModel)
          : base(config, semanticModel)
        { }

        ////protected override ArgumentDescriptor CreateArgumentDescriptor(ISymbolDescriptor parent,
        ////                                                               IParameterSymbol parameterSymbol)
        ////=> new ArgumentDescriptor(new ArgTypeInfoRoslyn(parameterSymbol.Type), parent, parameterSymbol.Name, parameterSymbol)
        ////{
        ////    Name = parameterSymbol.Name,
        ////    CliName = config.ArgumentNameToCliName(parameterSymbol.Name),
        ////    Description = config.GetDescription(parameterSymbol) ?? ""
        ////};

        //protected override OptionDescriptor CreateOptionDescriptor(ISymbolDescriptor parent,
        //                                                           IParameterSymbol parameterSymbol)
        //{
        //    var option = new OptionDescriptor(parent, parameterSymbol.Name, parameterSymbol)
        //    {
        //        Name = parameterSymbol.Name,
        //        CliName = config.OptionNameToCliName(parameterSymbol.Name),
        //        Description = config.GetDescription(parameterSymbol) ?? "",
        //        Required = config.GetIsRequired(parameterSymbol),
        //        IsHidden = config.GetIsHidden(parameterSymbol),
        //    };
        //    option.Aliases.AddRange(config.GetAliases(parameterSymbol));
        //    option.Arguments.Add(CreateOptionArgumentDescriptor(parent, parameterSymbol));
        //    return option;
        //}

        //private ArgumentDescriptor CreateOptionArgumentDescriptor(ISymbolDescriptor parent,
        //                                                          IParameterSymbol parameterSymbol)
        //=> new ArgumentDescriptor(new ArgTypeInfoRoslyn(parameterSymbol.Type),
        //                          parent,
        //                          parameterSymbol.Name,
        //                          parameterSymbol.Name)
        //{
        //    Name = parameterSymbol.Name,
        //    CliName = config.OptionArgumentNameToCliName(parameterSymbol.Name),
        //    Description = config.GetDescription(parameterSymbol) ?? ""
        //};

        //protected override IEnumerable<IParameterSymbol> GetMembers(IMethodSymbol parentSymbol)
        //=> parentSymbol.Parameters;

        //protected override IEnumerable<CommandDescriptor> GetSubCommands(ISymbolDescriptor parent, IMethodSymbol parentSymbol)
        //=> new List<CommandDescriptor> { };
    }
}
