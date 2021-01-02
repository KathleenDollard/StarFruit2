using System.Linq;

namespace FluentDom
{
    public class Expression
    {
        public static Assign Assign(IExpression leftHand, IExpression rightHand) 
            => new Assign(leftHand, rightHand);

        public static Assign Assign(IExpression leftHand, string rightHand) 
            => new Assign(leftHand, VariableReference(rightHand));

        public static Assign Assign(string leftHand, IExpression rightHand) 
            => new Assign(VariableReference(leftHand), rightHand);

        public static Assign Assign(string leftHand, string rightHand) 
            => new Assign(VariableReference(leftHand), VariableReference(rightHand));

        public static LocalDeclaration AssignVar(string localName, TypeRep typeRep, IExpression rightHand) 
            => new LocalDeclaration(localName, typeRep, rightHand);

        public static MethodCall MethodCall(string name, params IExpression[] arguments) 
            => new MethodCall(name, arguments);

        public static MethodCall MethodCall(string name, params string[] arguments) 
            => new MethodCall(name, arguments.Select(x => VariableReference(x)).ToArray());

        public static MethodCall MethodCall(string name)
            => new MethodCall(name);

        public static MultilineLambda MultilineLambda()
            => new MultilineLambda();
        public static NewObject NewObject(TypeRep typeRep)
           => new NewObject(typeRep, new IExpression[] { });

        public static NewObject NewObject(TypeRep typeRep, params IExpression[] arguments)
                => new NewObject(typeRep, arguments);

        public static NewObject NewObject(TypeRep typeRep, params string[] arguments)
            => NewObject(typeRep, arguments.Select(x => VariableReference(x)).ToArray());

        public static NewObject NewObject(string typeRep)
            => NewObject(new TypeRep(typeRep), new IExpression[] { });

        public static NewObject NewObject(string typeRep, params IExpression[] arguments)
            => new NewObject(new TypeRep(typeRep), arguments);

        public static NewObject NewObject(string typeRep, params string[] arguments)
            => NewObject(new TypeRep(typeRep), arguments);

        public static This This() 
            => new This();

        public static Null Null() 
            => new Null();

        public static Base Base()
            => new Base();

        public static Value Value<T>(T value) 
            => new Value<T>(value);

        public static VariableReference VariableReference(string variableName) 
            => new VariableReference(variableName);

        public static Dot Dot(IExpression left, string right) 
            => new Dot(left, right);

        public static Dot Dot(string left, string right) 
            => new Dot(VariableReference(left), right);

        public static As As(IExpression expression, TypeRep typeRep) 
            => new As(expression, typeRep);

        public static Return Return(IExpression expression)
            => new Return(expression);
    }
}
