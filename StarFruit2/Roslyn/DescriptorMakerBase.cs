using System.CommandLine.Parsing;
using StarFruit2.Common.Descriptors;
using StarFruit2.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;

namespace Starfruit2_B
{

    public class DescriptorMakerBase
    {
        // Create disembodied list of commands. These commands may or may not have their children attached. 
        // Arrange and check for circularity

        protected internal string SourceToOptionName(string sourceName)
        => $"--{sourceName.ToKebabCase()}";

        protected internal string SourceToArgumentName(string sourceName)
        {
            if (sourceName.EndsWith("Arg"))
            {
                sourceName = sourceName[..^3];
            }
            return sourceName.ToKebabCase();
        }

        protected internal string SourceToCommandName(string sourceName)
            => sourceName.ToKebabCase();
    }

    public abstract class DescriptorMakerBase<TCommandSource, TOptionArgSource, TExtra> : DescriptorMakerBase
    {
        protected internal abstract CliDescriptor CreateCliDescriptor(ISymbolDescriptor? parent, TCommandSource source, TExtra extra);
        protected internal abstract bool IsArgument(TOptionArgSource source);
        protected internal bool IsOption(TOptionArgSource source)
        => !IsArgument(source);

        private protected SemanticModel GetSemanticModel(SyntaxNode syntax,
                                                         Compilation? compilation)
        {
            if (compilation == null)
            {
                throw new NotImplementedException();
            }
            SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

            return semanticModel;
        }

    }
}
