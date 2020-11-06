using System;
using System.Collections.Generic;
using StarFruit2.Generator;
using System.CommandLine;
using System.Linq;
using System.Text;

// JEAN:    
// * We need Assign and Init assign to be different methods because VB uses .PropertyName
// * Parameters in generate should be IEnumerable
// * I have forgotten why the right of an assign is an collection.Can you add a comment explaining the scenario

namespace StarFruit2
{
    public enum Scope
    {
        Public,
        Private,
        Internal,
        Protected
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

        // FIXME: the init expression is now opt/arg method call
        public string OptionDeclarationForCtor(string name, string cliName, string optType)
           => Assign(name, OptionInitExpression(cliName, optType));

        public string ArgDeclarationForCtor(string name, string cliName, string argType)
            => Assign(name, ArgInitExpression(cliName, argType));

        // FIXME: look at naming
        public string SetNewInstance
            => Assign("NewInstance", "GetNewInstance(bindingContext)");


        public string AddToCommand(string methodName, string argumentName)
            => MethodCall($"Command.{methodName}",
                          new List<string> { argumentName }).EndStatement();



        // FIXME: these might stick around as the GetArg/Option method call builders
        public static string ArgInitExpression(string cliName, string argType)
            => $@"new Argument<{argType}>(""{cliName}"");";

        public string OptionInitExpression(string cliName, string optType)
            => $@"new Option<{optType}>(""{cliName}"");";
        internal string GetArg(string? cliName, string argType, string? description)
            => $"GetArg<argType>({cliName}, {description})";

        internal string GetOpt(string? cliName, string optType, string? description)
            => $"GetOpt<optType>({cliName}, {description})";

        internal string NewCommand(string cliName, string? desc = null)
        {
            var args = new List<string> { cliName, desc };
            return NewObject($"Command", args);
        }

        internal string NewCommandHandler(string cmdName)
            => NewObject("CommandSourceHandler", new List<String> { cmdName });


        internal string GetValueMethod(string valToFetch)
            => $"GetValue(bindingContext, {valToFetch})";


        // above is non-general, below is general



        public IEnumerable<string> Usings(params string[] nspaces)
            => nspaces.Select(nspace => $"using {nspace};");

        public List<string> Namespace(string name, IEnumerable<string> classBody)
        {
            List<string> nspace = new List<string>();
            nspace.Add($"namespace {name}");
            nspace.Add("{");
            nspace.AddRange(classBody);
            nspace.Add("}");

            return nspace;
        }

        public List<string> Class(Scope scope, bool isPartial, string className, string? baseClassName, IEnumerable<string> classBody)
        {
            var strPartial = isPartial ? "partial" : "";
            // TODO: this somewhat screws indenting whitespace but is much simpler to test!
            // consider using Utils.cs, but each line as list elem is ok
            List<string> strCollection = new List<string>
            {
                $@"{scope.CSharpString()} {strPartial} class {className}"
            };

            if (!(baseClassName is null))
            {
                strCollection.Add($": {baseClassName}");
            }

            strCollection.Add("{");
            strCollection.AddRange(classBody);
            strCollection.Add("}");

            return strCollection;
        }

        // string type and string genericType is very place holder, re-eval as needed
        public string Property(Scope scope, string type, string name, Scope setterScope)
        {
            string setter = setterScope == Scope.Private ? "private set;" : "set;";
            return $"{scope.CSharpString()} {type} {name} {{ get; {setter} }}";
        }

        public string Field(Scope scope, string type, string name)
            => $"{scope.CSharpString()} {type} {name}";

        // generate.MakeParam("BindingContext", "bindingContext")
        // could do String.join(",",generate.MakeParam(...), generate.MakeParam(...))
        // TODO: re-eval this interface for prettiness
        public List<string> Method(Scope scope, string name, IEnumerable<string> methodBody, string returnType, bool isAsync = false, bool isOverriden = false, params string[] arguments)
        {
            if (isAsync && !returnType.StartsWith("Task<"))
            {
                throw new InvalidOperationException("async method must return a task in this context");
            }

            string asyncStr = isAsync ? "async" : "";
            string overrideStr = isOverriden ? "override" : "";
            List<string> strCollection = new List<string> {
                $"{scope.CSharpString()} {overrideStr} {asyncStr} {returnType} {name}({String.Join(", ", arguments)})",
                "{"
            };

            strCollection.AddRange(methodBody);
            strCollection.Add("}");

            return strCollection;
        }

        public string MethodCall(string methodName, IEnumerable<string>? args = null)
            => $"{methodName}({FormattedArgs(args)})";
        public string MethodCall(string methodName, params string[] args)
            => MethodCall(methodName, args.ToList());

        public List<string> Constructor(string className, IEnumerable<string>? ctorArgs, IEnumerable<string>? baseArgs, IEnumerable<string> ctorBody, Scope scope = Scope.Public)
        {
            List<string> strCollection = new List<string>
            {
                $"{scope.CSharpString()} {className}({FormattedArgs(ctorArgs)})",
                $": base({FormattedArgs(baseArgs)})"
            };

            strCollection.Add("{");
            strCollection.AddRange(ctorBody);
            strCollection.Add("}");

            return strCollection;
        }

        // FIXME: delete me
        public List<string> BuildBlock(string firstLine, IEnumerable<string> body)
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

        public string GenericType(string type) => type;
        public string GenericType(string type, params string[] genericArgs)
            => $"{type}<{String.Join(", ", genericArgs)}>";

        public string Parameter(string paramType, string paramName)
            => $"{paramType} {paramName}";


        public string Assign(string leftHand, string rightHand, string op = "=")
            => $"{leftHand} {op} {rightHand}";

        public List<string> Assign(string leftHand, IEnumerable<string> rightHand, string op = "=")
        {
            var rightHandArr = rightHand.ToArray();
            var strCollection = new List<string>
            {
                Assign(leftHand, rightHandArr[0], op)
                //$"return{(await == true ? " await " : " ")}{returnValue[0]}"
            };

            switch (rightHandArr.Count())
            {
                case 1:
                    return strCollection;
            }

            // JEAN: What are we doing here?
            strCollection.AddRange(rightHandArr[1..^1]);
            strCollection.Add(rightHandArr[^1]);

            return strCollection;
        }
        // FIXME: consider how we handle expression vs statements
        //        COVER ME WITH TESTS
        public string Lambda(IEnumerable<string>? args, IEnumerable<string> statements)
        {
            // we can only handle bizz right now
            //Func<bool> foo = () => true;
            //Func<bool> bizz = () => { return true; };
            string formattedStatements;
            switch (statements.Count())
            {
                case 0:
                    formattedStatements = "{ }";
                    break;
                case 1:
                    formattedStatements = statements.First().EndStatement();
                    break;
                default:
                    {
                        formattedStatements = $"{{ {string.Join(" ", statements.Select(s => s.EndStatement()))} }}";
                        break;
                    }
            }

            return $"({FormattedArgs(args)}) => {formattedStatements}";
        }

        public string FormattedArgs(IEnumerable<string>? args)
            => args is null ? "" : string.Join(", ", args);

        // in VB I think this is "MyBase"
        public readonly string Base = "base";
        // In VB "Me"
        public readonly string This = "this";
        // In VB "Dim"
        public readonly string Var = "var";

        public string Return(string returnValue, bool await = false)
            => $"return{(await == true ? " await " : " ")}{returnValue}".EndStatement();

        public List<string> Return(IEnumerable<string> returnValue, bool await = false)
        {
            var returnValueArr = returnValue.ToArray();
            var strCollection = new List<string>
            {
                $"return{(await == true ? " await " : " ")}{returnValueArr[0]}"
            };

            switch (returnValueArr.Count())
            {
                case 0:
                    return new List<string> { };
                case 1:
                    return strCollection.EndStatement();
            }

            strCollection.AddRange(returnValueArr[1..^1]);
            strCollection.Add(returnValueArr[^1]);

            return strCollection.EndStatement();
        }

        public string NewObject(string objName, params string[] paramStrs)
            => NewObject(objName, paramStrs.ToList());
        public string NewObject(string objName, IEnumerable<string>? paramStr = null)
            => $"new {objName}({FormattedArgs(paramStr)})";

        public List<string> NewObjectWithInit(string objName, IEnumerable<string>? ctorArgs, IEnumerable<string>? assignments)
        {
            var strCollection = new List<string>
            {
                NewObject(objName, ctorArgs),
                "{"
            };

            strCollection.AddRange(assignments.Select(a => $"{a},"));

            strCollection.Add("}");
            return strCollection.EndStatement();
        }
    }
}
