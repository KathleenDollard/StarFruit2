//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace GeneratorSupport.Context
//{


//    public abstract class GenerationContext
//    {
//        protected GenerationContext(StringBuilder? sb = null)
//              => StringBuilder = sb ?? new StringBuilder();
//        internal StringBuilder StringBuilder { get; }

//    }

//    public abstract class GenerationEntryContext : GenerationContext
//    {
//        public GenerationEntryContext(StringBuilder? sb = null)
//            : base(sb) { }

//        public abstract string Base { get; }
//        public abstract string This { get; }
//        public abstract string Var { get; }
//        public abstract string GenericType(string type, params string[] genericArgs);
//    }
//    public abstract class GenerationEntryContext<T> : GenerationEntryContext
//        where T : GenerationEntryContext<T>
//    {
//        public GenerationEntryContext(StringBuilder? sb = null)
//             : base(sb) { }

//        public abstract Preamble Preamble();

//        public abstract Namespace Namespace(string name);

//        public abstract Class Class(string name,
//                                    Scope scope = Scope.Public,
//                                    bool isPartial = false,
//                                    bool isStatic = false);

//    }

//    public abstract class Preamble : GenerationContext
//    {
//        public Preamble(StringBuilder sb)
//            : base(sb) { }
//        public abstract Preamble Usings(params string[] nspaces);
//    }
//    public abstract class Preamble<T> : Preamble
//        where T : Preamble<T>
//    {
//        public Preamble(StringBuilder sb)
//           : base(sb) { }


//    }

//    public abstract class Namespace : GenerationContext
//    {
//        public Namespace(StringBuilder sb)
//            : base(sb) { }
//        public abstract Namespace StartBody();
//        public abstract Namespace EndBody();
//    }
//    public abstract class Namespace<T> : Namespace
//        where T : Namespace<T>
//    {
//        public Namespace(StringBuilder sb)
//           : base(sb) { }


//    }


//    public abstract class Class : GenerationContext
//    {
//        protected readonly string name;
//        protected string? baseClassName { get; private set; }
//        protected readonly Scope scope;
//        protected readonly bool isPartial;
//        protected readonly bool isStatic;
//        protected string[]? interfaces;

//        public Class(StringBuilder sb,
//                       string name,
//                       Scope scope = Scope.Public,
//                       bool isPartial = false,
//                       bool isStatic = false)
//              : base(sb)
//        {
//            this.name = name;
//            this.scope = scope;
//            this.isPartial = isPartial;
//            this.isStatic = isStatic;
//        }
//        public Class AddInterfaces(params string[] interfaces)
//        {
//            this.interfaces = interfaces;
//            return this;
//        }

//        public Class SetBaseClassName(string baseClassName)
//        {
//            this.baseClassName = baseClassName;
//            return this;
//        }

//        public abstract Class Ctor(string ctorDeclaration, string ctorBaseCall, params string[] ctorBody);
//        public abstract Class Field(string name, string type, Scope scope);
//        public abstract Class Method(Scope scope, string name, IEnumerable<string> methodBody, string returnType, bool isAsync = false, bool isOverriden = false, params string[] arguments);
//        public abstract Class Property(Scope scope, string type, string name, Scope setterScope);
//        public abstract Class StartBody();
//        public abstract Class EndBody();
//    }
//    public abstract class Class<T> : Class
//         where T : Class<T>
//    {

//        public Class(StringBuilder sb,
//                     string name,
//                     Scope scope = Scope.Public,
//                     bool isPartial = false,
//                     bool isStatic = false)
//            : base(sb, name, scope, isPartial, isStatic)
//        { }


//    }

//    public abstract class CodeBlock : GenerationContext
//    {
//        public CodeBlock(StringBuilder sb)
//            : base(sb) { }

//        public abstract string Assign(string leftHand, string rightHand, string op = "=");
//        public abstract string Assign(string leftHand, string rightHand);
//        public abstract string Lambda(string lambdaDeclaration, string expression);
//        public abstract string LambdaDeclaration(params string[] parameters);
//        public abstract string MethodCall(string methodName, params string[] args);
//        public abstract string MultiLineLambda(string lambdaDeclaration, params string[] statements);
//        public abstract string NewObject(string objName, params string[] ctorArgs);
//        public abstract CodeBlock MethodCallStatement(string methodName, params string[] args);
//        public abstract CodeBlock AssignStatement(string v1, string v2);
//        public abstract CodeBlock ReturnStatement(bool await = false, params string[] returnValue);
//        public abstract CodeBlock ReturnStatement(params string[] returnValue);
//        public abstract CodeBlock StartBody();
//        public abstract CodeBlock EndBody();

//    }

//    //Property accessors will not have parameters
//    public abstract class CodeBlockWithParameters : CodeBlock
//    {
//        public CodeBlockWithParameters(StringBuilder sb)
//             : base(sb) { }

//        public abstract string Parameter(string paramType, string paramName);
//    }

//    public abstract class Method : CodeBlockWithParameters
//    {
//        public Method(StringBuilder sb)
//             : base(sb) { }
//    }
//    public abstract class Method<T> : Method
//         where T : Method<T>
//    {
//        protected readonly string name;
//        protected readonly bool isPartial;
//        protected readonly bool isStatic;
//        protected readonly Scope Scope;

//        public Method(StringBuilder sb,
//                      string name,
//                      Scope scope = Scope.Public,
//                      bool isPartial = false,
//                      bool isStatic = false)
//             : base(sb)
//        {
//            this.name = name;
//            Scope = scope;
//            this.isPartial = isPartial;
//            this.isStatic = isStatic;
//        }

//    }

//    public abstract class Constructor : CodeBlockWithParameters
//    {
//        public Constructor(StringBuilder sb)
//             : base(sb) { }
//    }
//    public abstract class Constructor<T> : CodeBlockWithParameters
//         where T : Constructor<T>
//    {
//        public Constructor(StringBuilder sb)
//            : base(sb) { }

//        public abstract string CtorBaseCall(params string[] baseArgs);
//        public abstract string CtorDeclaration(string className, Scope scope = Scope.Public, params string[] ctorParameters);

//    }

//    public class MultiLineLambda : CodeBlockWithParameters
//    {
//        public MultiLineLambda(StringBuilder sb)
//             : base(sb) { }
//    }
//}
