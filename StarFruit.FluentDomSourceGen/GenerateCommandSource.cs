using FluentDom;
using FluentDom.Generator;
using StarFruit2.Common.Descriptors;
using System;
using System.Linq;
using static FluentDom.Expression;

// KAD: Extensibility
// * Use a base/derived overlad extensibility mechanism.Overriding methods may need to navigate DOM
// * Use a two phase appraoch to evaluation, maintaining Select methods as IENumerable and Funcs for extension
// * Have a separate calculate values method for each method so it can be called at any point
namespace GeneratorSupport
{
    public class GenerateCommandSource
    {

        public void Can_create_code()
        {
            var cli = new CliDescriptor();
            cli.CommandDescriptor = new CommandDescriptor(null, "Fizz", null);
            var code = CreateCode(cli);
            var ouptput = new GeneratorBase().Generate(code);
        }

        public virtual Code CreateCode(CliDescriptor cli)
        {
            var cmd = cli.CommandDescriptor;
            var baseClass = "";

            return Code.Create(cli.GeneratedComandSourceNamespace)
                    .Usings(
                        "StarFruit2",
                        "System.CommandLine",
                        new Using("StarFruit2.Common"),
                        "System.CommandLine.Invocation",
                        "System.CommandLine.Parsing")
                    .Class(RootClass(cli.CommandDescriptor))
                    .Classes(cli.CommandDescriptor.SubCommands,
                             c => SubCommandClass(c));
        }

        private Class SubCommandClass(CommandDescriptor c)
        {
            throw new NotImplementedException();
        }

        protected virtual Class RootClass(CommandDescriptor cmd)
        {
            return new Class(cmd.CommandSourceClassName())
                        .Base($"RootCommandSource<{cmd.CommandSourceClassName()}>")
                        .Constructor(GetRootCtor(cmd, null, null))
                        .Properties(cmd.GetChildren(),
                                    s => Property(s))
                        .Methods(cmd.GetOptionsAndArgs(),
                                 s => GetMethod(s));

        }


        protected virtual Constructor GetRootCtor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
        {
            Constructor newCtor = new Constructor(cmd.CommandSourceClassName())
                .BaseCall(NewObject("Command", cmd.CliName))
                .Statements(cmd.GetOptionsAndArgs(),
                            o => Assign(o.OriginalName, MethodCall(GetOptArgMethodName(o.OriginalName))),
                            o => MethodCall($"Command.Add", o.OriginalName))
                .Statements(cmd.SubCommands,
                            o => Assign(o.OriginalName, NewObject("$Get{o.OriginalName}CommandSource", This(), This())),
                            o => MethodCall($"Command.AddCommand", o.OriginalName))
                .Statements(Assign("CommandHandler", MethodCall("CommandHandler.Create", CreateLambda())));
            return newCtor;

            static Expression CreateLambda()
            {
                return Expression.MultilineLambda()
                    .Parameter("InvocationContext", "context")
                    .Statements(
                        Assign("CurrentCommandSource", This()),
                        Assign("CurrentParseResult", "context.ParseResult"))
                    .Return("0");
            }
        }

        protected virtual string GetOptArgMethodName(string name)
        {
            return $"Get{name}";
        }

        protected virtual Constructor GetCtor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
        {
            Constructor newCtor = new Constructor(cmd.CommandSourceClassName());
            newCtor.Parameter("root", $"{cmd.Root.OriginalName}CommandSource");
            newCtor.Parameter("parent", $"{cmd.ParentSymbolDescriptorBase.OriginalName}CommandSource");
            return newCtor;
        }

        protected virtual Property Property(SymbolDescriptor symbol)
        {
            return symbol switch
            {
                OptionDescriptor o => new Property(o.OriginalName, OptionType(o)),
                ArgumentDescriptor a => new Property(a.OriginalName, new TypeRep("Argument", a.GetFluentArgumentType())),
                CommandDescriptor c => new Property(c.OriginalName, $"{c.OriginalName}CommandSource"),
                _ => throw new InvalidOperationException("Unexpected symbol type")
            };
        }

        private static TypeRep OptionType(OptionDescriptor o)
        {
            return new TypeRep("Option", o.GetFluentArgumentType());
        }

        protected virtual Method GetMethod(SymbolDescriptor symbol)
        {
            return symbol switch
            {
                OptionDescriptor o => GetOptionMethod(o),
                ArgumentDescriptor a => GetArgumentMethod(a),
                _ => throw new InvalidOperationException("Unexpected symbol type")
            };
        }

        protected virtual Method GetArgumentMethod(ArgumentDescriptor a)
        {
            throw new NotImplementedException();
        }

        protected virtual Method GetOptionMethod(OptionDescriptor o)
        {
            var method = new Method(GetOptArgMethodName(o.OriginalName))
                .ReturnType(OptionType(o))
                .Statements(
                      AssignVar("option", OptionType(o).ToString(), NewObject(OptionType(o).ToString(), o.CliName)),
                      Assign("option.Description", o.Description),
                      Assign("option.IsRequired", Value(o.Required)),
                      Assign("option.IsHidden", Value(o.IsHidden)),
                      AssignVar("optionArg", "Boolean", Value(o.IsHidden)),
                      Assign("option.Argument", "optionArg"))
                 .Optional(
                      o.Arguments.First().DefaultValue is not null,
                      () => MethodCall("optionArg.SetDefaultValue", Value(o.Arguments.First().DefaultValue.DefaultValue)))
                 .Statements(
                      o.Aliases,
                      alias => MethodCall("option.AddAlias", Value($"-{alias}")))
                 .Return("option");

            //// This or optional in statements group
            //if (o.Arguments.First().DefaultValue is not null)
            //{
            //    MethodCall("optionArg.SetDefaultValue", Value(o.Arguments.First().DefaultValue.DefaultValue));
            //}

            //foreach (var alias in o.Aliases)
            //{
            //    MethodCall("option.AddAlias", Value($"-{alias}"));
            //}

            //method.Return("option");

            return method;
        }
    }

    public static class StarFruitExtensions
    {

    }
}

