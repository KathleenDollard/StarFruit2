using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorSupport
{
    // Jean:
    // * Adjusted some naming to be strict that parameter means there is a type and arg means there is not
    // * Adjusted some naming to have the presence of a semicolon reflected in name via "Statement"
    // * Made more composable (see InitStatement)
    public static class Extensions
    {
        public static StringBuilder AppendRange(this StringBuilder sb, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
            return sb;
        }

        public static string CommaJoin(this IEnumerable<string> strings)
            => string.Join(", ", strings);
    }
    public abstract class Generate
    {
        public Generate(StringBuilder? sb = null)
             => StringBuilder = sb ?? new StringBuilder();
        internal StringBuilder StringBuilder { get; }

        // in VB I think this is "MyBase"
        public abstract string Base { get; }
        // In VB "Me"
        public abstract string This { get; }
        // In VB "Dim"
        public abstract string Var { get; } 

        public abstract string Assign(string leftHand, string rightHand, string op = "=");
        public abstract string Assign(string leftHand, string rightHand);
        public ClassGen Class(string name, Scope scope = Scope.Public, bool isPartial = false, bool isStatic = false)
            => new ClassGen(this, name, scope, isPartial, isStatic);
        public abstract GenerateCSharp Ctor(string ctorDeclaration, string ctorBaseCall, params string[] ctorBody);
        public abstract string CtorBaseCall(params string[] baseArgs);
        public abstract string CtorDeclaration(string className, Scope scope = Scope.Public, params string[] ctorParameters);
        public abstract string Field(Scope scope, string type, string name);
        public abstract string GenericType(string type, params string[] genericArgs);
        public abstract GenerateCSharp InitStatement(string newObject, params string[] assignments); //
        public abstract string Lambda(string lambdaDeclaration, string expression);
        public abstract string LambdaDeclaration(params string[] parameters);
        public abstract GenerateCSharp Method(Scope scope, string name, IEnumerable<string> methodBody, string returnType, bool isAsync = false, bool isOverriden = false, params string[] arguments);
        public abstract string MethodCall(string methodName, params string[] args);
        public abstract string MultiLineLambda(string lambdaDeclaration, params string[] statements);
        public abstract GenerateCSharp Namespace(string name, IEnumerable<string> classBody);
        public abstract string NewObject(string objName, params string[] ctorArgs);
        public abstract string Parameter(string paramType, string paramName);
        public abstract string Property(Scope scope, string type, string name, Scope setterScope);
        public abstract GenerateCSharp ReturnStatement(bool await = false, params string[] returnValue);
        public abstract GenerateCSharp ReturnStatement( params string[] returnValue);
        public abstract GenerateCSharp Usings(params string[] nspaces);
        public abstract void AssignStatement(string v1, string v2);
    }


    public class GenerateCSharp : Generate
    {
        public override string Base => "base";

        public override string This => "this";

        public override string Var => "var";

        public GenerateCSharp(StringBuilder? sb = null)
            : base(sb) { }

        public override GenerateCSharp Usings(params string[] nspaces)
        {
            StringBuilder.AppendRange(nspaces.Select(nspace => $"using {nspace};"));
            return this;
        }

        public override GenerateCSharp Namespace(string name, IEnumerable<string> classBody)
        {
            StringBuilder.AppendLine($"namespace {name}");
            StringBuilder.AppendLine("{");
            StringBuilder.AppendRange(classBody);
            StringBuilder.AppendLine("}");

            return this;
        }

        // string type and string genericType is very place holder, re-eval as needed
        public override string Property(Scope scope, string type, string name, Scope setterScope)
        {
            string setter = setterScope == Scope.Private ? "private set;" : "set;";
            return $"{scope.CSharpString()} {type} {name} {{ get; {setter} }}";
        }

        public override string Field(Scope scope, string type, string name)
            => $"{scope.CSharpString()} {type} {name}";

        // generate.MakeParam("BindingContext", "bindingContext")
        // could do String.join(",",generate.MakeParam(...), generate.MakeParam(...))
        // TODO: re-eval this interface for prettiness
        public override GenerateCSharp Method(Scope scope,
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
                $"{scope.CSharpString()} {overrideStr} {asyncStr} {returnType} {name}({string.Join(", ", arguments)})",
                "{"
            };

            StringBuilder.AppendRange(methodBody);
            StringBuilder.AppendLine("}");

            return this;
        }

        public override string MethodCall(string methodName, params string[] args)
            => $"{methodName}({args.CommaJoin()})";

        public override string CtorDeclaration(string className, Scope scope = Scope.Public, params string[] ctorParameters)
            => $"{scope.CSharpString()} {className}({ctorParameters.CommaJoin()})";

        public override string CtorBaseCall(params string[] baseArgs)
            => $": base({baseArgs.CommaJoin()})";

        public override GenerateCSharp Ctor(string ctorDeclaration, string ctorBaseCall, params string[] ctorBody)
        {
            StringBuilder.AppendLine(ctorDeclaration);
            StringBuilder.AppendLine(ctorBaseCall);
            StringBuilder.AppendLine("{");
            StringBuilder.AppendRange(ctorBody);
            StringBuilder.AppendLine("}");

            return this;
        }

        public override string GenericType(string type, params string[] genericArgs)
            => genericArgs.Any()
                ? $"{type}<{string.Join(", ", genericArgs)}>"
                : type;

        public override string Parameter(string paramType, string paramName)
            => $"{paramType} {paramName}";

        public override string Assign(string leftHand, string rightHand, string op = "=")
            => $"{leftHand} {op} {rightHand}";

        // Other assignments are separate methods to accomodate op differences like =+ and =& for concat
        public override string Assign(string leftHand, string rightHand)
            => $"{leftHand} = {rightHand}";

        public override string LambdaDeclaration(params string[] parameters)
        {
            var s = $"{parameters.CommaJoin()}";
            return s.Contains(" ")
                    ? $"({s})"
                    : s;
        }

        public override string Lambda(string lambdaDeclaration, string expression)
            => $"{lambdaDeclaration} => {expression}";

        public override string MultiLineLambda(string lambdaDeclaration, params string[] statements)
            => $"{lambdaDeclaration} => \n{{{string.Join("\n", statements)}\n}}";

        public override GenerateCSharp ReturnStatement(bool await = false, params string[] returnValue)
        {

            for (int i = 0; i < returnValue.Length; i++)
            {
                if (i == 0)
                {
                    StringBuilder.AppendLine($"return{(await == true ? " await " : " ")}{(returnValue.Any() ? returnValue[(int)0] : "")}");
                }
                StringBuilder.AppendLine(returnValue[i]);
            }
            StringBuilder.Append(";");

            return this;
        }

        public override string NewObject(string objName, params string[] ctorArgs)
            => $"new {objName}({ctorArgs?.CommaJoin()})";

        public override GenerateCSharp InitStatement(string newObject, params string[] assignments)
        {
            StringBuilder.AppendLine(newObject);
            StringBuilder.AppendLine("{");
            StringBuilder.AppendRange(assignments.Select(a => $"{a},"));
            StringBuilder.AppendLine("}");

            return this;
        }

        public override GenerateCSharp ReturnStatement(params string[] returnValue)
        {
            throw new NotImplementedException();
        }

        public override void AssignStatement(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }

    public class ClassGen
    {
        private string? baseClassName;
        private readonly List<string> interfaces;
        private readonly StringBuilder sb;
        public Generate Generate { get; }

        public ClassGen(Generate generate, string name, Scope scope, bool isPartial, bool isStatic)
        {
            Name = name;
            Scope = scope;
            IsPartial = isPartial;
            IsStatic = isStatic;
            interfaces = new List<string>();
            sb = generate.StringBuilder;
            Generate = generate;
        }

        public string Name { get; }
        public Scope Scope { get; }
        public bool IsPartial { get; }
        public bool IsStatic { get; }

        public ClassGen Base(string name)
        {
            baseClassName = name;
            return this;
        }

        public ClassGen Interfaces(params string[] names)
        {
            interfaces.AddRange(names);
            return this;
        }

        public ClassGen Fields(params string[] statements)
        {
            foreach (var statement in statements)
            {
                sb.AppendLine(statement);
            }
            return this;
        }

        public ConstructorGen ConstructorDeclaration(string className,
                                    Scope scope = Scope.Public,
                                    params string[] parameters)
        {
            return new ConstructorGen(this, sb, className, scope, parameters);
        }



        public ClassGen OutputClass(params string[] statements)
        {
            sb.AppendLine($@"{Scope.CSharpString()}{(IsPartial ? " partial" : "")}{(IsStatic ? " static" : "")} class {Name}");

            if (!(baseClassName is null))
            {
                sb.AppendLine($": {baseClassName} {string.Join(", ", interfaces)}");
            }

            sb.AppendLine("{");
            sb.AppendRange(statements);
            sb.AppendLine("}");

            return this;
        }

    }



    public class ConstructorGen
    {
        private readonly StringBuilder sb;
        private readonly ClassGen classGen;
        private string className;
        private Scope scope;
        private string[] parameters;
        private Generate generate;

        public ConstructorGen(ClassGen classGen, StringBuilder sb, string className, Scope scope, string[] parameters)
        {
            this.sb = sb;
            this.classGen = classGen;
            this.className = className;
            this.scope = scope;
            this.parameters = parameters;
            generate = classGen.Generate;
        }

        public ConstructorGen ConstructorBase(params string[] baseArgs)
        {
            sb.AppendLine($": base({baseArgs.CommaJoin()})");
            return this;
        }

        public ConstructorGen This(params string[] baseArgs)
        {
            sb.AppendLine($": this({baseArgs.CommaJoin()})");
            return this;
        }

        public ConstructorGen ConstructorStatements(params string[] statements)
        {
            foreach (var statement in statements)
            {
                sb.AppendLine(statement);
            }
            return this;
        }


        public ConstructorGen ConstructorBodyStart()
        {
            sb.AppendLine("}");
            return this;
        }
        public ConstructorGen ConstructorBodyEnd()
        {
            sb.AppendLine("}");
            return this;
        }

    }
}
