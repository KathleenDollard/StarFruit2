//using GeneratorSupport.FluentDom;
//using StarFruit2.Common.Descriptors;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//// Goals:
//// * Easy to read
//// * Intellisense driven discovery
//// * Can be formatted automatically with existing tools
//// * Consistency

//// I think folks could recognize classes for namespace and class members. I think
//// they will need discovery to understand statements and other details.

//// I'm working on variations. This file contains a couple

//namespace GeneratorSupport.Tests
//{
//    public class FluentStarFruitTest_MH
//    {

//        public void Can_create_code()
//        {
//            var cli = new CliDescriptor();
//            cli.CommandDescriptor = new CommandDescriptor(null, "Fizz", null);
//        }
//    }
//}

//namespace GeneratorSupport.Tests.Variation1
//{

//    public class Generator
//    {
//        // Starting point from Friday
//        // * Added the End Class bcause we need to return to the CodeFile  return for the next class
//        // * If formatting gets lost, meaning gets lost
//        public Code CreateCode(CliDescriptor cli)
//        {
//            var cmd = cli.CommandDescriptor;
//            var baseClass = "";

//            return Code.Create(cli.GeneratedComandSourceNamespace)
//                    .Usings(
//                        "StarFruit2",
//                        "System.CommandLine",
//                        "StarFruit2.Common",
//                        "System.CommandLine.Invocation",
//                        "System.CommandLine.Parsing")
//                    .Class(new Class($"{cmd.OriginalName}CommandSource")
//                        .Base(baseClass)
//                        .Constructor(GetRootCtor(cmd, null, null))
//                        .Members(
//                            cmd.Options.Select(x => x.NewProperty()),
//                            cmd.Arguments.Select(x => x.NewProperty()),
//                            cmd.Options.Select(x => GetOptionMethod(x)),
//                            cmd.Arguments.Select(x => GetArgumentMethod(x))))
//                    .Class($"{cmd.OriginalName}CommandSource")
//                        .Base(baseClass)
//                        .Constructor(GetRootCtor(cmd, null, null))
//                        .Members(
//                            cmd.Options.Select(x => x.NewProperty()),
//                            cmd.Arguments.Select(x => x.NewProperty()),
//                            cmd.Options.Select(x => GetOptionMethod(x)),
//                            cmd.Arguments.Select(x => GetArgumentMethod(x)))
//                        .EndClass();
//        }

//        private Constructor GetRootCtor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
//        {
//            Constructor newCtor = new Constructor()
//                .BaseCall(Expression.NewObject("Command", cmd.CliName))
//                .Statements(
//                    cmd.GetOptionsAndArgs().Select(o => Assign(o.OriginalName, MethodCall("$Get{o.OriginalName}"))),
//                    cmd.GetOptionsAndArgs().Select(o => MethodCall($"Command.Add", o.OriginalName)),
//                    cmd.SubCommands.Select(o => Assign(o.OriginalName, x.NewObject("$Get{o.OriginalName}CommandSource", x.This(), x.This())),
//                    cmd.SubCommands.Select(o => MethodCall($"Command.AddCommand", o.OriginalName)),
//                    Assign("CommandHandler", MethodCall("CommandHandler.Create", CreateLambda())));
//            return newCtor;

//            static Expression CreateLambda()
//            {
//                return Expression.MultilineLambda()
//                    .Parameter("InvocationContext", "context")
//                    .Statements(
//                        Assign("CurrentCommandSource", This()),
//                        Assign("CurrentParseResult", "context.ParseResult"))
//                    .Return("0");
//            }
//        }

//        private Constructor GetRootCtor2(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
//        {
//            Constructor newCtor = new Constructor()
//                .BaseCall(Expression.NewObject("Command", cmd.CliName))
//                .Statements(cmd.GetOptionsAndArgs(),
//                     o => x.Assign(o.OriginalName, x.MethodCall("$Get{o.OriginalName}")),
//                     o => MethodCall($"Command.Add", o.OriginalName))
//                .Statements(cmd.SubCommands,
//                     o => x.Assign(o.OriginalName, x.NewObject("$Get{o.OriginalName}CommandSource", x.This(), x.This())),
//                     o => MethodCall($"Command.AddCommand", o.OriginalName))
//                .Statement(x.Assign("CommandHandler", MethodCall("CommandHandler.Create", CreateLambda())));
//            return newCtor;

//            static Expression CreateLambda()
//            {
//                return Expression.MultilineLambda()
//                    .Parameter("InvocationContext", "context")
//                    .Statements(
//                        x.Assign("CurrentCommandSource", x.This()),
//                        x.Assign("CurrentParseResult", "context.ParseResult"))
//                    .Return("0");
//            }
//        }
//    }
//}

//namespace GeneratorSupport.Tests.Variation2
//{
//    using x = Expression;

//    public class Generator
//    {
//        // 
//        public Code CreateCode(CliDescriptor cli)
//        {
//            var cmd = cli.CommandDescriptor;
//            var baseClass = "";

//            return new Code(cli.GeneratedComandSourceNamespace)
//            {
//                Usings = new[]
//                         {  "StarFruit2",
//                                "System.CommandLine",
//                                "StarFruit2.Common",
//                                "System.CommandLine.Invocation",
//                                "System.CommandLine.Parsing"},
//                Classes = new[]
//                         { new Class($"{cmd.OriginalName}CommandSource")
//                                 {
//                                     Base =baseClass,
//                                     Constructor= GetRootCtor(cmd,null,null),
//                                     Members = new[]
//                                     {
//                                         cmd.Options.Select(x => x.NewProperty()),
//                                         cmd.Arguments.Select(x => x.NewProperty()),
//                                         cmd.Options.Select(x => GetOptionMethod(x)),
//                                         cmd.Arguments.Select(x => GetArgumentMethod(x))
//                                      },
//                                 }
//                             }
//            };
//        }

//        private Constructor GetRootCtor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
//        {
//            Constructor newCtor = new Constructor()
//            {
//                BaseCall = x.NewObject("Command", cmd.CliName),
//                Statements = new StatementBlock()
//                     // I like this combined call better than the others, but I am not crazy about AddRange as a name
//                     .AddRange(cmd.GetOptionsAndArgs(),
//                               o => x.Assign(o.OriginalName, x.MethodCall("$Get{o.OriginalName}")),
//                               o => x.MethodCall($"Command.Add", o.OriginalName))
//                     .AddRange(cmd.SubCommands,
//                               o => x.Assign(o.OriginalName, x.NewObject("$Get{o.OriginalName}CommandSource", x.This(), x.This())),
//                               o => x.MethodCall($"Command.AddCommand", o.OriginalName))
//                    // I think the following line is key. We could make .Assign work (without Add), but then the MethodCall 
//                    // needs a way to access the statement block, or it is on a different class, which can work, but looks weird
//                    .Add(x.Assign("CommandHandler", x.MethodCall("CommandHandler.Create", CreateLambda())))
//            };
//            return newCtor;

//            static Expression CreateLambda()
//            {
//                return new MultilineLambda()
//                {
//                    // I'm not crazy about the combination of arrays and special types
//                    Parameters = new[] { new Parameter("InvocationContext", new TypeRep("context")) },
//                    Statements = new StatementBlock()
//                       .Add(x.Assign("CurrentCommandSource", x.This()))
//                       .Add(x.Assign("CurrentParseResult", "context.ParseResult")),
//                    Return = "0"
//                };
//            }
//        }
//    }

//    public class StatementBlock
//    {
//        private List<Expression> statements = new List<Expression>();

//        public StatementBlock AddRange<T>(IEnumerable<T> values, params Func<T, Expression>[] statementFunc)
//        {

//            return this;
//        }

//        public NewObject NewObject(string typeRep, params Expression[] arguments)
//        {
//            return Expression.NewObject(typeRep, arguments);
//        }

//        public NewObject NewObject(TypeRep typeRep, params Expression[] arguments)
//        {
//            return Expression.NewObject(typeRep, arguments);
//        }


//        public NewObject NewObject(string typeRep, params string[] arguments)
//        {
//            return Expression.NewObject(typeRep, arguments);
//        }

//        public MethodCall MethodCall(string name, params Expression[] arguments)
//        {
//            return Expression.MethodCall(name, arguments);
//        }

//        public MethodCall MethodCall(string name, params string[] arguments)
//        {
//            return Expression.MethodCall(name, arguments);
//        }

//        public MethodCall MethodCall(string name)
//        {
//            return Expression.MethodCall(name);
//        }

//        public Assign Assign(string leftHand, Expression rightHand)
//        {
//            return Expression.Assign(leftHand, rightHand);
//        }

//        public Assign Assign(string leftHand, string rightHand)
//        {
//            return Expression.Assign(leftHand, rightHand);
//        }

//        public Expression This()
//        {
//            return Expression.This();
//        }

//        public Expression Base()
//        {
//            return Expression.Base();
//        }

//        public MultilineLambda MultilineLambda()
//        {
//            return statements.Add(Expression.MultilineLambda();
//        }

//        internal StatementBlock Add(object p)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class Code
//    {

//        public Code(string nspace)
//        {
//            Namespace = nspace;
//        }
//        public string Namespace { get; }

//        public string[] Usings { get; set; }
//        public Class[] Classes { get; set; }
//    }

//    public class Class
//    {
//        public Class(string name)
//        {
//            Name = name;
//        }

//        public string Name { get; }
//        public string Base { get; internal set; }
//        public Constructor Constructor { get; internal set; }
//        public Member[] Members { get; internal set; }
//    }

//    public class Constructor : Member
//    {
//        public MethodCall BaseCall { get; internal set; }
//        public StatementBlock Statements { get; internal set; }
//    }

//    public class Member
//    {

//    }

//    public class MultilineLambda : IExpression
//    {

//        public MultilineLambda()
//        {
//        }

//        public IEnumerable<Parameter> Parameters { get; set; }

//        public StatementBlock Statements { get; set; }

//        public Expression Return { get; set; }


//    }

//}
