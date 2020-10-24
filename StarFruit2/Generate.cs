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

        // REVERT COMMIT, ADD TO NEW BRANCH

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
                // better way than casting like this?
                // solve this w/ extension method on Scope enum, if not introduce method in Generate for VB vs C#
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

        public List<string> Method(Scope scope, string name, List<string> methodBody, string type, string? genericType, string arguments = "", bool isAsync = false)
        {
            // very mixed feelings about this. It's super easy to check here, but maybe it's just the
            // compiler's job and I shouldn't worry about it
            if (isAsync && type != "Task")
            {
                throw new InvalidOperationException("async method must return a task in this context");
            }

            string asyncStr = isAsync ? "async" : "";
            string compositeType = genericType is null ? type : $"{type}<{genericType}>";
            List<string> strCollection = new List<string> {
                $"{scope.CSharpString()} {asyncStr} {compositeType} {name}({arguments})",
                "{"
            };

            strCollection.AddRange(methodBody);
            strCollection.Add("}");

            return strCollection;
        }

        // TODO: rename me
        public string InstanceDeclaration => "NewInstance = GetNewInstance(bindingContext);";

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

        public static string ArgInitExpression(string cliName, string argType) 
            => $@"new Argument<{argType}>(""{cliName}"");";

        public string OptionInitExpression(string optType, string cliName) => $@"new Option<{optType}>(""{cliName}"");";


        public string AddToCommand(string methodName, string argumentName)
            => $"Command.{methodName}({argumentName});";
    }
}
