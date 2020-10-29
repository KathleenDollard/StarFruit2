using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarFruit2
{
    public class CodeGeneratorTemp
    {
        private Generate generate;
        public CodeGeneratorTemp(Generate generate)
        {
            this.generate = generate;
        }
        // kill foo, move to CodeGenerator
        public List<string> Foo(CliDescriptor cliDescriptor)
        {
            List<string> strCollection = new List<string>();
            strCollection.AddRange(generate.Usings("System.CommandLine", "System.Linq", "System.Text"));

            // hardcoded moving towards descriptor
            strCollection.AddRange(generate.Namespace("TwoLayerCli", GetNamespaceBody(cliDescriptor)));
            // append namespace opening?

            return strCollection;
        }

        private List<string> GetNamespaceBody(CliDescriptor cliDescriptor)
            => generate.Class(Scope.Public, true, cliDescriptor.GeneratedCommandSourceClassName, GetClassBody(cliDescriptor.CommandDescriptor, cliDescriptor.GeneratedCommandSourceClassName))
                ?? new List<string> { };

        private List<string> GetClassBody(CommandDescriptor commandDescriptor, string className)
        {
            // this needs to call into the Class method on Generate, following pattern of the .gen.cs code

            List<string> strCollection = new List<string>();
            strCollection.AddRange(GetProperties(commandDescriptor));
            strCollection.AddRange(GetConstructor(commandDescriptor, className));


            // consider if these can leverage same underlying Generate method, be mindful of rabbit hole, generated methods can have > 1 param
            strCollection.AddRange(GetInvokeMethods(commandDescriptor)); // <--- gotta get all of them
            strCollection.AddRange(GetCommandMethods(commandDescriptor));


            strCollection.AddRange(GetNewInstanceMethod(commandDescriptor));

            return strCollection;
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
            var cmdValues = string.Join(", ", GetCommandValues(cmd.Name, cmd.Arguments, cmd.Options));

            // original name may fail on 1 layer
            // push this to generate, since it's C#. generate.Return(body, isAsync)
            methodBody.Add(generate.Return($"NewInstance.{cmd.OriginalName}({cmdValues})", true));

            // this will fall over with compound generic types, deal with that if encountered
            return generate.Method(Scope.Public,
                                   NameForInvokeCommand(cmd.Name),
                                   methodBody,
                                   generate.MakeGenericType("Task", "int"),
                                   true,
                                   generate.MakeParam("BindingContext", "bindingContext"));
        }

        private List<string> GetCommandValues(string cmdName, List<ArgumentDescriptor> args, List<OptionDescriptor> options)
        {
            List<string> strCollection = new List<string> { };

            // unsure on original name vs name here
            var argValues = args.Select(arg => generate.GetValueMethod(NameForProperty(cmdName, arg.Name)));
            var optValues = options.Select(opt => generate.GetValueMethod(NameForProperty(cmdName, opt.Name)));

            strCollection.AddRange(argValues);
            strCollection.AddRange(optValues);

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
                    )
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
            // placeholder, must fetch those passed as ctor params
            var ctorParams = new List<string> { };
            // placeholder, must fetch those passed as ctor properties
            var initProperties = new List<string> { };

            // add a version of return that takes a list and returns a list
            var methodBody = generate.Return(generate.NewCliRoot(ctorParams, initProperties));

            return generate.Method(Scope.Private,
                                   "GetNewInstance",
                                   methodBody,
                                   "CliRoot",
                                   arguments: generate.MakeParam("BindingContext", "bindingContext"));
        }

        private IEnumerable<string> GetConstructor(CommandDescriptor commandDescriptor, string className)
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

            return generate.Constructor(className: className, cliName: commandDescriptor.CliName, constructorBody: strCollection);

            string GetCtorOpts(OptionDescriptor opt, ArgumentDescriptor arg)
                => generate.Assign(opt.Name, generate.OptionInitExpression(arg.ArgumentType.TypeAsString(), opt.CliName));

            string GetCtorArg(ArgumentDescriptor arg)
                => generate.Assign(arg.Name, generate.OptionInitExpression(arg.ArgumentType.TypeAsString(), arg.CliName));
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
                type: "Argument",
                genericType: arg.ArgumentType.TypeAsString(),
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
                type: "Option",
                // can I safely assume First() is valid here?
                genericType: opt.Arguments.First().ArgumentType.TypeAsString(),
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
