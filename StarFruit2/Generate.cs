using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;

namespace StarFruit2
{
    public enum Scope
    {
        Public,
        Private,
        Internal
    }

    static class ScopeExtensions
    {
        public static string CSharpString(this Scope scope)
            => scope.ToString().ToLower();

        public static string VBString(this Scope scope)
            => scope.ToString();
    }

    public class Generate
    {

        // generate is a c# generate class

        // descriptor free, CodeGenerator should rip descirptor apart and pass into
        // use source gen file to see pattern


        // create a class, put it is temp place, let compiler see it and scream if necessary
        // 1a. create temp class, put stuff into it
        // 1b. create variation of gen class that compiles, copy field from the gen and compare to it
        // 2. dotnet interactive and just compile it, could potentially ask roslyn instead
        // CompileAndTest file
        public IEnumerable<string> Usings(params string[] nspaces)
            => nspaces.Select(nspace => $"using {nspace};");

        public List<string> Namespace(string name, List<string> classBody)
        {
            List<string> nspace = new List<string>();
            nspace.Add($"namespace {name}");
            nspace.Add("{");
            nspace.AddRange(classBody);
            nspace.Add("}");

            return nspace;
        }

        public List<string> Class(Scope scope, bool isPartial, string className, List<string> classBody)
        {
            var strPartial = isPartial ? "partial" : "";
            // TODO: this somewhat screws indenting whitespace but is much simpler to test!
            // consider using Utils.cs, but each line as list elem is ok
            List<string> strCollection = new List<string>
            {
                $@"{scope.CSharpString()} {strPartial} class {className}", "{"
            };

            strCollection.AddRange(classBody);
            strCollection.Add("}");

            return strCollection;
        }

        // string type and string genericType is very place holder, re-eval as needed
        public string Property(Scope scope, string type, string? genericType, string name, Scope setterScope)
        {
            string compositeType = (genericType is null) ? type : $"{type}<{genericType}>";
            string setter = setterScope == Scope.Private ? "private set;" : "set;";
            return $"{scope.CSharpString()} {compositeType} {name} {{ get; {setter} }}";
        }

        // generate.MakeParam("BindingContext", "bindingContext")
        // could do String.join(",",generate.MakeParam(...), generate.MakeParam(...))
        // TODO: re-eval this interface for prettiness
        public List<string> Method(Scope scope, string name, List<string> methodBody, string returnType, bool isAsync = false, params string[] arguments)
        {
            if (isAsync && !returnType.StartsWith("Task"))
            {
                throw new InvalidOperationException("async method must return a task in this context");
            }

            string asyncStr = isAsync ? "async" : "";
            List<string> strCollection = new List<string> {
                $"{scope.CSharpString()} {asyncStr} {returnType} {name}({String.Join(", ", arguments)})",
                "{"
            };

            strCollection.AddRange(methodBody);
            strCollection.Add("}");

            return strCollection;
        }

        public string SetNewInstance 
            => "NewInstance = GetNewInstance(bindingContext);";

        public List<string> Constructor(string className, string cliName, IEnumerable<string> constructorBody)
        {
            List<string> strCollection = new List<string>
            {
                $@"{Scope.Public.CSharpString()} {className}",
                $": base(new Command({cliName}))",
                "{"
            };

            strCollection.AddRange(constructorBody);
            strCollection.Add("}");

            return strCollection;
        }

        public List<string> BuildBlock(string firstLine, List<string> body)
        {
            var strCollection = new List<string>
            {
                firstLine,
                "{"
            };

            strCollection.AddRange(body);

            strCollection.Add("}");

            return strCollection;
        }

        public string MakeGenericType(string type) => type;
        public string MakeGenericType(string type, params string[] genericArgs) 
            => $"{type}<{String.Join(", ", genericArgs)}>";

        public string MakeParam(string paramType, string paramName)
            => $"{paramType} {paramName}";


        // string AssignTo(string varName, string assignment) -> varname = assignment;
        // TODO: split left and right parts into assignment (left) and expression (right)
        //public string OptionDeclarationForCtor(string name, string cliName, string optType)
        //    => $@"{name} = new Option<{optType}>(""{cliName}"");";
        public string OptionDeclarationForCtor(string name, string cliName, string optType)
           => Assign(name, OptionInitExpression(cliName, optType));

        public string ArgDeclarationForCtor(string name, string cliName, string argType)
            => Assign(name, ArgInitExpression(cliName, argType));

        public string Assign(string leftHand, string rightHand, string op = "=") 
            => $"{leftHand} {op} {rightHand}{(rightHand.EndsWith(";") ? "" : ";")}";

        // if we keep these 2 init expressions, then this could just be a single
        // object init method call.
        public static string ArgInitExpression(string cliName, string argType)
            => $@"new Argument<{argType}>(""{cliName}"");";

        public string OptionInitExpression(string cliName, string optType)
            => $@"new Option<{optType}>(""{cliName}"");";


        public string AddToCommand(string methodName, string argumentName)
            => $"Command.{methodName}({argumentName});";

        internal string GetArg(string? cliName, string argType, string? description) 
            => $"GetArg<argType>({cliName}, {description})";

        internal string GetOpt(string? cliName, string optType, string? description) 
            => $"GetOpt<optType>({cliName}, {description})";

        internal string NewCommand(string? cliName) 
            => $"new Command({cliName})";

        internal string NewCommandHandler(string cmdName) 
            => $"new CommandSourceHandler({cmdName})";

        public string Return(string returnValue, bool await = false) 
            => $"return {(await == true ? "await" : "")} {returnValue};";

        public List<string> Return(List<string> returnValue, bool await = false) {

            var strCollection = new List<string> 
            { 
                $"return {(await == true ? "await" : "")} {returnValue[0]}" 
            };

            if (returnValue.Count() == 1)
            {
                strCollection[0] += ";";
                return strCollection;
            } else if (returnValue.Count() == 0)
            {
                return new List<string> { };
            }

            strCollection.AddRange(returnValue.ToArray()[1..^1]);
            strCollection.Add($"{returnValue[^1]};");

            return strCollection;
        }

        internal string GetValueMethod(string valToFetch)
            => $"GetValue(bindingContext, {valToFetch})";

        // how much should be here vs code generator?
        // look at this using a public object init method
        internal List<string> NewCliRoot(List<string> ctorParams, List<string> ctorProperties)
        {
            var paramValues = ctorParams.Select(param => GetValueMethod(param));
            var propDeclarations = ctorProperties.Select(prop => $"{Assign(prop, GetValueMethod(prop))},");

            var strCollection = new List<string> 
            {
                $"new CliRoot({String.Join(", ", paramValues)})",
                "{"
            };

            strCollection.AddRange(propDeclarations);
            strCollection.Add("}");

            return strCollection;
        }
    }
}
