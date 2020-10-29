using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using StarFruit2.Common;
using System.Linq;
using StarFruit2.Common.Descriptors;

namespace StarFruit2
{
    public class MakerConfiguration
    {
        private readonly LanguageHelper languageHelper;

        public MakerConfiguration(LanguageHelper languageHelper)
        {
            this.languageHelper = languageHelper;
        }

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

        internal string CommandNameToName(string name)
        => name = name.EndsWith("Command")
                   ? name = name[..^7]
                   : name;

        internal string CommandNameToCliName(string name)
        => CommandNameToName(name).ToKebabCase().ToLowerInvariant();


        public virtual string ArgumentNameToName(string name)
        => name.EndsWith("Arg")
                   ? name = name[..^3]
                   : name;

        public virtual string ArgumentNameToCliName(string name)
        => ArgumentNameToName(name).ToKebabCase().ToLowerInvariant();

        public virtual string OptionNameToName(string name)
        => name = name.EndsWith("Option")
                   ? name = name[..^6]
                   : name;

        public virtual string OptionNameToCliName(string name)
        => $"--{OptionNameToName(name).ToKebabCase().ToLowerInvariant()}";

        public virtual ArgTypeInfoBase? GetArgTypeInfo<TMemberSymbol>(TMemberSymbol symbol)
            => symbol switch
            {
                IPropertySymbol s => new ArgTypeInfoRoslyn(s.Type),
                // TODO: get type from parameter
                IParameterSymbol s => new ArgTypeInfoRoslyn(s.Type),
                _ => null
            };

        internal IEnumerable<object> GetAllowedValues<TMemberSymbol>(TMemberSymbol symbol)
           where TMemberSymbol : class, ISymbol
        => symbol switch
        {
            IPropertySymbol s => GetAllowedValues(s),
            IParameterSymbol s => GetAllowedValues(s),
            _ => Enumerable.Empty<object>()
        };
        private IEnumerable<object> GetAllowedValues(IPropertySymbol propertySymbol)
        => propertySymbol.AttributeValueForList<AllowedValuesAttribute, object>();

        private IEnumerable<object> GetAllowedValues(IParameterSymbol propertySymbol)
        => propertySymbol.AttributeValueForList<AllowedValuesAttribute, object>();

        internal DefaultValueDescriptor? GetDefaultValue<TMemberSymbol>(TMemberSymbol symbol)
           where TMemberSymbol : class, ISymbol
          => symbol switch
          {
              IPropertySymbol s => GetDefaultValueForProperty(s),
              IParameterSymbol s => GetDefaultValueForParameter(s),
              _ => null
          };

        private DefaultValueDescriptor? GetDefaultValueForProperty(IPropertySymbol propertySymbol)
        {
            var field = propertySymbol.ContainingType.GetMembers()
                                                      .OfType<IFieldSymbol>()
                                                      .Where(field => SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, propertySymbol))
                                                      .FirstOrDefault();
            if (field is null)
            {
                // it isn't an auto-property
                return null;
            }

            string defaultValue = languageHelper.GetDefaultValue(propertySymbol);
            return defaultValue is null
                ? null
                : (DefaultValueDescriptor)new ExplicitDefaultValueDescriptor(defaultValue);
        }

        internal bool GetAsync<TCommandSymbol>(TCommandSymbol symbol) where TCommandSymbol : class, ISymbol
        {
            return symbol switch
            {
                INamedTypeSymbol _ => false,
                IMethodSymbol s => s.IsAsync,
                _ => throw new NotImplementedException()
            };
        }

        private DefaultValueDescriptor? GetDefaultValueForParameter(IParameterSymbol symbol) 
            => !symbol.HasExplicitDefaultValue
                ? null
                : new DefaultValueDescriptor(symbol.ExplicitDefaultValue);

        internal bool GetIsHidden<TSymbol>(TSymbol symbol)
            where TSymbol : ISymbol
            => symbol switch
            {
                IPropertySymbol s => s.BoolAttributeValue<HiddenAttribute>(),
                IParameterSymbol s => s.BoolAttributeValue<HiddenAttribute>(),
                INamedTypeSymbol s => s.BoolAttributeValue<HiddenAttribute>(),
                _ => false
            };

        internal bool GetIsRequired<TSymbol>(TSymbol symbol)
            where TSymbol : ISymbol
            => symbol switch
            {
                IPropertySymbol s => s.BoolAttributeValue<RequiredAttribute>(),
                IParameterSymbol s => s.BoolAttributeValue<RequiredAttribute>(),
                _ => false
            };

        internal bool GetTreatUnmatchedTokensAsErrors<TSymbol>(TSymbol symbol)
            where TSymbol : class, ISymbol
            => symbol switch
            {
                INamedTypeSymbol s => s.BoolAttributeValue<TreatUnmatchedTokensAsErrorsAttribute>(true),
                _ => false
            };

        public virtual string OptionArgumentNameToCliName(string name)
        {
            name = name.EndsWith("Option")
                   ? name = name[^6..]
                   : name;
            return $"--{name.ToKebabCase().ToLowerInvariant()}";
        }

        internal IEnumerable<string> GetAliases<TSymbol>(TSymbol symbol)
             where TSymbol : ISymbol
             => symbol switch
             {
                 IPropertySymbol s => s.AttributeValueForList<AliasesAttribute, string>(),
                 IParameterSymbol s => s.AttributeValueForList<AliasesAttribute, string>(),
                 INamedTypeSymbol s => s.AttributeValueForList<AliasesAttribute, string>(),
                 _ => new List<string>()
             };

        internal IEnumerable<string> GetAliases(IParameterSymbol symbol)
            => symbol.AttributeValueForList<AliasAttribute, string>();

        private List<IDescriptionProvider> AdditionalDescriptionSources = new List<IDescriptionProvider>();

        public void AddDescriptionProvider(IDescriptionProvider descriptionProvider)
        {
            AdditionalDescriptionSources.Add(descriptionProvider);
        }

        public bool UseXmlCommentsForDescription { get; set; } = true;

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
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    return desc;
                }
            }
            return null;
        }

        public IEnumerable<ISymbol> GetSubCommandMembers(INamedTypeSymbol parentSymbol)
        {
            IEnumerable<ISymbol> derivedClasses = parentSymbol.ContainingNamespace
                                    .GetTypeMembers()
                                    .Where(x => SymbolEqualityComparer.Default.Equals(x.BaseType, parentSymbol));
            IEnumerable<ISymbol> methods = parentSymbol.GetMembers()
                                    .OfType<IMethodSymbol>()
                                    .Where(x => x.MethodKind != MethodKind.Constructor &&
                                                x.MethodKind != MethodKind.PropertyGet &&
                                                x.MethodKind != MethodKind.PropertySet);
            return derivedClasses.Union(methods);
        }
    }
}
