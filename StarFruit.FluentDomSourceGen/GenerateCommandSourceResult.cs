using FluentDom;
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

            return Code.Create(cli.GeneratedComandSourceNamespace)
                    .Usings(usings)
                    .Class(CommandResultClass(cmd, new TypeRep("RootCommandSource", cmd.CommandSourceClassName()), null))
                    .Classes(new CommandDescriptor[] { cli.CommandDescriptor },
                             c => SubCommandClasses(c));
        }


        private Class CommandResultClass(CommandDescriptor cmd,
                                         TypeRep baseClass,
                                         object parent)
              => new Class(cmd.CommandSourceResultClassName())
                    .Base(baseClass)
                    .Constructor(parent is null ? GetRootCtor(cmd, cmd.Root, parent) : GetCtor(cmd, cmd.Root, parent))
                    .Properties(cmd.GetChildren(),
                                s => ChildProperty(s))
                    .BlankLine()
                    .Method(CreateInstance(cmd));

        private Constructor GetRootCtor(CommandDescriptor cmd, CommandDescriptor root, object parent)
            => new Constructor(cmd.CommandSourceClassName())
                    .Parameter("parseResult", "ParseResult")
                    .Parameters(cmd.GetOptionsAndArgs(), x => ResultParameterMaker(x))
                    .Parameter("exitCode", "int")
                    .BaseCall(NewObject("parseResult", "exitCode"))
                    .Statements(cmd.GetOptionsAndArgs(), x => CtorAssigns(x));

        private IExpression CtorAssigns(SymbolDescriptor symbol)
            => Assign(symbol.GetPropertyName(), symbol.GetPropertyResultName());

        private Parameter ResultParameterMaker(ISymbolDescriptor item)
            => new Parameter(item.GetParameterResultName(), new TypeRep("CommandSourceMemberResult", item.SymbolType()));

        private Constructor GetCtor(CommandDescriptor cmd, CommandDescriptor root, object parent)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Class> SubCommandClasses(CommandDescriptor c)
        {
            throw new NotImplementedException();
        }

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
            var arguments = cmd.GetOptionsAndArgs().Where(x=>x.ParentSymbolDescriptorBase is ).(.Select(s => s.GetParameterResultName());
            return method = new Method("CreateInstance", modifiers: MemberModifiers.Override)
                           .Statements(Assign(leftHand, NewObject(cmd.OriginalName, arguments.ToArray())))
                           .Statements(cmd.GetOptionsAndArgs(), x => Assign(VariableReference(x.OriginalName), VariableReference(x.GetPropertyResultName())));
        }
    }
}

