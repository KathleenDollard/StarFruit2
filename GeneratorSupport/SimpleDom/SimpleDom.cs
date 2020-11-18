using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.SimpleDom
{
    public class Entry
    {
        public string? NamespaceName { get; init; }
        public List<string> Usings { get; } = new List<string>();
        public List<Class> Classes { get; } = new List<Class>();
    }

    public abstract class Member
    {
        public Scope Scope { get; init; }
        public bool IsStatic { get; init; }
    }

    public abstract class CodeBlock
    {
        public List<Expression> Statements { get; } = new List<Expression>();
    }

    public abstract class Expression
    { }

    public class Parameter
    {
        public Parameter(string name, string typeName = null)
        {
            Name = name;
            if (typeName is not null)
            {
                TypeRepresentation = new TypeRep(typeName);
            }
        }
        public Parameter(string name, TypeRep typeRepresentation)
        {
            Name = name;
            TypeRepresentation = typeRepresentation;
        }
        public string Name { get; init; }
        public TypeRep TypeRepresentation { get; init; }
    }

    public class TypeRep
    {
        public static TypeRep Var { get; } = new TypeRep(true) { };
        public static TypeRep Implicit { get; } = new TypeRep(true) { };
      
        public TypeRep(string name)
        {
            Name = name;
        }
        public TypeRep(string name, params TypeRep[] genericTypeArguments)
            : this(name)
        {
            GenericTypeArguments.AddRange(genericTypeArguments);
        }
        public TypeRep(string name, params string[] genericTypeArguments)
              : this(name)
        {
            GenericTypeArguments.AddRange(genericTypeArguments.Select(x => new TypeRep(x)));
        }
        public string Name { get; init; }
        public bool IsImplicit { get; set; }

        private TypeRep(bool isImplicit)
        {
            IsImplicit = isImplicit;
            Name = "";
        }


        public List<TypeRep> GenericTypeArguments { get; } = new List<TypeRep>();
    }

    public class Class : Member //  member to support nested classes
    {
        public string Name { get; init; }

        public Class(string name)
        {
            Name = name;
        }

        public TypeRep? BaseClass { get; init; }
        public bool IsTopLevel { get; init; }
        public List<Member> Members { get; } = new List<Member>();
    }

    public class Method : Member
    {

        public string Name { get; init; }

        public Method(string name)
        {
            Name = name;
        }

        public List<Parameter> Parameters { get; } = new List<Parameter>();
        public bool IsAsync { get; init; }
        public List<Expression> Statements { get; } = new List<Expression>();

    }

    public class Constructor : Member
    {
        public List<Parameter> Parameters { get; } = new List<Parameter>();
        public BaseOrThis BaseOrThisCalled { get; init; }
        public List<Expression> BaseConstructorArguments { get; } = new List<Expression>();
        public List<Expression> Statements { get; } = new List<Expression>();
    }

    public class Field : Member
    {
        public string Name { get; init; }
        public TypeRep TypeRepresentation { get; init; }
        public bool IsReadOnly { get; init; }
    }

    public class Property : Member
    {
        public string Name { get; init; }

        public Property(string name, TypeRep typeRepresentation, Scope scope = Scope.Public)
        {
            Name = name;
            Scope = scope;
            TypeRepresentation = typeRepresentation;
        }

        public Property(string name, string typeName, Scope scope = Scope.Public)
        {
            Name = name;
            Scope = scope;
            if (typeName is not null)
            {
                TypeRepresentation = new TypeRep(typeName);
            }
        }

        public TypeRep TypeRepresentation { get; init; }
        public List<Expression> GetterStatements { get; } = new List<Expression>();
        public List<Expression> SetterStatements { get; } = new List<Expression>();
    }

    public class LocalDeclaration : Expression
    {
        public string Name { get; init; }

        public LocalDeclaration(string name, TypeRep typeRepresentation)
        {
            Name = name;
            TypeRepresentation = typeRepresentation;
        }

        public LocalDeclaration(string name, TypeRep typeRepresentation, Expression value)
        {
            Name = name;
            TypeRepresentation = typeRepresentation;
            Value = value;
        }

        public TypeRep TypeRepresentation { get; init; }
        public Expression? Value { get; init; }
    }

    public class MethodCall : Expression
    {
        public MethodCall(string name)
        {
            Name = name;
        }
        public MethodCall(string name, params Expression[] arguments)
            : this(name)
        {
            Arguments.AddRange(arguments);
        }
        public MethodCall(string name, params string[] arguments)
            : this(name)
        {
            Arguments.AddRange(arguments.Select(x => new Direct(x)));
        }
        public string Name { get; init; }
        public List<Expression> Arguments { get; } = new List<Expression>();
    }

    public class NewObject : Expression
    {
        public NewObject(TypeRep typeRepresentation, params string[] arguments)
        {
            NewObjectType = typeRepresentation;
            Arguments.AddRange(arguments);
        }
        TypeRep NewObjectType { get; init; }
        public List<string> Arguments { get; } = new List<string>();
    }

    public class Assignment : Expression
    {
        public string Name { get; init; }

        public Assignment(string name, Expression value, BaseOrThis baseOrThis = BaseOrThis.Neither)
        {
            Name = name;
            Value = value;
            BaseOrThis = baseOrThis;
        }

        public Assignment(string name, string value, BaseOrThis baseOrThis = BaseOrThis.Neither)
        {
            Name = name;
            Value = new Direct(value);
            BaseOrThis = baseOrThis;
        }

        public Expression Value { get; init; }
        public BaseOrThis BaseOrThis { get; }
    }

    public class Direct : Expression
    {
        public Direct(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class This : Expression
    { }
    public class Base : Expression
    { }

    public class Return : Expression
    {
        public Expression Value { get; init; }

        public Return(Expression value)
        {
            Value = value;
        }

        public Return(string value)
        {
            Value = new Direct(value);
        }

        public class MultilineLambda : Expression
        {
            public MultilineLambda(List<string> arguments, params Expression[] statements)
            {
                Arguments = arguments;
                Statements = statements;
            }
            public List<string> Arguments { get; } = new List<string>();
            public List<Expression> Statements { get; } = new List<Expression>();

        }
    }
}
