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
                                              CommandDescriptor? parent)
        {
            return new Class(cmd.CommandSourceClassName())
                               .Base(baseClass)
                               .OptionalMember(parent is null, ParameterlessCtor(cmd))
                               .Constructor(GetCtor(cmd, cmd.Root, parent))
                               .BlankLine()
                               .Properties(cmd.GetChildren(),
                                           s => ChildProperty(s))
                               .BlankLine()
                               .Methods(cmd.GetOptionsAndArgs(),
                                        s => ChildGetMethod(s));

            static IClassMember ParameterlessCtor(CommandDescriptor cmd)
                => new Constructor(cmd.CommandSourceClassName())
                            .ThisCall(Null(), Null());
       }


        protected virtual Constructor GetCtor(CommandDescriptor cmd, CommandDescriptor root, CommandDescriptor? parent)
        {
            return new Constructor(cmd.CommandSourceClassName())
                           .Parameter("root", "RootCommandSource")
                           .Parameter("parent", "CommandSource")
                           .BaseCall(NewObject("Command", Value(cmd.CliName), Value(cmd.Description)), VariableReference("parent"))
                           .Statements(cmd.GetOptionsAndArgs(), CtorOptionsAndArgs())
                           .Statements(cmd.SubCommands, CtorSubCommands())
                           .Statements(Assign("Command.Handler", MethodCall("CommandHandler.Create",
                                         GetCommandHandler(parent is null))));

            static MultilineLambda GetCommandHandler(bool isRoot)
            {
                return isRoot
                        ? MultilineLambda()
                              .Parameter("context", "InvocationContext")
                              .Statements(
                                   Assign("CurrentCommandSource", This()),
                                   Assign("CurrentParseResult", "context.ParseResult"),
                                   Return(Value(0)))
                          : MultilineLambda()
                              .Statements(
                                   Assign("root.CurrentCommandSource", This()),
                                   Return(Value(0)));
            }
        }

        //protected virtual Constructor GetCtor(CommandDescriptor cmd, CommandDescriptor root, CommandDescriptor parent)
        //    => new Constructor(cmd.CommandSourceClassName())
        //        .Parameter("root", "RootCommandSource")
        //        .Parameter("parent", "CommandSource")
        //        .BaseCall(NewObject("Command", Value(cmd.CliName), Value(cmd.Description)), VariableReference("parent"))
        //        .Statements(cmd.GetOptionsAndArgs(), CtorOptionsAndArgs())
        //        .Statements(cmd.SubCommands, CtorSubCommands())
        //        .Statements(Assign("Command.Handler", MethodCall("CommandHandler.Create",
        //                MultilineLambda()
        //                        .Statements(
        //                             Assign("root.CurrentCommandSource", This()),
        //                             Return(Value(0))))));

        protected virtual Func<SymbolDescriptor, IExpression>[] CtorOptionsAndArgs()
            => new List<Func<SymbolDescriptor, IExpression>>
            {
                o => Assign(o.PropertyName(), MethodCall(GetOptArgMethodName(o))),
                o => MethodCall($"Command.Add", o.PropertyName())
            }.ToArray();

        protected virtual Func<CommandDescriptor, IExpression>[] CtorSubCommands()
            => new List<Func<CommandDescriptor, IExpression>>
            {
                o => Assign(o.PropertyName(), NewObject($"{o.CommandSourceClassName()}", This(), This())),
                o => MethodCall($"Command.AddCommand",$"{o.PropertyName()}.Command")
            }.ToArray();

        protected virtual Property ChildProperty(SymbolDescriptor symbol)
            => symbol switch
            {
                OptionDescriptor o => new Property(o.PropertyName(), o.OptionType()),
                ArgumentDescriptor a => new Property(a.PropertyName(), a.ArgumentType()),
                CommandDescriptor c => new Property(c.PropertyName(), $"{c.CommandSourceClassName()}"),
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
                      AssignVar("argument", o.ArgumentType(), NewObject(o.ArgumentType(), Value(o.CliName))),
                      Assign("argument.Description", Value(o.Description)),
                      Assign("argument.IsHidden", Value(o.IsHidden)))
                 .OptionalStatements(
                      o.DefaultValue is not null,
                      () => MethodCall("argument.SetDefaultValue", Value(o.DefaultValue!.DefaultValue)))
                 .Statements(Return(VariableReference("argument")));

            return method;
        }

        protected virtual Method GetOptionMethod(OptionDescriptor o)
        {
            var method = new Method(GetOptArgMethodName(o))
                .ReturnType(o.OptionType())
                .Statements(
                      AssignVar("option", o.OptionType(), NewObject(o.OptionType(), Value(o.CliName))),
                      Assign("option.Description", Value(o.Description)),
                      Assign("option.IsRequired", Value(o.Required)),
                      Assign("option.IsHidden", Value(o.IsHidden)))
                 .OptionalStatements(
                      o.Arguments?.FirstOrDefault()?.DefaultValue is not null,
                      () => AssignVar("optionArg", "var", Dot(VariableReference("option"), "Argument")),
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
                list.Add(CommandSourceClass(subCommand, "CommandSource", subCommand.ParentSymbolDescriptorBase as CommandDescriptor));
                if (subCommand.SubCommands.Any())
                {
                    list.AddRange(SubCommandClasses(subCommand));
                }
            }
            return list;
        }

        private Class CommandSourceResultMethod(Class cls, CommandDescriptor cmd)
            => cls.Method(new Method("GetCommandSourceResult", modifiers: MemberModifiers.Override)
                          .ReturnType("CommandSourceResult")
                          .Parameter("parseResult", "ParseResult")
                          .Parameter("exitCode", "int")
                          .Statements(Return(NewObject(cmd.CommandSourceResultClassName(),
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

