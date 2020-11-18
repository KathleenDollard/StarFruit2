
using GeneratorSupport.Context.CSharp;
using GeneratorSupport.SimpleDom;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratorSupport.Tests
{
    public class UnitTest1
    {

        public void Test2()
        {
            var generate = new GenerationEntryContextCSharp();
            var cmd = new CommandDescriptor(null, "Fizz", null);

            var dom = new Entry()
            {
                NamespaceName = "StarFruit2",
            };
            dom.Usings.AddRange(new string[]
            {
                "StarFruit2",
                "System.CommandLine",
                "StarFruit2.Common",
                "System.CommandLine.Invocation",
                "System.CommandLine.Parsing",
            });

            dom.Classes.Add(GetRootCommandSourceClass(cmd));
            dom.Classes.AddRange(cmd.SubCommands.Select(s => GetCommandSourceClass(s)));

        }

        private static Class GetCommandSourceClass(CommandDescriptor s)
        {
            return GetCommandSourceClass(s, new TypeRep("CommandSource"));
        }

        private static Class GetCommandSourceClass(CommandDescriptor cmd, TypeRep baseClass)
        {
            var commandSouceClass = new Class($"{cmd.OriginalName}CommandSource")
            {
                BaseClass = baseClass,
            };
  
            commandSouceClass.Members.Add(GetConstructor(cmd,cmd.Root, cmd.ParentSymbolDescriptorBase  ));
            commandSouceClass.Members.AddRange(cmd.Options.Select(x => x.NewProperty()));
            commandSouceClass.Members.AddRange(cmd.Arguments.Select(x => x.NewProperty()));
            commandSouceClass.Members.AddRange(cmd.Options.Select(x => GetOptionMethod(x)));
            commandSouceClass.Members.AddRange(cmd.Arguments.Select(x => GetArgumentMethod(x)));

            return commandSouceClass;
        }


        private static string SymbolPropertyName(string name)
        => name;

        private static string GetSymbolMethodName(string name)
        => $"Get{name}";

        private static Member GetConstructor(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
        {
            var ctor = new Constructor();
            ctor.Parameters.AddRange(GetConstructorParameters(cmd));
            ctor.BaseConstructorArguments.AddRange(GetBaseArguments(cmd));

            ctor.Statements.Add(new Assignment("parent", "parent", BaseOrThis.This));
            ctor.Statements.AddRange(cmd.Options.Select(x => new Assignment(x.Name, new MethodCall(GetSymbolMethodName(x.Name)))));
            ctor.Statements.AddRange(cmd.Arguments.Select(x => new Assignment(x.Name, new MethodCall(GetSymbolMethodName(x.Name)))));
            ctor.Statements.AddRange(cmd.SubCommands.Select(x => new Assignment(x.Name, new MethodCall(GetSymbolMethodName(x.Name)))));
            ctor.Statements.AddRange(cmd.Options.Select(x => new MethodCall("Command.AddOption", GetSymbolMethodName(x.Name))));
            ctor.Statements.AddRange(cmd.Arguments.Select(x => new MethodCall("Command.AddArgument", GetSymbolMethodName(x.Name))));
            ctor.Statements.AddRange(cmd.SubCommands.Select(x => new MethodCall("Command.AddCommand", $"{GetSymbolMethodName(x.Name)}")));
            ctor.Statements.Add(GetCommandHandler(cmd, root, parent));
            return ctor;

            static IEnumerable<Parameter> GetConstructorParameters(CommandDescriptor cmd)
            {
                return !cmd.IsRoot
                        ? null
                        : new Parameter[]
                        {
                            new Parameter($"{cmd.Root.OriginalName}CommandSource", "root"),
                            new Parameter($"{cmd.ParentSymbolDescriptorBase.OriginalName}CommandSource", "parent")
                        };
            }
  
            static IEnumerable<Expression> GetBaseArguments(CommandDescriptor cmd)
            => new Expression[]
                {
                    new NewObject (new TypeRep("Command"), cmd.CliName,cmd.Description )
                };

            static Expression GetCommandHandler(CommandDescriptor cmd, CommandDescriptor root, ISymbolDescriptor parent)
            {
                var statements = new List<Expression>
                {
                    new Assignment("CurrentCommandSource", This),
                    new Assignment("CurrentParseResult", "context.ParseResult"),
                    new Return("0")
                };
                var lambda = new MultilineLambda(null, statements);
                var handler = new MethodCall("CommandHandler.Create", lambda);
                return new Assignment("Command.Handler", handler);
            }

        }

        private static Method GetArgumentMethod(ArgumentDescriptor argument)
        {
            var method = new Method($"Get{Helpers.GetArgumentPropertyName(argument.Name)}");
            NewObject newObject = new NewObject(new TypeRep("Argument", new TypeRep(argument.ArgumentType.TypeAsString())));
            newObject.Arguments.Add(argument.CliName);
            method.Statements.Add(new LocalDeclaration("symbol", TypeRep.Var, newObject));
            method.Statements.Add(new Assignment("symbol.Description", new Direct(argument.Description)));
            method.Statements.Add(new Assignment("symbol.IsHidden", new Direct(argument.IsHidden.ToString())));
            if (argument.DefaultValue is not null)
            {
                method.Statements.Add(new MethodCall("symbol.SetDefaultValue", argument.DefaultValue.DefaultValue.ToString()));
            }
            method.Statements.Add(new Return("symbol"));

            return method;
        }

        private static Method GetOptionMethod(OptionDescriptor option)
        {
            var method = new Method($"Get{Helpers.GetOptionPropertyName(option.Name)}");
            NewObject newObject = new NewObject(new TypeRep("Option", new TypeRep(option.Arguments.First().ArgumentType.TypeAsString())));
            newObject.Arguments.Add(option.CliName);
            method.Statements.Add(new LocalDeclaration("symbol", TypeRep.Var, newObject));
            method.Statements.Add(new Assignment("symbol.Description", new Direct(option.Description)));
            method.Statements.Add(new Assignment("symbol.IsHidden", new Direct(option.IsHidden.ToString())));
            method.Statements.Add(new Assignment("symbol.IsRequired", new Direct(option.Required.ToString())));

            if (option.Arguments.First().DefaultValue is not null)
            {
                method.Statements.Add(new MethodCall("symbol.Arguments.First().SetDefaultValue", option.Arguments.First().DefaultValue.DefaultValue.ToString()));
            }

            foreach(var alias in option.Aliases )
            {
                method.Statements.Add(new MethodCall("symbol.AddAlias", alias));
            }
            method.Statements.Add(new Return("symbol"));

            return method;
        }

        private static Class GetRootCommandSourceClass(CommandDescriptor cmd)
        {
            TypeRep baseTypeRepresentation = cmd.IsRoot ? new TypeRep("RootCommandSource", cmd.OriginalName) : new TypeRep("CommandSource");
            var commandSouceClass = GetCommandSourceClass(cmd, baseTypeRepresentation);
            if (!cmd.IsRoot)
            {
                commandSouceClass.Members.Add(new Field()
                {
                    Name = "parent",
                    TypeRepresentation = new TypeRep(cmd.ParentSymbolDescriptorBase.Name) ,
                    Scope = Scope.Internal
                });
            }
            return commandSouceClass;
        }


    }
}
