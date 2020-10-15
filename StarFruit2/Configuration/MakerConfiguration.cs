using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using StarFruit2.Common;
using System.Linq;

namespace StarFruit2
{
    public class MakerConfiguration
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
                _ => throw new InvalidOperationException("Symbol type not recognized")
            };
        }

        public virtual string ArgumentNameToCliName(string name)
        {
            name = name.EndsWith("Arg")
                   ? name = name[^3..]
                   : name;
            return name.ToKebabCase().ToLowerInvariant();
        }

        public virtual string OptionNameToCliName(string name)
        {
            name = name.EndsWith("Option")
                   ? name = name[^6..]
                   : name;
            return $"--{name.ToKebabCase().ToLowerInvariant()}";
        }

        internal bool GetIsHidden(IPropertySymbol propertySymbol)
        {
            throw new NotImplementedException();
        }

        internal bool GetIsRequired(IPropertySymbol propertySymbol)
        {
            throw new NotImplementedException();
        }

        public virtual string OptionArgumentNameToCliName(string name)
        {
            name = name.EndsWith("Option")
                   ? name = name[^6..]
                   : name;
            return $"--{name.ToKebabCase().ToLowerInvariant()}";
        }

        internal IEnumerable<string> GetAliases(IParameterSymbol parameterSymbol)
        {
            throw new NotImplementedException();
        }

        public virtual string CommandNameToCliName(string name)
        {
            name = name.EndsWith("Command")
                   ? name = name[^7..]
                   : name;
            return name.ToKebabCase().ToLowerInvariant();
        }

        private List<IDescriptionProvider> AdditionalDescriptionSources = new List<IDescriptionProvider>();

        public void AddDescriptionProvider(IDescriptionProvider descriptionProvider )
        {
            AdditionalDescriptionSources.Add(descriptionProvider);
        }

        public bool UseXmlCommentsForDescription { get; set; }

        public string? GetDescription<T>(T source)
        {
            var descriptionSources = new List<IDescriptionProvider>();
            if (UseXmlCommentsForDescription)
            {
                descriptionSources.Add(DescriptionFromXmlComments.Provider);
            }
            // TODO: Add other sources
            descriptionSources.Union(descriptionSources);

            foreach (var provider in descriptionSources)
            {
                var desc = provider.GetDescription(source);
                if(!string.IsNullOrWhiteSpace(desc))
                {
                    return desc;
                }
            }
            return null;
        }
    }
}
