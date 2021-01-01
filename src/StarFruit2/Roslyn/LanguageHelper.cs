using Microsoft.CodeAnalysis;
using System;

namespace StarFruit2
{
    public abstract class LanguageHelper
    {
        public abstract string? GetDefaultValue(IPropertySymbol propertySymbol);
        protected abstract string ArgTypeInfoAsStringInternal(object? typeRepresentation);

        protected LanguageHelper()
            => HelperInUse = this;

        private static LanguageHelper? HelperInUse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageName">A member of Microsoft.CodeAnalysis.LanguageNames</param>
        /// <returns></returns>
        public static LanguageHelper GetLanguageHelper(string languageName)
            => languageName switch
            {
                LanguageNames.CSharp => new CSharpLanguageHelper(),
                LanguageNames.VisualBasic => new VBLanguageHelper(),
                LanguageNames.FSharp => throw new ArgumentException("F# is not supported by Roslyn generators because it doesn't use Rolsyn.", "languageName"),
                _ => throw new ArgumentException("Unexpected language type", "languageName")
            };

        internal static string ArgTypeInfoAsString(object? typeRepresentation)
            => HelperInUse is null
               ? throw new InvalidOperationException("You cannot use a helper until it is initialized")
               : HelperInUse.ArgTypeInfoAsStringInternal(typeRepresentation);

    }

}