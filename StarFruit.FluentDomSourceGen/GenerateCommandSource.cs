﻿using FluentDom;
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
    public class GenerateCommandSource
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
                    .Class(CommandSourceClass(cmd, new TypeRep("RootCommandSource", cmd.CommandSourceClassName()), null))
                    .Classes(new CommandDescriptor[] { cli.CommandDescriptor },
                             c => SubCommandClasses(c));
        }

        protected virtual Class CommandSourceClass(CommandDescriptor cmd,
                                                   TypeRep baseClass,
                                                   CommandDescriptor parent)
            => new Class(cmd.CommandSourceClassName())
                    .Base(baseClass)
                    .Constructor(parent is null ? GetRootCtor(cmd, cmd.Root, parent) : GetCtor(cmd, cmd.Root, parent))
                    .OptionalMembers(!(parent is null),
                            cl => cl.BlankLine(),
                            cl => cl.Field("parent", parent.CommandSourceClassName()),
                            cl => CommandSourceResultMethod(cl, cmd))
                    .BlankLine()
                    .Properties(cmd.GetChildren(),
                                s => ChildProperty(s))
                    .BlankLine()
                    .Methods(cmd.GetOptionsAndArgs(),
                             s => ChildGetMethod(s));

        protected virtual Constructor GetRootCtor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
            => new Constructor(cmd.CommandSourceClassName())
                .BaseCall(NewObject("Command", Value(cmd.CliName), Value(cmd.Description)))
                .Statements(cmd.GetOptionsAndArgs(), CtorOptionsAndArgs())
                .Statements(cmd.SubCommands, CtorSubCommands())
                .Statements(Assign("Command.Handler", MethodCall("CommandHandler.Create",
                        MultilineLambda()
                                .Parameter("context", "InvocationContext")
                                .Statements(
                                     Assign("CurrentCommandSource", This()),
                                     Assign("CurrentParseResult", "context.ParseResult"),
                                     Return(Value(0))))));

        protected virtual Constructor GetCtor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
            => new Constructor(cmd.CommandSourceClassName())
                .BaseCall(NewObject("Command", Value(cmd.CliName), Value(cmd.Description)))
                .Statements(Assign(Dot(This(), "parent"), "parent"))
                .Statements(cmd.GetOptionsAndArgs(), CtorOptionsAndArgs())
                .Statements(cmd.SubCommands, CtorSubCommands())
                .Statements(Assign("Command.Handler", MethodCall("CommandHandler.Create",
                        MultilineLambda()
                                .Statements(
                                     Assign("CurrentCommandSource", This()),
                                     Return(Value(0))))));

        protected virtual Func<SymbolDescriptor, IExpression>[] CtorOptionsAndArgs()
            => new List<Func<SymbolDescriptor, IExpression>>
            {
                o => Assign(o.GetPropertyName(), MethodCall(GetOptArgMethodName(o))),
                o => MethodCall($"Command.Add", o.GetPropertyName())
            }.ToArray();

        protected virtual Func<CommandDescriptor, IExpression>[] CtorSubCommands()
            => new List<Func<CommandDescriptor, IExpression>>
            {
                o => Assign(o.GetPropertyName(), NewObject($"{o.CommandSourceClassName()}", This(), This())),
                o => MethodCall($"Command.AddCommand", o.GetPropertyName())
            }.ToArray();

        protected virtual Property ChildProperty(SymbolDescriptor symbol)
            => symbol switch
            {
                OptionDescriptor o => new Property(o.GetPropertyName(), o.OptionType()),
                ArgumentDescriptor a => new Property(a.GetPropertyName(), a.ArgumentType()),
                CommandDescriptor c => new Property(c.GetPropertyName(), $"{c.CommandSourceClassName()}"),
                _ => throw new InvalidOperationException("Unexpected symbol type")
            };

        protected virtual Method ChildGetMethod(SymbolDescriptor symbol)
            => symbol switch
            {
                OptionDescriptor o => GetOptionMethod(o),
                ArgumentDescriptor a => GetArgumentMethod(a),
                _ => throw new InvalidOperationException("Unexpected symbol type")
            };

        protected virtual Method GetArgumentMethod(ArgumentDescriptor o)
        {
            var method = new Method(GetOptArgMethodName(o))
                .ReturnType(o.ArgumentType())
                .Statements(
                      AssignVar("argument", o.ArgumentType(), NewObject(o.ArgumentType(), o.CliName)),
                      Assign("argument.Description", Value(o.Description)),
                      Assign("argument.IsRequired", Value(o.Required)),
                      Assign("argument.IsHidden", Value(o.IsHidden)))
                 .OptionalStatements(
                      o.DefaultValue is not null,
                      () => MethodCall("optionArg.SetDefaultValue", Value(o.DefaultValue.DefaultValue)))
                 .Statements(Return(VariableReference("argument")));

            return method;
        }

        protected virtual Method GetOptionMethod(OptionDescriptor o)
        {
            var method = new Method(GetOptArgMethodName(o))
                .ReturnType(o.OptionType())
                .Statements(
                      AssignVar("option", o.OptionType(), NewObject(o.OptionType(), o.CliName)),
                      Assign("option.Description", Value(o.Description)),
                      Assign("option.IsRequired", Value(o.Required)),
                      Assign("option.IsHidden", Value(o.IsHidden)))
                 .OptionalStatements(
                      o.Arguments?.FirstOrDefault()?.DefaultValue is not null,
                      () => MethodCall("optionArg.SetDefaultValue", Value(o.Arguments.First().DefaultValue.DefaultValue)))
                 .Statements(
                      o.Aliases,
                      alias => MethodCall("option.AddAlias", Value($"-{alias}")))
                 .Statements(Return(VariableReference("option")));

            return method;
        }

        protected virtual IEnumerable<Class> SubCommandClasses(CommandDescriptor cmd)
        {
            List<Class> list = new();
            //CommandClass(c, new TypeRep(cmd.Root.CommandSourceClassName());
            foreach (var subCommand in cmd.SubCommands)
            {
                list.Add(CommandSourceClass(subCommand, cmd.CommandSourceClassName(), subCommand.ParentSymbolDescriptorBase as CommandDescriptor));
                if (subCommand.SubCommands.Any())
                {
                    list.AddRange(SubCommandClasses(subCommand));
                }
            }
            return list;
        }

        private Class CommandSourceResultMethod(Class cls, CommandDescriptor cmd)
            => cls.Method(new Method("GetCommandSourceResult")
                          .ReturnType("CommandSourceResult")
                          .Parameter("parseResult", "ParseResult")
                          .Parameter("int", "exitCode")
                          .Statements(Return(NewObject(cmd.CommandSourceClassName(),
                                              VariableReference("parseResult"),
                                              This(),
                                              VariableReference("exitCode"))))
                                );

        protected virtual string GetOptArgMethodName(SymbolDescriptor symbol)
        {
            return $"Get{symbol.OriginalName}";
        }


    }
}

