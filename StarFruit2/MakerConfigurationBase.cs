using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Text;

namespace StarFruit2
{
   public class MakerConfigurationBase
    {
        public virtual bool IsOption<T>(string name, T source) => !IsArgument(name, source);
        public virtual bool IsArgument<T>(string name, T source)
        {
            if (name.EndsWith("Arg"))
            {
                return true;
            }
            return source switch
            {
                IPropertySymbol p => p.HasAttribute<ArgumentAttribute>(),
                IParameterSymbol p => p.HasAttribute<ArgumentAttribute>(),
                PropertyDeclarationSyntax p => p.HasAttribute<ArgumentAttribute>(),
                ParameterSyntax p => p.HasAttribute<ArgumentAttribute>()
            };


        }

        public virtual string OptionNameMungeToCli(string name)
        {
            name = name.EndsWith("Option")
                   ? name = name[^6..]
                   : name;
            return name.ToKebabCase().ToLowerInvariant();
        }

        public virtual string ArgumentNameMungeToCli(string name)
        {
            name = name.EndsWith("Arg")
                   ? name = name[^3..]
                   : name;
            return name.ToKebabCase().ToLowerInvariant();
        }

        public virtual string CommandNameMungeToCli(string name)
        {
            name = name.EndsWith("Command")
                   ? name = name[^7..]
                   : name;
            return name.ToKebabCase().ToLowerInvariant();
        }

        public virtual bool DescriptionFromXmlComments() => true;

    }
}
