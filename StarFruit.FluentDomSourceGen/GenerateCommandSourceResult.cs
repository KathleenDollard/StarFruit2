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

        public virtual Code CreateCode(CliDescriptor cli, IEnumerable<Using>? sourceUsings = null)
        {
            _ = cli ?? throw new InvalidOperationException("CliDescriptor cannot be null");
            var cmd = cli.CommandDescriptor;
            return Code.Create(cli.GeneratedComandSourceNamespace ?? "default namespace")
                .Usings("StarFruit2")
                .Usings("System.CommandLine")
                .Usings("StarFruit2.Common")
                .Usings("System.CommandLine.Invocation")
                .Usings("System.CommandLine.Parsing")
                .Usings(sourceUsings)
                    .Class(CommandResultClass(cmd, new TypeRep("CommandSourceResult", cmd.OriginalName)))
                    .Classes(cmd.SubCommands,
                             c => CommandResultClass(c, (c.ParentSymbolDescriptorBase as CommandDescriptor)?.CommandSourceResultClassName() ?? "<missing name>"));
        }

        private Class CommandResultClass(CommandDescriptor cmd, TypeRep baseTypeRep)
            => new Class(cmd.CommandSourceResultClassName())
                .Base(baseTypeRep)
                .Constructor(GetCtor(cmd))
                .Properties(cmd.GetOptionsAndArgs(),
                            s => ChildProperty(s))
                .BlankLine()
                .OptionalMembers(cmd.OriginalElementType == OriginalElementType.Class,
                                 c => c.Method(CreateInstance(cmd)));

        private Constructor GetCtor(CommandDescriptor cmd)
            => new Constructor(cmd.CommandSourceResultClassName())
                    .Parameter("parseResult", "ParseResult")
                    .Parameter("commandSource", cmd.CommandSourceClassName())
                    .Parameter("exitCode", "int")
                    .BaseCall(VariableReference("parseResult"), CommandSourceParent(cmd), VariableReference("exitCode"))
                    .Statements(cmd.GetOptionsAndArgs(), x => CtorAssigns(cmd, x));

        private IExpression CommandSourceParent(CommandDescriptor cmd)
            => As(VariableReference("commandSource.ParentCommandSource"),
                     (cmd.ParentSymbolDescriptorBase as CommandDescriptor).CommandSourceClassName());

        private IExpression CtorAssigns(CommandDescriptor cmd, SymbolDescriptor symbol)
            => Assign(symbol.PropertyResultName(),
                  MethodCall("CommandSourceMemberResult.Create", $"commandSource.{symbol.PropertyName()}", "parseResult"));

        private Property ChildProperty(SymbolDescriptor symbol)
            => symbol switch
            {
                OptionDescriptor o => new Property(o.PropertyResultName(), new TypeRep("CommandSourceMemberResult", o.GetFluentArgumentType())),
                ArgumentDescriptor a => new Property(a.PropertyResultName(), new TypeRep("CommandSourceMemberResult", a.GetFluentArgumentType())),
                _ => throw new InvalidOperationException("Unexpected symbol type")
            };

        private Method CreateInstance(CommandDescriptor cmd)
        {
            const string newItem = "newItem";
            var arguments = cmd.GetOptionsAndArgs()
                               .Where(x => x.OriginalElementType == OriginalElementType.CtorParameter)
                               .Select(s => $"{s.ParameterResultName()}.Value")
                               .ToArray();
            return new Method("CreateInstance", modifiers: MemberModifiers.Override)
                           .ReturnType(cmd.OriginalName)
                           .Statements(AssignVar(newItem, "var", NewObject(cmd.OriginalName, arguments)))
                           .Statements(cmd.GetOptionsAndArgs()
                                          .Where(x => x.OriginalElementType == OriginalElementType.Property),
                                       x => Assign(VariableReference($"{newItem}.{x.OriginalName}"),
                                                   VariableReference($"{x.PropertyResultName()}.Value")))
                           .Statements(Return(VariableReference(newItem)));
        }

        private Method Run(CommandDescriptor cmd)
        {
            return new Method("Run", modifiers: MemberModifiers.Override)
                       .ReturnType("int")
                       .Statements(Return(MethodCall($"CreateInstance().{cmd.OriginalName}", GetArgs(cmd))));

            static IExpression[] GetArgs(CommandDescriptor cmd)
                => cmd.GetOptionsAndArgs()
                      .Select(x => Dot(x.PropertyResultName(), "Value"))
                      .ToArray();
        }


    }
}

