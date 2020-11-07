using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GeneratorSupport.Context.CSharp;

namespace GeneratorSupport.Tests
{
    public class UnitTest1
    {
        //[Fact]
        //public void Test1()
        //{
        //    var generate = new GenerateCSharp();

        //    generate.ClassDeclaration("Fizz", Scope.Public)
        //            .Base("Buzz")
        //            .Interfaces("IFizz", "IBuzz")
        //            .Body()
        //            .EndClass();
        //}

        [Fact]
        public void Test2()
        {
            var generate = new GenerationEntryContextCSharp();
            var cmd = new CommandDescriptor("Fizz", true);
            var usings = new string[]
            {
                "StarFruit2",
                "System.CommandLine",
                "StarFruit2.Common",
                "System.CommandLine.Invocation",
                "System.CommandLine.Parsing"
            };

            generate.Preamble()
                    .Usings(usings);
            var genNamespace = generate.Namespace("StarFruit2")
                             .StartBody();
            var genClass = generate.Class($"{cmd.OriginalName}CommandSource")
                            .SetBaseClassName(cmd.IsRoot ? generate.GenericType("RootCommandSource", cmd.OriginalName) : "CommandSource")
                            .StartBody();
            if (!cmd.IsRoot)
            {
                genClass.Field("parent",cmd.ParentSymbolDescriptorBase.Name,Scope.Internal);
            }

            genClass.EndBody();
            genNamespace.EndBody();
        }


        //        [Fact]
        //        public void Test3()
        //        {
        //            var generate = new GenerateCSharp();
        //            var cmd = new CommandDescriptor("Fizz", true);

        //            var className = $"{cmd.OriginalName}CommandSource";
        //            var baseClassName = cmd.IsRoot ? generate.GenericType("RootCommandSource", cmd.OriginalName) : "CommandSource";

        //            var classGen = generate.Class(className,
        //                                          scope: Scope.Public,
        //                                          isPartial: true)
        //                                   .Base(baseClassName)
        //                                   .Interfaces()  //Not needed,just to show options
        //                                   .Fields(cmd.IsRoot ? null : generate.Field(Scope.Internal, cmd.ParentSymbolDescriptorBase.Name, "parent"))
        //                                   .Constructor(className, cmd)
        //                                   .AddMethods(GetCommandSourceResultMethod(cmd))
        //                                   .AddProperties(GetProperties(cmd))
        //                                   .AddMethods(GetOptAndArgMethods(cmd))
        //                                   .OutputClass();

        //            static string[] GetConstructorBody(GenerateCSharp generate, CommandDescriptor cmd)
        //            {
        //                var strCollection = new List<string> { };
        //                var optsAndArgs = cmd.Arguments.OfType<SymbolDescriptor>().Union(cmd.Options);
        //                strCollection.AddRange(optsAndArgs.Select(elem =>
        //                                generate.Assign(elem.OriginalName, generate.MethodCall($"Get{elem.OriginalName}"))));
        //                strCollection.AddRange(optsAndArgs.Select(elem =>
        //                                generate.MethodCall("Command.Add",
        //                                                     new string[] { elem.OriginalName })));

        //                //// add commands
        //                //strCollection.AddRange(cmd.SubCommands.SelectMany(subCmd => AddSubCommand(subCmd, generate)));

        //                //// add command handler
        //                //strCollection.Add(GetCommandHandler(cmd.IsRoot));

        //                return strCollection.ToArray();
        //            }
        //        }
        //    }

        //    public static class StarFruitSourceGen
        //    {
        //        public static ClassGen Constructor(this ClassGen classGen, string className, CommandDescriptor cmd)
        //        {
        //            var generate = classGen.Generate;
        //            var ctorParams = GetConstructorParameters(generate, cmd);
        //            var constructorGen = classGen.ConstructorDeclaration(className, Scope.Public, ctorParams)
        //                                         .ConstructorBase(generate.NewObject(cmd.CliName, cmd.Description))
        //                                         .ConstructorBodyStart();
        //            if (!cmd.IsRoot)
        //            {
        //                generate.AssignStatement($"{generate.This}.parent", "parent");
        //            }

        //            OptionsAndArguments(cmd, generate);
        //            SubCommands(cmd, generate);
        //            CommandHandler(cmd, generate);

        //            var baseArgs = new List<string> {
        //                generate.NewCommand(commandDescriptor.CliName, commandDescriptor.Description)
        //            };

        //                               .ConstructorStatements(GetConstructorBody(generate, cmd))
        //                               ;
        //            constructorGen.ConstructorBodyEnd();
        //            return classGen.EndConstructor();

        //            static string[] GetConstructorParameters(Generate generate, CommandDescriptor cmd)
        //            {
        //                return !cmd.IsRoot
        //                        ? null
        //                        : new string[]
        //                        {
        //                            generate.Parameter($"{cmd.Root.OriginalName}CommandSource", "root"),
        //                            generate.Parameter($"{cmd.ParentSymbolDescriptorBase.OriginalName}CommandSource", "parent")
        //                        };
        //            }

        //            static void OptionsAndArguments(CommandDescriptor cmd, Generate generate)
        //            {
        //                var optsAndArgs = cmd.Arguments.OfType<SymbolDescriptor>().Union(cmd.Options);
        //                foreach (var symbol in optsAndArgs)
        //                {
        //                    var name = symbol.OriginalName;
        //                    generate.AssignStatement(name, generate.MethodCall($"Get{name}"));
        //                    generate.MethodCall($"Command.Add", new string[] { name });
        //                }
        //            }

        //            static void SubCommands(CommandDescriptor cmd, Generate generate)
        //            {
        //                foreach (var command in cmd.SubCommands)
        //                {
        //                    var root = cmd.IsRoot
        //                                    ? generate.This
        //                                    : $"{generate.This}.root";
        //                    var parent = cmd.IsRoot
        //                                    ? generate.This
        //                                    : $"{generate.This}.parent";
        //                    var name = command.Name;
        //                    var newObject = generate.NewObject($"{command.Name}CommandSource", new string[] { root, parent });
        //                    generate.Assign(name, newObject);
        //                    generate.MethodCall($"Command.Add", new string[] { name });
        //                }
        //            }

        //            string CommandHandler(CommandDescriptor cmd, Generate generate)
        //            {
        //                var commandSource = cmd.IsRoot ? "CurrentCommandSource" : "root.CurrentCommandSource";
        //                generate.MultiLineLambda(
        //                    generate.LambdaDeclaration(generate.Parameter("InvocationContext", "context")),
        //                    generate.AssignStatement(commandSource, generate.This),
        //                    generate.ReturnStatement(generate.This)
        //                };
        //            var handlerLambda = generate.Lambda(null, handlerLambdaStatements);
        //            var commandHandler = generate.MethodCall("CommandHandler.Create", handlerLambda);
        //            return generate.Assign($"Command.Handler", commandHandler);
        //        }
        //    }
        //}
    }
}
