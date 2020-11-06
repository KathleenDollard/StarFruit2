using StarFruit.Common;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using StarFruit2.Generator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2
{
    public class CodeGeneratorTemp
    {
        private string[] Libraries = new string[]
        {
            "StarFruit2",
            "System.CommandLine",
            "StarFruit2.Common",
            "System.CommandLine.Invocation",
            "System.CommandLine.Parsing"
        };

        private Generate generate;


        public CodeGeneratorTemp(Generate generate)
        {
            this.generate = generate;
        }

        public List<string> GenerateSourceCode(CliDescriptor cliDescriptor)
        {
            List<string> strCollection = new List<string>();
            strCollection.AddRange(generate.Usings(Libraries));
            strCollection.AddRange(generate.Namespace(cliDescriptor.GeneratedComandSourceNamespace,
                                                      GetNamespaceBody(cliDescriptor)));

            return strCollection;
        }

        private List<string> GetNamespaceBody(CliDescriptor cliDescriptor)
            => GetClasses(cliDescriptor.CommandDescriptor);

        private List<string> GetClasses(CommandDescriptor commandDescriptor)
        {
            var rootCommandClass = GetClass(commandDescriptor);
            var subCommandClasses = commandDescriptor.SubCommands.SelectMany(subCmd => GetClass(subCmd));

            return rootCommandClass.Union(subCommandClasses).ToList();
        }

        private List<string> GetClass(CommandDescriptor cmd)
        {
            var className = $"{cmd.OriginalName}CommandSource";
            var baseClassName = cmd.IsRoot ? generate.GenericType("RootCommandSource", cmd.OriginalName) : "CommandSource";

            return generate.Class(scope: Scope.Public,
                                  isPartial: true,
                                  className: className,
                                  baseClassName: baseClassName,
                                  classBody: GetClassBody(cmd, className));
        }

        private List<string> GetClassBody(CommandDescriptor commandDescriptor, string className)
        {
            List<string> strCollection = new List<string>();

            if (!commandDescriptor.IsRoot)
            {
                strCollection.AddRange(GetFields(commandDescriptor));
            }

            strCollection.AddRange(GetConstructor(commandDescriptor, className));

            // WE WANT THEM ON ALL FOR NOW, CommandSourceResult will deal with it potentially being a trivial call
            strCollection.AddRange(GetCommandSourceResultMethod(commandDescriptor));
            strCollection.AddRange(GetProperties(commandDescriptor));
            strCollection.AddRange(GetOptAndArgMethods(commandDescriptor));

            return strCollection;
        }

        private IEnumerable<string> GetConstructor(CommandDescriptor commandDescriptor, string className)
        {
            var strCollection = new List<string> { };
            List<string>? ctorArgs = null;
            string rootStr;
            string? parentStr;

            if (commandDescriptor.IsRoot)
            {
                rootStr = generate.This;
                parentStr = generate.This;
            }
            else
            {
                rootStr = $"{generate.This}.root";
                parentStr = $"{generate.This}.parent";
                ctorArgs = new List<string>
                {
                    generate.Parameter($"{commandDescriptor.Root.Name}CommandSource", "root"),
                    generate.Parameter($"{commandDescriptor.ParentSymbolDescriptorBase.Name}CommandSource", "parent")
                };

                strCollection.Add(generate.Assign($"{generate.This}.parent", "parent"));
            }

            // add options and args
            var optsAndArgs = commandDescriptor.Arguments.OfType<SymbolDescriptor>().Union(commandDescriptor.Options);
            strCollection.AddRange(optsAndArgs.SelectMany(elem => AssignAndAdd(elem, generate)));

            // add commands
            strCollection.AddRange(commandDescriptor.SubCommands.SelectMany(subCmd => AddSubCommand(subCmd, generate)));

            // add command handler
            strCollection.Add(GetCommandHandler(commandDescriptor.IsRoot));

            var baseArgs = new List<string> {
                generate.NewCommand(commandDescriptor.CliName, commandDescriptor.Description)
            };

            return generate.Constructor(className: className,
                                        ctorArgs: ctorArgs,
                                        baseArgs: baseArgs,
                                        ctorBody: strCollection);

            static List<string> AssignAndAdd(SymbolDescriptor symbol, Generate generate)
            {
                var name = symbol.OriginalName;
                return new List<string> {
                    // StringProperty = GetStringProperty();
                    generate.Assign(name, generate.MethodCall($"Get{name}")),
                    // Command.Add(StringProperty);
                    generate.AddToCommand("Add", name)
                };
            }

            List<string> AddSubCommand(CommandDescriptor subCmd, Generate generate)
            {
                var commandArgs = new List<string> { rootStr, parentStr };
                var newObject = generate.NewObject($"{subCmd.Name}CommandSource", commandArgs);
                var commandAssignment = generate.Assign(subCmd.Name, newObject);

                return new List<string>
                {
                    commandAssignment,
                    generate.AddToCommand("AddCommand", $"{subCmd.Name}.Command")
                };
            }

            string GetCommandHandler(bool isRoot)
            {
                var commandSource = isRoot ? "CurrentCommandSource" : "root.CurrentCommandSource";
                var handlerLambdaStatements = new List<string>
                {
                    generate.Assign(commandSource, generate.This),
                     generate.Return(generate.This)
                };
                var handlerLambda = generate.Lambda(null, handlerLambdaStatements);
                var commandHandler = generate.MethodCall("CommandHandler.Create", handlerLambda);
                return generate.Assign($"Command.Handler", commandHandler);
            }
        }

        private IEnumerable<string> GetCommandSourceResultMethod(CommandDescriptor commandDescriptor)
        {
            // not sure if that is the right name here, want just Find
            var cmdSourceObj = generate.NewObject($"{commandDescriptor.Name}CommandSourceResult", "parseResult", generate.This);
            var methodBody = new List<string>
            {
                generate.Return(cmdSourceObj)
            };

            return generate.Method(scope: Scope.Protected,
                                   name: $"GetCommandSourceResult",
                                   methodBody: methodBody,
                                   returnType: "CommandSourceResult",
                                   isAsync: false,
                                   isOverriden: true,
                                   arguments: generate.Parameter("ParseResult", "parseResult"));
        }

        private IEnumerable<string> GetFields(CommandDescriptor commandDescriptor)
            => new List<string>
            {
                generate.Field(Scope.Internal, commandDescriptor.ParentSymbolDescriptorBase.Name, "parent")
            };

        private List<string> GetProperties(CommandDescriptor commandDescriptor)
        {
            List<string> strCollection = new List<string>();
            // add top level args/opts

            strCollection.AddRange(commandDescriptor.Arguments
                                                   .Where(arg => !arg.IsHidden)
                                                   .Select(arg => GetArgument(arg, commandDescriptor))
                                                   .ToList());

            strCollection.AddRange(commandDescriptor.Options
                                                   .Where(opt => !opt.IsHidden)
                                                   .Select(opt => GetOption(opt, commandDescriptor))
                                                   .ToList());

            // TODO: not done, haven't handled adding parent props, see 86-87 in .gen

            return strCollection;
        }

        private IEnumerable<string> GetOptAndArgMethods(CommandDescriptor cmd)
        {
            var optionMethods = cmd.Options.SelectMany(opt => GetOptionMethod(cmd, opt));
            var argumentMethods = cmd.Arguments.SelectMany(arg => GetArgMethod(cmd, arg));

            return optionMethods.Union(argumentMethods);
        }


        private string GetArgument(ArgumentDescriptor arg, CommandDescriptor commandDescriptor)
        {
            Scope scope = Scope.Public;
            return generate.Property(
                scope: scope,
                type: generate.GenericType("Argument", arg.ArgumentType.TypeAsString()),
                // need to properly handle top level prop names (i.e. no command prefix)
                name: arg.Name,
                setterScope: scope
            );
        }

        private string GetOption(OptionDescriptor opt, CommandDescriptor commandDescriptor)
        {
            Scope scope = Scope.Public;
            return generate.Property(
                scope: scope,
                // this still feels not quite right w/ the .First
                type: generate.GenericType("Option", opt.Arguments.First().ArgumentType.TypeAsString()),
                name: opt.Name,
                setterScope: scope
            );
        }

        private IEnumerable<string> GetOptionMethod(CommandDescriptor cmd, OptionDescriptor opt)
        {
            var methodBody = new List<string> { };
            var optionArg = opt.Arguments.First();

            // var option = ...
            var type = optionArg.ArgumentType.TypeAsString();
            var optType = generate.GenericType("Option", type);
            var args = new List<string> { opt.CliName };
            var assignments = new List<string> {
                generate.Assign("Description", opt.Description),
                // TODO: we have no faith in passing the bool this way, come back and check
                generate.Assign("IsRequired", opt.Required.ToString()),
                generate.Assign("IsHidden", opt.IsHidden.ToString())
            };
            var optObject = generate.NewObjectWithInit(optType, args, assignments);
            methodBody.AddRange(generate.Assign($"{generate.Var} option", optObject));

            var optArgName = $"{generate.Var} {cmd.Name}_{opt.OriginalName}_arg";
            var optArgObj = generate.NewObject(generate.GenericType("Argument", type), optionArg.CliName);
            methodBody.Add(generate.Assign(optArgName, optArgObj));
            // TODO: must add default value logic

            methodBody.Add(generate.Assign("option.Argument", optArgName));

            // TODO: might blow up if aliases aren't there
            methodBody.AddRange(opt.Aliases.Select(alias => generate.MethodCall("option.AddAlias", alias)));

            if (!(optionArg.DefaultValue is null))
            {
                methodBody.Add(GetArgumentDefaultValue(optionArg));
            }

            methodBody.Add(generate.Return("option"));

            return generate.Method(scope: Scope.Private,
                                   name: $"Get{opt.OriginalName}",
                                   methodBody: methodBody,
                                   returnType: optType);
        }

        private IEnumerable<string> GetArgMethod(CommandDescriptor cmd, ArgumentDescriptor arg)
        {
            var methodBody = new List<string> { };
            var argType = generate.GenericType("Argument", arg.ArgumentType.TypeAsString());
            var args = new List<string> { arg.CliName };
            var assignments = new List<string> {
                generate.Assign("Description", arg.Description),
                generate.Assign("IsHidden", arg.IsHidden.ToString())
            };

            if (!(arg.Arity is null))
            {
                var arityObject = generate.NewObject("ArgumentArity",
                                                     arg.Arity.MinimumCount.ToString(),
                                                     arg.Arity.MaximumCount.ToString());
                var arityAssignment = generate.Assign("Arity", arityObject);

                assignments.Add(arityAssignment);
            }

            var optObject = generate.NewObjectWithInit(argType, args, assignments);

            methodBody.AddRange(generate.Assign($"{generate.Var} argument", optObject));

            if (!(arg.DefaultValue is null))
            {
                methodBody.Add(GetArgumentDefaultValue(arg));
            }

            methodBody.Add(generate.Return("argument"));

            return generate.Method(scope: Scope.Private,
                                   name: $"Get{arg.OriginalName}",
                                   methodBody: methodBody,
                                   returnType: argType);
        }

        private string GetArgumentDefaultValue(ArgumentDescriptor arg)
        {

            return generate.MethodCall("argument.SetDefaultValue", arg.DefaultValue.CodeRepresentation);
        }


        //// TODO: this works for 2 layer, does this break for 1 or multi layer?
        //private IEnumerable<string> GetInvokeMethods(CommandDescriptor commandDescriptor)
        //    => commandDescriptor.SubCommands.SelectMany(cmd => GetInvokeMethod(cmd));

        //private List<string> GetInvokeMethod(CommandDescriptor cmd)
        //{
        //    // "NewInstance = GetNewInstance(bindingContext)";
        //    var methodBody = new List<string>
        //    {
        //        generate.SetNewInstance,
        //    };

        //    // see two layer gen #36-38, unsure best vert whitespace management here
        //    // fetch all the GetValue calls
        //    var methodArgs = cmd.Arguments.Where(arg => arg.CodeElement == CodeElement.MethodParameter);
        //    var methodOptions = cmd.Options.Where(opt => opt.CodeElement == CodeElement.MethodParameter);
        //    var cmdValues = string.Join(", ", GetCommandValues(cmd.Name, methodArgs, methodOptions));

        //    // original name may fail on 1 layer
        //    // push this to generate, since it's C#. generate.Return(body, isAsync)
        //    methodBody.Add(generate.Return($"NewInstance.{cmd.OriginalName}({cmdValues})", true));

        //    // this will fall over with compound generic types, deal with that if encountered
        //    return generate.Method(scope: Scope.Public,
        //                           name: NameForInvokeCommand(cmd.Name),
        //                           methodBody: methodBody,
        //                           returnType: generate.GenericType("Task", "int"),
        //                           isAsync: cmd.IsAsync,
        //                           isOverriden: false,
        //                           arguments: generate.Parameter("BindingContext", "bindingContext"));
        //}
        //
        //private List<string> GetCommandValues(string cmdName, IEnumerable<ArgumentDescriptor> args, IEnumerable<OptionDescriptor> options)
        //{
        //    List<string> strCollection = new List<string> { };

        //    // unsure on original name vs name here
        //    //var optsAndArgs = args.OfType<SymbolDescriptor>().Union(options);
        //    // grrr, this only works if SymbolDescriptors have code elements, and they shouldn't. Maybe a shared base class for opts and args?
        //    // same issue for position
        //    //var values = optsAndArgs.Where(elem => elem.CodeElement == CodeElement.MethodParameter).Select(elem => generate.GetValueMethod(NameForProperty(cmdName, elem.Name)));
        //    //strCollection.AddRange(values);

        //    var combo = args.OfType<SymbolDescriptor>().Union(options).OrderBy(elem => elem.Position).Select(elem => generate.GetValueMethod(NameForProperty(cmdName, elem.Name)));
        //    //var foo = args.Select(arg => (name: arg.Name, position: arg.Position));
        //    //var bizz = options.Select(opt => (name: opt.Name, position: opt.Position));
        //    //var combo = foo.Union(bizz).OrderBy(elem => elem.position).Select(elem => elem.name);

        //    strCollection.AddRange(combo);

        //    //var argValues = args.Select(arg => generate.GetValueMethod(NameForProperty(cmdName, arg.Name)));
        //    //var optValues = options.Select(opt => generate.GetValueMethod(NameForProperty(cmdName, opt.Name)));
        //    //strCollection.AddRange(argValues);
        //    //strCollection.AddRange(optValues);

        //    return strCollection;
        //}

        //private IEnumerable<string> GetCommandMethods(CommandDescriptor commandDescriptor)
        //{
        //    var strCollection = GetCommandMethod(commandDescriptor);
        //    strCollection.AddRange(commandDescriptor.SubCommands.SelectMany(cmd => GetCommandMethod(cmd)));

        //    return strCollection;
        //}

        //private List<string> GetCommandMethod(CommandDescriptor cmd)
        //{
        //    List<string> methodBody = new List<string> { };

        //    // if this is VB unfriendly, push down to generate, might also push the get arg def down there as well
        //    // TODO: finalize how to handle arg declaration
        //    var argDeclarations =
        //        cmd.Arguments.Select
        //        (
        //            arg => generate.Assign
        //            (
        //                NameForProperty(arg.OriginalName, cmd.Name),
        //                generate.GetArg(arg.CliName, arg.ArgumentType.TypeAsString(), arg.Description)
        //            ).EndStatement()
        //        );

        //    // do samething for option

        //    methodBody.AddRange(argDeclarations);

        //    // throw all the args and opts into the new command (line 53-58), subcommands would come here as well

        //    methodBody.AddRange(NewCommand(cmd));

        //    methodBody.Add(generate.Assign($"{NameForCommand(cmd.Name)}.Handler", generate.NewCommandHandler(cmd.Name)));


        //    methodBody.Add(generate.Return(NameForCommand(cmd.Name)));

        //    return generate.Method(Scope.Private,
        //                           NameForGetCommand(cmd.Name),
        //                           methodBody,
        //                           "Command");
        //}

        //private List<string> NewCommand(CommandDescriptor cmd)
        //{
        //    var strCollection = new List<string> { };

        //    strCollection.AddRange(cmd.Arguments.Select(arg => $"{NameForProperty(cmd.Name, arg.OriginalName)},"));
        //    strCollection.AddRange(cmd.Arguments.Select(opt => $"{NameForProperty(cmd.Name, opt.OriginalName)},"));

        //    // scrap BuildBlock for now, instead build like 2 layer gen line 83
        //    return generate.BuildBlock(generate.Assign(NameForCommand(cmd.Name), generate.NewCommand(cmd.CliName)), strCollection);
        //}

        //private IEnumerable<string> GetNewInstanceMethod(CommandDescriptor commandDescriptor)
        //{
        //    // same problem as invoke: base class doesn't have code element :/

        //    var ctorParamOpts = commandDescriptor.Options.Where(opt => opt.CodeElement == CodeElement.CtorParameter);
        //    var ctorParamArgs = commandDescriptor.Arguments.Where(arg => arg.CodeElement == CodeElement.CtorParameter);
        //    // union, then filter, then order, then take name
        //    var ctorParamNames = ctorParamArgs.OfType<SymbolDescriptor>().Union(ctorParamOpts).Select(param => param.Name);
        //    // placeholder, must fetch those passed as ctor properties
        //    var PropertyOpts = commandDescriptor.Options.Where(opt => opt.CodeElement == CodeElement.Property);
        //    var PropertyArgs = commandDescriptor.Arguments.Where(arg => arg.CodeElement == CodeElement.Property);
        //    var PropertyNames = PropertyOpts.OfType<SymbolDescriptor>().Union(PropertyArgs).Select(prop => prop.Name);

        //    var ctorParams = ctorParamNames.Select(param => generate.GetValueMethod(param));
        //    //var ctorOptsAndArgs = new List<IEnumerable<string>> { ctorParamOpts, ctorParamArgs };
        //    //var ctorParams = ctorOptsAndArgs.SelectMany(elem => elem).ToList();

        //    var initProps = PropertyNames.Select(prop => generate.Assign(prop, generate.GetValueMethod(prop)));

        //    //var PropertyOptsAndArgs = new List<IEnumerable<string>> { ctorParamOpts, ctorParamArgs };
        //    //var initProperties = PropertyOptsAndArgs.SelectMany(elem => elem).ToList();

        //    var methodBody = generate.Return(generate.NewObjectWithInit("CliRoot", ctorParams, initProps));
        //    // add a version of return that takes a list and returns a list
        //    //var methodBody = generate.Return(generate.NewCliRoot(ctorParams, initProperties));

        //    return generate.Method(Scope.Private,
        //                           "GetNewInstance",
        //                           methodBody,
        //                           "CliRoot",
        //                           arguments: generate.Parameter("BindingContext", "bindingContext"));
        ////}


        //private string NameForGetCommand(string cmdName)
        //    => $"Get{cmdName}Command";

        //private string NameForInvokeCommand(string cmdName)
        //    => $"Invoke{cmdName}Async";

        //private string NameForCommand(string cmdName)
        //    => $"{cmdName}Command";

        //private string NameForProperty(string cmdName, string name)
        //    => cmdName == "" ? name : $"{cmdName}_{name}";
    }
}
