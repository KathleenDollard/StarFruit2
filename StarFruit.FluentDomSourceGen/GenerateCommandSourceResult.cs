using FluentDom;
using StarFruit.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using static FluentDom.Expression;

// KAD: Extensibility
// * Use a base/derived overlad extensibility mechanism.Overriding methods may need to navigate DOM
// * Use a two phase appraoch to evaluation, maintaining Select methods as IENumerable and Funcs for extension
// * Have a separate calculate values method for each method so it can be called at any point
namespace StarFruit2.Generate
{
    public class GenerateCommandSourceResult
    {

        public virtual Code CreateCode(CliDescriptor cli)
        {
            var cmd = cli.CommandDescriptor;
            var usings = new Using[]
            {
                        "StarFruit2",
                        "System.CommandLine",
                        "StarFruit2.Common",
                        "System.CommandLine.Invocation",
                        "System.CommandLine.Parsing"
            };

            _ = cli ?? throw new InvalidOperationException("CliDescriptor cannot be null");

            return Code.Create(cli.GeneratedComandSourceNamespace ?? "default namespace")
                    .Usings(usings)
                    .Class(RootCommandResultClass(cmd))
                    .Classes(cli.CommandDescriptor.SubCommands,
                             c => SubCommandResultClass(c));
        }

        private Class RootCommandResultClass(CommandDescriptor cmd)
            => new Class(cmd.CommandSourceResultClassName())
                .Base(new TypeRep("CommandSourceResult", cmd.OriginalName))
                .Constructor(GetRootCtor(cmd, cmd.Root))
                .Properties(cmd.GetOptionsAndArgs(),
                            s => ChildProperty(s))
                .BlankLine()
                .Method(CreateInstance(cmd));

        private Class SubCommandResultClass(CommandDescriptor cmd)
            => new Class(cmd.CommandSourceResultClassName())
                .Base((cmd.ParentSymbolDescriptorBase as CommandDescriptor)?.CommandSourceResultClassName() ?? "<missing name>")
                .Constructor(GetCtor(cmd))
                .Properties(cmd.GetChildren(),
                            s => ChildProperty(s))
                .BlankLine()
                .Method(CreateInstance(cmd))
                .Method(Run(cmd));


        private Constructor GetRootCtor(CommandDescriptor cmd, CommandDescriptor root)
           => new Constructor(cmd.CommandSourceResultClassName())
                   .Parameter("parseResult", "ParseResult")
                   .Parameters(cmd.GetOptionsAndArgs(), x => ResultParameterMaker(x))
                   .Parameter("exitCode", "int")
                   .BaseCall("parseResult", "exitCode")
                   .Statements(cmd.GetOptionsAndArgs(), x => CtorAssigns(x));

        private Constructor GetCtor(CommandDescriptor cmd)
            => new Constructor(cmd.CommandSourceResultClassName())
                    .Parameter("parseResult", "ParseResult")
                    .Parameters(cmd.GetOptionsAndArgs(), x => ResultParameterMaker(x))
                    .Parameter("exitCode", "int")
                    .BaseCall(CtorBaseCallArgs(cmd))
                    .Statements(cmd.GetOptionsAndArgs(), x => CtorAssigns(x));

        private IExpression[] CtorBaseCallArgs(CommandDescriptor cmd)
        {
            if (cmd?.ParentSymbolDescriptorBase is not CommandDescriptor parent)
            {
                return new IExpression[] { };
            }
            var args = parent.GetOptionsAndArgs()
                             .Select(x => (IExpression)MethodCall("CommandSourceMemberResult.Create", cmd.CommandSourceClassName()))
                             .ToList();
            args.Insert(0, VariableReference("parseResult"));
            args.Add(VariableReference("exitCode"));
            return args.ToArray(); ;
        }

        private IExpression CtorAssigns(SymbolDescriptor symbol)
            => Assign(symbol.GetPropertyResultName(), symbol.GetParameterResultName());

        private Parameter ResultParameterMaker(ISymbolDescriptor item)
            => new Parameter(item.GetParameterResultName(), new TypeRep("CommandSourceMemberResult", item.SymbolType()));

        private Property ChildProperty(SymbolDescriptor symbol)
            => symbol switch
            {
                OptionDescriptor o => new Property(o.GetParameterResultName(), new TypeRep("CommandSourceMemberResult", o.OptionType())),
                ArgumentDescriptor a => new Property(a.GetParameterResultName(), new TypeRep("CommandSourceMemberResult", a.ArgumentType())),
                _ => throw new InvalidOperationException("Unexpected symbol type")
            };

        private Method CreateInstance(CommandDescriptor cmd)
        {
            const string leftHand = "newItem";
            var arguments = cmd.GetOptionsAndArgs()
                               .Where(x => x.CodeElement == CodeElement.CtorParameter)
                               .Select(s => s.GetParameterResultName())
                               .ToArray();
            return new Method("CreateInstance", modifiers: MemberModifiers.Override)
                           .ReturnType(cmd.OriginalName)
                           .Statements(Assign(leftHand, NewObject(cmd.OriginalName, arguments)))
                           .Statements(cmd.GetOptionsAndArgs(), x => Assign(VariableReference(x.OriginalName), VariableReference(x.GetPropertyResultName())));
        }

        private Method Run(CommandDescriptor cmd)
        {
            return new Method("Run", modifiers: MemberModifiers.Override)
                       .ReturnType("int")
                       .Statements(Return(MethodCall($"CreateInstance().{cmd.OriginalName}", GetArgs(cmd))));

            static IExpression[] GetArgs(CommandDescriptor cmd)
                => cmd.GetOptionsAndArgs()
                      .Select(x => Dot(x.GetPropertyResultName(), "Value"))
                      .ToArray();
        }
 

    }
}

