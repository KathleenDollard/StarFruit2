using Microsoft.CodeAnalysis;
using System.Linq;
using RoslynSourceGenSupport.CSharp;
using RoslynSourceGenSupport;

namespace StarFruit2.Generate
{
    public class CSharpSyntaxReceiver : CandidateSyntaxVisitor
    {
        //private readonly CSharpRoslynUtilities utilties = new();

            /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the 
        /// nodes and save any information useful for generation
        /// </summary>
        public override void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!AddPocoTypes(syntaxNode))
                if (!AddInterfaceMarkedTypes(syntaxNode))
                    AddCalledMethods(syntaxNode);
        }

        private bool AddCandidateIfNotNull(SyntaxNode? syntaxNode)
        {
            if (syntaxNode is not null
                && !Candidates.Contains(syntaxNode))
            {
                Candidates.Add(syntaxNode);
                Usings.AddRange(syntaxNode.GetUsings()
                                          .Select(y => y.Name.ToString()));
                return true;
            }
            return false;
        }

        /// <summary>
        /// This allows specifying a simple POCO CLI root class as the CLI 
        /// root by stating hte generic in CommandSource static methods
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        private bool AddPocoTypes(SyntaxNode syntaxNode)
        {
            var memberAccessSyntax = syntaxNode.IfCallToMethodOnClass("CommandSource");
            return AddCandidateIfNotNull(memberAccessSyntax?.Name.GetGenericArguments().FirstOrDefault());
        }

        /// <summary>
        /// This allows specifying the CLI root via a marker interface
        /// It is not clear that this is still needed.
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        private bool AddInterfaceMarkedTypes(SyntaxNode syntaxNode)
            => AddCandidateIfNotNull(syntaxNode.IfClassWithBaseOrInterface("ICliRoot"));

        /// <summary>
        /// This allows specifying a simple POCO CLI root class as the CLI 
        /// root by stating the generic in CommandSource static methods
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        private bool AddCalledMethods(SyntaxNode syntaxNode)
        {
            var memberAccessSyntax = syntaxNode.IfCallToMethodOnClass("CommandSource");
            if (memberAccessSyntax is not null
                && !memberAccessSyntax.Name.GetGenericArguments().Any())
            {
                var argument = memberAccessSyntax.ArgumentsOnMethod().FirstOrDefault();
                var nameOfInvocation = argument.Expression.IfCallToMethod("nameof");
                var namedMethodSyntax = nameOfInvocation?.ArgumentList.Arguments.First().Expression;
                return AddCandidateIfNotNull(namedMethodSyntax);
            }
            return false;
        }
    }
}
