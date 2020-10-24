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
            //StringBuilder sb = new StringBuilder();
            //Generate generate = new Generate();
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
            strCollection.AddRange(GetFields(commandDescriptor));
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
                generate.InstanceDeclaration,
            };

            // see two layer gen #36-38, unsure best vert whitespace management here
            // fetch all the GetValue calls
            var cmdValues = string.Join("\n,", GetCommandValues(cmd.Name, cmd.Arguments, cmd.Options));

            // original name may fail on 1 layer
            methodBody.Add($"return await NewInstance.{cmd.OriginalName}({cmdValues});"); ;

            return generate.Method(Scope.Public,
                                   NameForInvokeCommand(cmd.Name),
                                   methodBody,
                                   "Task",
                                   "int",
                                   "BindingContext bindingContext",
                                   true);
        }

        private List<string> GetCommandValues(string cmdName, List<ArgumentDescriptor> args, List<OptionDescriptor> options)
        {
            List<string> strCollection = new List<string> { };

            // unsure on original name vs name here
            var argValues = args.Select(arg => GetValueMethod(cmdName, arg.Name));
            var optValues = options.Select(opt => GetValueMethod(cmdName, opt.Name));

            strCollection.AddRange(argValues);
            strCollection.AddRange(optValues);

            return strCollection;

            static string GetValueMethod(string cmdName, string name) => $"GetValue(bindingContext, {cmdName}_{name})";
        }

        private IEnumerable<string> GetCommandMethods(CommandDescriptor commandDescriptor)
        {
            foreach (var cmd in commandDescriptor.SubCommands)
            {
                GetCommandMethod(cmd);
            }
        }

        private List<string> GetCommandMethod(CommandDescriptor cmd)
        {
            List<string> strCollection = new List<string>
            {
                $"private Command {NameForGetCommand(cmd.Name)}",
                "{"
            };

            // if this is VB unfriendly, push down to generate, might also push the get arg def down there as well
            // TODO: finalize how to handle arg declaration
            var argDeclarations = cmd.Arguments.Select(arg => $@"{cmd.Name}_{arg.OriginalName} = GetArg<{arg.ArgumentType.TypeAsString()}>(""{arg.CliName}"", ""{arg.Description}"")");


            // do samething for option

            strCollection.AddRange(argDeclarations);

            // do new command, unsure how to get which ones go to constructor


            strCollection.Add(generate.Assign($"{cmd.Name}Command.Handler", $"new CommandSourceHandler({NameForInvokeCommand(cmd.Name)});"));
            strCollection.Add("return {cmd.Name}Command");

            return strCollection;
        }
        private IEnumerable<string> GetNewInstanceMethod(CommandDescriptor commandDescriptor)
        {
            throw new NotImplementedException();
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

            // go add commands to command
            // method that takes subcommand and returns the name of the method to get subcommand
            // i.e. give it "Find" -> "GetFindCommand", "Find" -> "InvokeFindAsync"
            // see two layer .gen #27, #32, #46
            // NameForGetCommand, NameForInvokeCommand 

            // assume all subcommands are methods. This is bad assumption, they can be classes or methods, but sidestep for now, see multi-layer model

            return generate.Constructor(className: className, cliName: commandDescriptor.CliName, constructorBody: strCollection);

            string GetCtorOpts(OptionDescriptor opt, ArgumentDescriptor arg)
                => generate.Assign(opt.Name, generate.OptionInitExpression(opt.CliName, arg.ArgumentType.TypeAsString()));
            //=> generate.OptionDeclarationForCtor(opt.Name, opt.CliName, arg.ArgumentType.TypeAsString());

            string GetCtorArg(ArgumentDescriptor arg)
                => generate.Assign(arg.Name, generate.OptionInitExpression(arg.CliName, arg.ArgumentType.TypeAsString()));
            //=> generate.ArgDeclarationForCtor(arg.Name, arg.CliName, arg.ArgumentType.TypeAsString());
        }

        private List<string> GetFields(CommandDescriptor commandDescriptor)
        {
            List<string> fieldStrings = new List<string>();
            fieldStrings.AddRange(commandDescriptor.Arguments
                                                   .Where(arg => !arg.IsHidden)
                                                   .Select(arg => GetArgument(arg))
                                                   .ToList());

            fieldStrings.AddRange(commandDescriptor.Options
                                                   .Where(opt => !opt.IsHidden)
                                                   .Select(opt => GetOption(opt))
                                                   .ToList());

            return fieldStrings;
        }

        private string GetArgument(ArgumentDescriptor arg)
        {
            var scope = Scope.Public;
            return generate.Property(
                scope: scope,
                type: "Argument",
                genericType: arg.ArgumentType.TypeAsString(),
                name: arg.Name /*might need to massage name*/,
                setterScope: scope
            );
        }

        private string GetOption(OptionDescriptor opt)
        {
            Scope scope = Scope.Public;
            return generate.Property(
                scope: scope,
                type: "Option",
                // can I safely assume First() is valid here?
                genericType: opt.Arguments.First().ArgumentType.TypeAsString(),
                name: opt.Name,
                setterScope: scope
            );
        }

        // does this need to take args, and should that be handled here or elsewhere?
        private string NameForGetCommand(string cmdName)
        {
            return $"Get{cmdName}Command";
        }

        private string NameForInvokeCommand(string cmdName)
        {
            return $"Invoke{cmdName}Async";
        }


    }
}
