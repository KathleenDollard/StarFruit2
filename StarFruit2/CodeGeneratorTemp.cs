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
        // HEY KATHLEEN!
        // If you're taking a look, I'm part way through teasing apart the differences between ctors
        // depending on whether it is the root one or not. Relevant methods
        // - GetClassBody
        // - GetConstructor
        // - RootCtorName
        // - NonRootCtorName

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
            var rootCommandClass = GetRootClass(commandDescriptor);
            var subCommandClasses = commandDescriptor.SubCommands.SelectMany(subCmd => GetClass(subCmd));

            return rootCommandClass.Union(subCommandClasses).ToList();
        }

        // This isn't quite enough, since the ctor for the root and non-root are different
        private List<string> GetRootClass(CommandDescriptor cmd) 
            => generate.Class(Scope.Public,
                              true,
                              generate.Inheritance($"{cmd.OriginalName}CommandSource", $"RootCommandSource<{cmd.OriginalName}>"),
                              GetClassBody(cmd, $"{cmd.OriginalName}CommandSource", true));

        private List<string> GetClass(CommandDescriptor cmd) 
            => generate.Class(Scope.Public,
                              true,
                              generate.Inheritance($"{cmd.OriginalName}CommandSource", "CommandSource"),
                              GetClassBody(cmd, $"{cmd.OriginalName}CommandSource", false));

        private List<string> GetClassBody(CommandDescriptor commandDescriptor, string className, bool isRoot)
        {
            List<string> strCollection = new List<string>();
            // TODO: implement GetFields
            strCollection.AddRange(GetFields());
            strCollection.AddRange(GetProperties(commandDescriptor));
            strCollection.AddRange(GetConstructor(commandDescriptor, className, isRoot));


            // consider if these can leverage same underlying Generate method, be mindful of rabbit hole, generated methods can have > 1 param
            strCollection.AddRange(GetInvokeMethods(commandDescriptor)); // <--- gotta get all of them
            strCollection.AddRange(GetCommandMethods(commandDescriptor));


            strCollection.AddRange(GetNewInstanceMethod(commandDescriptor));

            return strCollection;
        }

        private IEnumerable<string> GetFields()
        {
            throw new NotImplementedException();
        }


        // TODO: this works for 2 layer, does this break for 1 or multi layer?
        private IEnumerable<string> GetInvokeMethods(CommandDescriptor commandDescriptor)
            => commandDescriptor.SubCommands.SelectMany(cmd => GetInvokeMethod(cmd));

        private List<string> GetInvokeMethod(CommandDescriptor cmd)
        {
            // "NewInstance = GetNewInstance(bindingContext)";
            var methodBody = new List<string>
            {
                generate.SetNewInstance,
            };

            // see two layer gen #36-38, unsure best vert whitespace management here
            // fetch all the GetValue calls
            var methodArgs = cmd.Arguments.Where(arg => arg.CodeElement == CodeElement.MethodParameter);
            var methodOptions = cmd.Options.Where(opt => opt.CodeElement == CodeElement.MethodParameter);
            var cmdValues = string.Join(", ", GetCommandValues(cmd.Name, methodArgs, methodOptions));

            // original name may fail on 1 layer
            // push this to generate, since it's C#. generate.Return(body, isAsync)
            methodBody.Add(generate.Return($"NewInstance.{cmd.OriginalName}({cmdValues})", true));

            // this will fall over with compound generic types, deal with that if encountered
            return generate.Method(Scope.Public,
                                   NameForInvokeCommand(cmd.Name),
                                   methodBody,
                                   generate.MakeGenericType("Task", "int"),
                                   cmd.IsAsync,
                                   generate.MakeParam("BindingContext", "bindingContext"));
        }

        private List<string> GetCommandValues(string cmdName, IEnumerable<ArgumentDescriptor> args, IEnumerable<OptionDescriptor> options)
        {
            List<string> strCollection = new List<string> { };

            // unsure on original name vs name here
            //var optsAndArgs = args.OfType<SymbolDescriptor>().Union(options);
            // grrr, this only works if SymbolDescriptors have code elements, and they shouldn't. Maybe a shared base class for opts and args?
            // same issue for position
            //var values = optsAndArgs.Where(elem => elem.CodeElement == CodeElement.MethodParameter).Select(elem => generate.GetValueMethod(NameForProperty(cmdName, elem.Name)));
            //strCollection.AddRange(values);

            var combo = args.OfType<SymbolDescriptor>().Union(options).OrderBy(elem => elem.Position).Select(elem => generate.GetValueMethod(NameForProperty(cmdName, elem.Name)));
            //var foo = args.Select(arg => (name: arg.Name, position: arg.Position));
            //var bizz = options.Select(opt => (name: opt.Name, position: opt.Position));
            //var combo = foo.Union(bizz).OrderBy(elem => elem.position).Select(elem => elem.name);

            strCollection.AddRange(combo);

            //var argValues = args.Select(arg => generate.GetValueMethod(NameForProperty(cmdName, arg.Name)));
            //var optValues = options.Select(opt => generate.GetValueMethod(NameForProperty(cmdName, opt.Name)));
            //strCollection.AddRange(argValues);
            //strCollection.AddRange(optValues);

            return strCollection;
        }

        private IEnumerable<string> GetCommandMethods(CommandDescriptor commandDescriptor)
        {
            var strCollection = GetCommandMethod(commandDescriptor);
            strCollection.AddRange(commandDescriptor.SubCommands.SelectMany(cmd => GetCommandMethod(cmd)));

            return strCollection;
        }

        private List<string> GetCommandMethod(CommandDescriptor cmd)
        {
            List<string> methodBody = new List<string> { };

            // if this is VB unfriendly, push down to generate, might also push the get arg def down there as well
            // TODO: finalize how to handle arg declaration
            var argDeclarations =
                cmd.Arguments.Select
                (
                    arg => generate.Assign
                    (
                        NameForProperty(arg.OriginalName, cmd.Name),
                        generate.GetArg(arg.CliName, arg.ArgumentType.TypeAsString(), arg.Description)
                    ).EndStatement()
                );

            // do samething for option

            methodBody.AddRange(argDeclarations);

            // throw all the args and opts into the new command (line 53-58), subcommands would come here as well

            methodBody.AddRange(NewCommand(cmd));

            methodBody.Add(generate.Assign($"{NameForCommand(cmd.Name)}.Handler", generate.NewCommandHandler(cmd.Name)));


            methodBody.Add(generate.Return(NameForCommand(cmd.Name)));

            return generate.Method(Scope.Private,
                                   NameForGetCommand(cmd.Name),
                                   methodBody,
                                   "Command");
        }

        private List<string> NewCommand(CommandDescriptor cmd)
        {
            var strCollection = new List<string> { };

            strCollection.AddRange(cmd.Arguments.Select(arg => $"{NameForProperty(cmd.Name, arg.OriginalName)},"));
            strCollection.AddRange(cmd.Arguments.Select(opt => $"{NameForProperty(cmd.Name, opt.OriginalName)},"));

            // scrap BuildBlock for now, instead build like 2 layer gen line 83
            return generate.BuildBlock(generate.Assign(NameForCommand(cmd.Name), generate.NewCommand(cmd.CliName)), strCollection);
        }

        private IEnumerable<string> GetNewInstanceMethod(CommandDescriptor commandDescriptor)
        {
            // same problem as invoke: base class doesn't have code element :/

            var ctorParamOpts = commandDescriptor.Options.Where(opt => opt.CodeElement == CodeElement.CtorParameter);
            var ctorParamArgs = commandDescriptor.Arguments.Where(arg => arg.CodeElement == CodeElement.CtorParameter);
            // union, then filter, then order, then take name
            var ctorParamNames = ctorParamArgs.OfType<SymbolDescriptor>().Union(ctorParamOpts).Select(param => param.Name);
            // placeholder, must fetch those passed as ctor properties
            var PropertyOpts = commandDescriptor.Options.Where(opt => opt.CodeElement == CodeElement.Property);
            var PropertyArgs = commandDescriptor.Arguments.Where(arg => arg.CodeElement == CodeElement.Property);
            var PropertyNames = PropertyOpts.OfType<SymbolDescriptor>().Union(PropertyArgs).Select(prop => prop.Name);

            var ctorParams = ctorParamNames.Select(param => generate.GetValueMethod(param));
            //var ctorOptsAndArgs = new List<IEnumerable<string>> { ctorParamOpts, ctorParamArgs };
            //var ctorParams = ctorOptsAndArgs.SelectMany(elem => elem).ToList();

            var initProps = PropertyNames.Select(prop => generate.Assign(prop, generate.GetValueMethod(prop)));

            //var PropertyOptsAndArgs = new List<IEnumerable<string>> { ctorParamOpts, ctorParamArgs };
            //var initProperties = PropertyOptsAndArgs.SelectMany(elem => elem).ToList();

            var methodBody = generate.Return(generate.ObjectInit("CliRoot", ctorParams, initProps));
            // add a version of return that takes a list and returns a list
            //var methodBody = generate.Return(generate.NewCliRoot(ctorParams, initProperties));

            return generate.Method(Scope.Private,
                                   "GetNewInstance",
                                   methodBody,
                                   "CliRoot",
                                   arguments: generate.MakeParam("BindingContext", "bindingContext"));
        }

        private IEnumerable<string> GetConstructor(CommandDescriptor commandDescriptor, string className, bool isRoot)
        {
            var strCollection = new List<string> { };

            strCollection.AddRange(commandDescriptor.Options
                        .SelectMany(opt => opt.Arguments.Select(arg => GetCtorOpts(opt, arg))));

            strCollection.AddRange(commandDescriptor.Arguments.Select(arg => GetCtorArg(arg)));

            // go add new arg declarations
            strCollection.AddRange(commandDescriptor.Options.Select(opt => generate.AddToCommand("AddOption", opt.Name)));
            // go add arguments to command
            strCollection.AddRange(commandDescriptor.Arguments.Select(arg => generate.AddToCommand("AddArgument", arg.Name)));
            // go add subcommands to command
            strCollection.AddRange(commandDescriptor.SubCommands.Select(cmd => generate.AddToCommand("AddCommand", $"{NameForGetCommand(cmd.Name)}()")));


            // assume all subcommands are methods. This is bad assumption, they can be classes or methods, but sidestep for now, see multi-layer model

            if (isRoot)
            {
                var ctorName = RootCtorName(className, commandDescriptor.CliName);
                return generate.Constructor(ctorName: ctorName, constructorBody: strCollection);
            } else
            {
                // must fetch this, seems like we have to either pass root along or traverse up parents to root
                var rootClassName = "";
                // must fetch this, seems like we need some sort of .Parent on commandDescriptior
                var parentClassName = "";

                var ctorName = NonRootCtorName(className, rootClassName, parentClassName); ;
                return generate.Constructor(ctorName: ctorName, constructorBody: strCollection);
            }

            string GetCtorOpts(OptionDescriptor opt, ArgumentDescriptor arg)
                => generate.Assign(opt.Name, generate.OptionInitExpression(arg.ArgumentType.TypeAsString(), opt.CliName)).EndStatement();

            string GetCtorArg(ArgumentDescriptor arg)
                => generate.Assign(arg.Name, generate.OptionInitExpression(arg.ArgumentType.TypeAsString(), arg.CliName)).EndStatement();
        }

        private string RootCtorName(string className, string cliName)
        {
            return generate.ConstructorName(className: className,
                                            ctorArgs: null,
                                            baseClassName: generate.Base,
                                            baseCtorArgs: new List<string> { generate.NewCommand(cliName) });
        }

        private string NonRootCtorName(string className, string rootClassName, string parentClassName)
        {
            var rootParam = generate.MakeParam(rootClassName, "root");
            var parentParam = generate.MakeParam(parentClassName, "parent");
            return generate.ConstructorName(className: className,
                                            ctorArgs: new List<string> { rootParam, parentParam },
                                            baseClassName: null,
                                            baseCtorArgs: null);
        }

        private List<string> GetProperties(CommandDescriptor commandDescriptor)
        {
            List<string> fieldStrings = new List<string>();
            // add top level args/opts
            fieldStrings.AddRange(commandDescriptor.Arguments
                                                   .Where(arg => !arg.IsHidden)
                                                   .Select(arg => GetArgument(arg, commandDescriptor))
                                                   .ToList());

            fieldStrings.AddRange(commandDescriptor.Options
                                                   .Where(opt => !opt.IsHidden)
                                                   .Select(opt => GetOption(opt, commandDescriptor))
                                                   .ToList());
            // add subcommands
            fieldStrings.AddRange(commandDescriptor.SubCommands.SelectMany(cmd => GetProperties(cmd)));

            return fieldStrings;
        }

        private string GetArgument(ArgumentDescriptor arg, CommandDescriptor commandDescriptor)
        {
            var scope = Scope.Public;
            return generate.Property(
                scope: scope,
                type: generate.MakeGenericType("Argument", arg.ArgumentType.TypeAsString()),
                // need to properly handle top level prop names (i.e. no command prefix)
                name: NameForProperty(arg.Name, commandDescriptor.Name ?? ""),
                setterScope: scope
            );
        }

        private string GetOption(OptionDescriptor opt, CommandDescriptor commandDescriptor)
        {
            Scope scope = Scope.Public;
            return generate.Property(
                scope: scope,
                type: generate.MakeGenericType("Option", opt.Arguments.First().ArgumentType.TypeAsString()),
                name: NameForProperty(opt.Name, commandDescriptor.Name ?? ""),
                setterScope: scope
            );
        }

        private string NameForGetCommand(string cmdName)
            => $"Get{cmdName}Command";

        private string NameForInvokeCommand(string cmdName)
            => $"Invoke{cmdName}Async";

        private string NameForCommand(string cmdName)
            => $"{cmdName}Command";

        private string NameForProperty(string cmdName, string name)
            => cmdName == "" ? name : $"{cmdName}_{name}";
    }
}
