using FluentDom;
using Microsoft.CodeAnalysis;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Linq;

namespace StarFruit2.Generate
{
    public static class Helpers
    {
        public static string PropertyName(this ISymbolDescriptor symbol)
         => symbol switch
         {
             OptionDescriptor option => $"{option.OriginalName}Option",
             ArgumentDescriptor argument => $"{argument.OriginalName}Argument",
             CommandDescriptor command => $"{command.OriginalName}Command",
             _ => throw new InvalidOperationException("Unexpected symbol type")
         };

        public static string ParameterName(this ISymbolDescriptor symbol)
         => symbol.PropertyName().AsParameter();

        public static string PropertyResultName(this ISymbolDescriptor symbol)
            => $"{symbol.PropertyName()}_Result";

        public static string ParameterResultName(this ISymbolDescriptor symbol)
            => symbol.PropertyResultName().AsParameter();

        public static string AsParameter(this string name)
            => name.CamelCase();

        public static string CommandSourceClassName(this CommandDescriptor cmd)
            => $"{cmd?.OriginalName}CommandSource";
        public static string CommandSourceResultClassName(this CommandDescriptor cmd)
            => $"{cmd.OriginalName}CommandSourceResult";

        public static TypeRep GetFluentArgumentType(this OptionDescriptor o)
        => o.Arguments.Any()
            ? GetFluentArgumentType(o.Arguments.First())
            : new TypeRep("bool");

        // TODO: Arguments do not currently support generic types
        public static TypeRep GetFluentArgumentType(this ArgumentDescriptor o)
          => o.ArgumentType.TypeRepFromArgumentType()
             ?? new TypeRep("bool");

        public static TypeRep? TypeRepFromArgumentType(this ArgTypeInfoBase argType)
        {
            if (argType.TypeRepresentation is null) return null;
            return TypeRepFromNonNullObject(argType.TypeRepresentation);

            static TypeRep TypeRepFromNamedType(INamedTypeSymbol t)
            {
                var typeArgs = t.TypeArguments.Select(a => TypeRepFromNonNullObject(a));
                var typeRep = typeArgs.Count() switch
                {
                    0=> new TypeRep(t.Name),
                    _=> new TypeRep(t.Name, t.TypeArguments.Select(TypeRepFromNonNullObject).ToArray()),
                };
                return typeRep;     
            }

            static TypeRep TypeRepFromNonNullObject(object? typeAsObject)
            {
                return typeAsObject switch
                {
                    INamedTypeSymbol t => TypeRepFromNamedType(t),
                    Type t => new TypeRep(t.Name), // currently just used for tests, so generics ignored
                    _ => throw new NotImplementedException($"Not implemented { nameof(typeAsObject) } in { nameof(Helpers)}.{ nameof(TypeRepFromNonNullObject)}")
                };
            }
        }



        public static TypeRep SymbolType(this ISymbolDescriptor symbolDescriptor)
        {
            return symbolDescriptor switch
            {
                ArgumentDescriptor a => ArgumentType(a),
                OptionDescriptor o => OptionType(o),
                _ => throw new InvalidOperationException($"Not implemented { nameof(symbolDescriptor) } in { nameof(Helpers)}.{ nameof(SymbolType)}")
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
