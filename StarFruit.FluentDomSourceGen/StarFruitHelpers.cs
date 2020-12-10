﻿using FluentDom;
using StarFruit2.Common.Descriptors;
using System.Linq;

namespace StarFruit2.Generate
{
    public static class Helpers
    {

        public static string GetOptionPropertyName(string name)
          => $"{name}Option";
        public static string GetArgumentPropertyName(string name)
           => $"{name}Argument";
        public static string GetCommandPropertyName(string name)
           => $"{name}Command";
        public static string CommandSourceClassName(this CommandDescriptor cmd) 
            => $"{cmd.OriginalName}CommandSource";


        public static TypeRep GetArgumentType(this OptionDescriptor o)
        => o.Arguments.Any()
            ? GetArgumentType(o.Arguments.First())
            : new TypeRep("bool");

        // TODO: Arguments do not currently support generic types
        public static TypeRep GetArgumentType(this ArgumentDescriptor o)
          => new TypeRep(o.ArgumentType.TypeAsString());

        public static Property NewProperty(this OptionDescriptor option)
        => new Property(GetOptionPropertyName(option.Name), new TypeRep("Option", option.GetArgumentType()));

        public static Property NewProperty(this ArgumentDescriptor argument)
        => new Property(GetArgumentPropertyName(argument.Name), new TypeRep("Argument", argument.GetArgumentType()));

        public static TypeRep GetFluentArgumentType(this OptionDescriptor o)
        => o.Arguments.Any()
            ? GetFluentArgumentType(o.Arguments.First())
            : new TypeRep("bool");

        // TODO: Arguments do not currently support generic types
        public static TypeRep GetFluentArgumentType(this ArgumentDescriptor o)
          => new TypeRep(o.ArgumentType.TypeAsString());
    }
}
