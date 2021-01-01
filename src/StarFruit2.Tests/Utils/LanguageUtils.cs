using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace StarFruit2.Tests
{
    public abstract class LanguageUtils
    {
        // Do not call this MyClass, that is a keyword in VB
        public const string StandardClassName = "MyGreatClass";
        public const string StandardNamespace = "MyNamespace";
        public const string StandardMethodName = "MyMethod";

        internal static (LanguageUtils utils, string code) Choose(Type type, string cSharpCode, string vbCode)
        {
            var utils = Activator.CreateInstance(type) as LanguageUtils;
            _ = utils ?? throw new InvalidOperationException("Unexpected type for Language Utility");
            var code = utils is CSharpLanguageUtils
                       ? cSharpCode
                       : vbCode;
            return (utils, code);
        }

        public static string WrapInClass(Type type, string code, string className = StandardClassName, string namespaceName = StandardNamespace, params string[] usings)
        {
            var utils = Activator.CreateInstance(type) as LanguageUtils;
            _ = utils ?? throw new InvalidOperationException("Unexpected type for Language Utility");
            return utils.PrefaceWithUsing(
                            utils.WrapInClass(code, className, namespaceName),
                            usings);
        }
        public static string WrapInNamespace(Type type, string code, string namespaceName = StandardNamespace, params string[] usings)
        {
            var utils = Activator.CreateInstance(type) as LanguageUtils;
            _ = utils ?? throw new InvalidOperationException("Unexpected type for Language Utility");
            return utils.PrefaceWithUsing(utils.WrapInNamespace(code, namespaceName), usings); 
        }

        protected static IEnumerable<MetadataReference> GetAssemblyReferences()
        {
            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }
            return references;
        }

        public abstract string PrefaceWithUsing(string code, params string[] usingNamespaces);
        public abstract string WrapInClass(string code, string className = StandardClassName, string namespaceName = StandardNamespace);
        public abstract string WrapInNamespace(string code, string namespaceName);
        public abstract SyntaxTree GetSyntaxTree(string code);
        public abstract Compilation GetCompilation(SyntaxTree syntaxTree);
        public abstract IEnumerable<SyntaxNode> GetClassNodes(SyntaxTree syntaxTree);
        public abstract IEnumerable<SyntaxNode> GetMethodNodes(SyntaxTree syntaxTree);
    }
}
