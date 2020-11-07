using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorSupport.Context.CSharp
{
    public class GenerationContextCSharp : GenerationContext
    {
        public GenerationContextCSharp(StringBuilder? sb)
          : base(sb) { }
    }

    public class GenerationEntryContextCSharp : GenerationEntryContext<GenerationEntryContextCSharp>
    {
        public override string Base => "base";
        public override string This => "this";
        public override string Var => "var";

        public GenerationEntryContextCSharp(StringBuilder? sb = null)
            : base(sb) { }

        public override Preamble Preamble()
           => new PreambleCSharp(StringBuilder);

        public override Namespace Namespace(string name)
            => new NamespaceCSharp(StringBuilder, name);

        public override Class Class(string name, Scope scope = Scope.Public, bool isPartial = false, bool isStatic = false)
            => new ClassCSharp(StringBuilder, name, scope, isPartial, isStatic);

        public override string GenericType(string type, params string[] genericArgs)
            => genericArgs.Any()
                    ? $"{type}<{string.Join(", ", genericArgs)}>"
                    : type;
    }

    public class PreambleCSharp : Preamble<PreambleCSharp>
    {
        public PreambleCSharp(StringBuilder sb)
            : base(sb) { }

        public override Preamble Usings(params string[] nspaces)
        {
            StringBuilder.AppendRange(nspaces.Select(nspace => $"using {nspace};"));
            return this;
        }
    }

    public class NamespaceCSharp : Namespace<NamespaceCSharp>
    {
        private readonly string name;

        public NamespaceCSharp(StringBuilder sb, string name)
            : base(sb)
        {
            this.name = name;
        }

        public override Namespace StartBody()
        {
            StringBuilder.AppendLine($"namespace {name}");
            StringBuilder.AppendLine("{");
            return this;
        }

        public override Namespace EndBody()
        {
            StringBuilder.AppendLine("}");
            return this;
        }
    }

    public class ClassCSharp : Class<ClassCSharp>
    {
        public ClassCSharp(StringBuilder sb, string name, Scope scope = Scope.Public, bool isPartial = false, bool isStatic = false)
             : base(sb, name, scope, isPartial, isStatic) { }

        public override Class Ctor(string ctorDeclaration, string ctorBaseCall, params string[] ctorBody)
        {
            throw new NotImplementedException();
        }

        public override Class Field(string? name, string type , Scope scope=Scope.Private )
        {
            // TODO: Support Defaults
            StringBuilder.AppendLine($"{scope.CSharpString()} {type} {name}");
            return this;
        }

        public override Class Method(Scope scope, string name, IEnumerable<string> methodBody, string returnType, bool isAsync = false, bool isOverriden = false, params string[] arguments)
        {
            throw new NotImplementedException();
        }

        public override Class Property(Scope scope, string type, string name, Scope setterScope)
        {
            // TODO: Auto properties, default values and getter/setter bodies
            string setter = setterScope == Scope.Private ? "private set;" : "set;";
            StringBuilder.AppendLine($"{scope.CSharpString()} {type} {name} {{ get; {setter} }}");
            return this;
        }

        public override Class StartBody()
        {
            string isPartialString = (isPartial ? " partial" : "");
            string isStaticString = (isStatic ? " static" : "");
            StringBuilder.Append($@"{scope.CSharpString()}{isPartialString}{isStaticString} class {name}");
            var baseAndInterfaces = new List<string>();
            if (!string.IsNullOrWhiteSpace(baseClassName))
            {
                baseAndInterfaces.Add(baseClassName);
            }
            baseAndInterfaces.AddRange(baseAndInterfaces);
            StringBuilder.AppendLine(baseAndInterfaces.Any()
                                          ? $": {string.Join(", ", baseAndInterfaces)}"
                                          : "");
            StringBuilder.AppendLine("{");
            return this;
        }

        public override Class EndBody()
        {
            StringBuilder.AppendLine("}");
            return this;
        }


    }

    public class MethodCSharp : Method<MethodCSharp>
    {
        public MethodCSharp(StringBuilder sb, string name, Scope scope = Scope.Public, bool isPartial = false, bool isStatic = false)
             : base(sb, name, scope, isPartial, isStatic) { }

        public override string Assign(string leftHand, string rightHand, string op = "=")
        {
            throw new NotImplementedException();
        }

        public override string Assign(string leftHand, string rightHand)
        {
            throw new NotImplementedException();
        }

        public override CodeBlock AssignStatement(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public override CodeBlock EndBody()
        {
            throw new NotImplementedException();
        }

        public override string Lambda(string lambdaDeclaration, string expression)
        {
            throw new NotImplementedException();
        }

        public override string LambdaDeclaration(params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public override string MethodCall(string methodName, params string[] args)
        {
            throw new NotImplementedException();
        }

        public override CodeBlock MethodCallStatement(string methodName, params string[] args)
        {
            throw new NotImplementedException();
        }

        public override string MultiLineLambda(string lambdaDeclaration, params string[] statements)
        {
            throw new NotImplementedException();
        }

        public override string NewObject(string objName, params string[] ctorArgs)
        {
            throw new NotImplementedException();
        }

        public override string Parameter(string paramType, string paramName)
        {
            throw new NotImplementedException();
        }

        public override CodeBlock ReturnStatement(bool await = false, params string[] returnValue)
        {
            throw new NotImplementedException();
        }

        public override CodeBlock ReturnStatement(params string[] returnValue)
        {
            throw new NotImplementedException();
        }

        public override CodeBlock StartBody()
        {
            throw new NotImplementedException();
        }
    }
}
