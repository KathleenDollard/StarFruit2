using FluentDom;
using StarFruit2.Common.Descriptors;
using System;
using System.Linq;

namespace StarFruit2.Generate
{
    public static class Helpers
    {
        public static string GetPropertyName(this ISymbolDescriptor symbol)
         => symbol switch
         {
             OptionDescriptor option => $"{option.OriginalName}Option",
             ArgumentDescriptor argument => $"{argument.OriginalName}Argument",
             CommandDescriptor command => $"{command.OriginalName}Command",
             _ => throw new InvalidOperationException("Unexpected symbol type")
         };


        public static string GetPropertyResultName(this ISymbolDescriptor symbol)
         => symbol switch
         {
             OptionDescriptor option => $"{option.GetPropertyName()}_Result",
             ArgumentDescriptor argument => $"{argument.GetPropertyName()}_Result",
             CommandDescriptor command => $"{command.GetPropertyName()}_Result",
             _ => throw new InvalidOperationException("Unexpected symbol type")
         };

        public static string GetParameterResultName(this ISymbolDescriptor symbol)
           => symbol switch
           {
               OptionDescriptor option => $"{option.GetPropertyName().CamelCase()}_result",
               ArgumentDescriptor argument => $"{argument.GetPropertyName().CamelCase()}_result",
               CommandDescriptor command => $"{command.GetPropertyName().CamelCase()}_result",
               _ => throw new InvalidOperationException("Unexpected symbol type")
           };

        public static string CommandSourceClassName(this CommandDescriptor cmd) 
            => $"{cmd.OriginalName}CommandSource";
        public static string CommandSourceResultClassName(this CommandDescriptor cmd)
            => $"{cmd.OriginalName}CommandSourceResult";

        public static TypeRep GetArgumentType(this OptionDescriptor o)
        => o.Arguments.Any()
            ? GetArgumentType(o.Arguments.First())
            : new TypeRep("bool");

        // TODO: Arguments do not currently support generic types
        public static TypeRep GetArgumentType(this ArgumentDescriptor o)
          => new TypeRep(o.ArgumentType.TypeAsString());

        public static TypeRep GetFluentArgumentType(this OptionDescriptor o)
        => o.Arguments.Any()
            ? GetFluentArgumentType(o.Arguments.First())
            : new TypeRep("bool");

        // TODO: Arguments do not currently support generic types
        public static TypeRep GetFluentArgumentType(this ArgumentDescriptor o)
          => new TypeRep(o.ArgumentType.TypeAsString());

        public static TypeRep SymbolType(this ISymbolDescriptor symbolDescriptor)
        {
            return symbolDescriptor switch
            {
                ArgumentDescriptor a => ArgumentType(a),
                OptionDescriptor o => OptionType(o),
                _ => throw new InvalidOperationException("Unknown symbol type")
            };
        }

        public static TypeRep ArgumentType(this ArgumentDescriptor argumentDescriptor)
        {
            return new TypeRep("Argument", argumentDescriptor.GetFluentArgumentType());
        }

        public static TypeRep OptionType(this OptionDescriptor optionDescriptor)
        {
            return new TypeRep("Option", optionDescriptor.GetFluentArgumentType());
        }
    }
}
