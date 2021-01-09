using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;
using FluentDom;
using System;
using RoslynSourceGenSupport.VisualBasic;

namespace StarFruit2.Generate
{
    public class VBSyntaxReceiver : SyntaxReceiverBase
    {

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
            var memberAccessSyntax = syntaxNode.CallToMethodOnClass("CommandSource");
            return AddCandidateIfNotNull(memberAccessSyntax?.Name.GenericArgumentsFromName().FirstOrDefault());
        }

        /// <summary>
        /// This allows specifying the CLI root via a marker interface
        /// It is not clear that this is still needed.
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        private bool AddInterfaceMarkedTypes(SyntaxNode syntaxNode)
            => AddCandidateIfNotNull(syntaxNode.ClassWithBaseOrInterface("ICliRoot"));

        // This allows specifying a simple POCO CLI root class as the CLI root
        // by stating hte generic in CommandSource static methods
        /// <summary>
        /// This allows specifying the CLI root via a marker interface
        /// It is not clear that this is still needed.
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        private bool AddCalledMethods(SyntaxNode syntaxNode)
        {
            var memberAccessSyntax = syntaxNode.CallToMethodOnClass("CommandSource");
            if (memberAccessSyntax is not null
                && !memberAccessSyntax.Name.GenericArgumentsFromName().Any())
            {
                var argument = memberAccessSyntax.ArgumentsOnMethod().FirstOrDefault();
                switch (argument.GetExpression() )
                {
                    case NameOfExpressionSyntax s:
                        var rootIdentifier = s.Argument switch
                        {
                            IdentifierNameSyntax r => (SyntaxNode)r,
                            MemberAccessExpressionSyntax r => r,
                            _ => throw new NotImplementedException(),
                        };
                        return AddCandidateIfNotNull(rootIdentifier);


                    case MemberAccessExpressionSyntax s:
                        return AddCandidateIfNotNull(s);


                    default:
                        return false;

                }
            }

            return false;

        }
    }
}

