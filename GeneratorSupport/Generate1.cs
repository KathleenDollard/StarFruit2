using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorSupport
{
    //// Jean:
    //// * Adjusted some naming to be strict that parameter means there is a type and arg means there is not
    //// * Adjusted some naming to have the presence of a semicolon reflected in name via "Statement"
    //// * Made more composable (see InitStatement)
    //public static class Extensions1
    //{
    //    public static StringBuilder AppendRange(this StringBuilder sb, IEnumerable<string> lines)
    //    {
    //        foreach (var line in lines)
    //        {
    //            sb.AppendLine(line);
    //        }
    //        return sb;
    //    }

    //    public static string CommaJoin(this IEnumerable<string> strings)
    //        => string.Join(", ", strings);
    //}


    public class GenerateCSharp1
    {
        private readonly StringBuilder sb;

        public GenerateCSharp1(StringBuilder? sb = null)
            => this.sb = sb ?? new StringBuilder();

        public GenerateCSharp1 Usings(params string[] nspaces)
        {
            sb.AppendRange(nspaces.Select(nspace => $"using {nspace};"));
            return this;
        }

        public GenerateCSharp1 Namespace(string name, IEnumerable<string> classBody)
        {
            sb.AppendLine($"namespace {name}");
            sb.AppendLine("{");
            sb.AppendRange(classBody);
            sb.AppendLine("}");

            return this;
        }

        public string ClassDeclaration(string className,
                                    Scope scope,
                                    bool isPartial=false,
                                    bool isStatic=false,
                                    string? baseClassName=null,
                                    params string[] interfaces)
        {
            var decl = $@"{scope.CSharpString()}{(isPartial ? " partial" : "")}{(isStatic ? " static" : "")} class {className}";

            if (!(baseClassName is null))
            {
                decl +=$": {baseClassName} {string.Join(", ", interfaces)}";
            }
            return decl;
        }

        public GenerateCSharp1 Class(string classDeclaration,
                                    IEnumerable<string> classBody)
        {
            sb.AppendLine(classDeclaration);

            sb.AppendLine("{");
            sb.AppendRange(classBody);
            sb.AppendLine("}");

            return this;
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
        public GenerateCSharp1 Method(Scope scope,
                                     string name,
                                     IEnumerable<string> methodBody,
                                     string returnType,
                                     bool isAsync = false,
                                     bool isOverriden = false,
                                     params string[] arguments)
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

            sb.AppendRange(methodBody);
            sb.AppendLine("}");

            return this;
        }

        public string MethodCall(string methodName, params string[] args)
            => $"{methodName}({args.CommaJoin()})";

        public string CtorDeclaration(string className, Scope scope = Scope.Public, params string[] ctorParameters)
            => $"{scope.CSharpString()} {className}({ctorParameters.CommaJoin()})";

        public string CtorBaseCall(params string[] baseArgs)
            => $": base({baseArgs.CommaJoin()})";

        public GenerateCSharp1 Ctor(string ctorDeclaration, string ctorBaseCall, params string[] ctorBody)
        {
            sb.AppendLine(ctorDeclaration);
            sb.AppendLine(ctorBaseCall);
            sb.AppendLine("{");
            sb.AppendRange(ctorBody);
            sb.AppendLine("}");

            return this;
        }

        public string GenericType(string type, params string[] genericArgs)
            => genericArgs.Any()
                ? $"{type}<{string.Join(", ", genericArgs)}>"
                : type;

        public string Parameter(string paramType, string paramName)
            => $"{paramType} {paramName}";

        public string Assign(string leftHand, string rightHand, string op = "=")
            => $"{leftHand} {op} {rightHand}";

        // Other assignments are separate methods to accomodate op differences like =+ and =& for concat
        public string Assign(string leftHand, string rightHand)
            => $"{leftHand} = {rightHand}";

        public string LambdaDeclaration(params string[] parameters)
        {
            var s = $"{parameters.CommaJoin()}";
            return s.Contains(" ")
                    ? $"({s})"
                    : s;
        }

        public string Lambda(string lambdaDeclaration, string expression)
            => $"{lambdaDeclaration} => {expression}";

        public string MultiLineLambda(string lambdaDeclaration, params string[] statements)
            => $"{lambdaDeclaration} => \n{{{string.Join("\n", statements)}\n}}";

        // in VB I think this is "MyBase"
        public readonly string Base = "base";
        // In VB "Me"
        public readonly string This = "this";
        // In VB "Dim"
        public readonly string Var = "var";

        public GenerateCSharp1 ReturnStatement(bool await = false, params string[] returnValue)
        {

            for (int i = 0; i < returnValue.Length; i++)
            {
                if (i == 0)
                {
                    sb.AppendLine($"return{(await == true ? " await " : " ")}{(returnValue.Any() ? returnValue[(int)0] : "")}");
                }
                sb.AppendLine(returnValue[i]);
            }
            sb.Append(";");

            return this;
        }

        public string NewObject(string objName, params string[] ctorArgs)
            => $"new {objName}({ctorArgs?.CommaJoin()})";

        public GenerateCSharp1 InitStatement(string newObject, params string[] assignments)
        {
            sb.AppendLine(newObject);
            sb.AppendLine("{");
            sb.AppendRange(assignments.Select(a => $"{a},"));
            sb.AppendLine("}");

            return this;
        }
    }
}
