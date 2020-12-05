using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class Expression
    {
        public static Assign Assign(IExpression leftHand, IExpression rightHand)
        {
            return new Assign(leftHand, rightHand);
        }

        public static Assign Assign(IExpression leftHand, string rightHand)
        {
            return new Assign(leftHand, VariableReference(rightHand));
        }

        public static Assign Assign(string leftHand, IExpression rightHand)
        {
            return new Assign(VariableReference(leftHand), rightHand);
        }

        public static Assign Assign(string leftHand, string rightHand)
        {
            return new Assign(VariableReference(leftHand), VariableReference(rightHand));
        }

        public static LocalDeclaration AssignVar(string localName, TypeRep typeRep, IExpression rightHand)
        {
            return new LocalDeclaration(localName, typeRep, rightHand);
        }

        public static MethodCall MethodCall(string name, params IExpression[] arguments)
        {
            return new MethodCall(name, arguments);
        }

        public static MethodCall MethodCall(string name, params string[] arguments)
        {
            return new MethodCall(name, arguments.Select(x => VariableReference(x)).ToArray());
        }

        public static MethodCall MethodCall(string name)
        {
            return new MethodCall(name);
        }

        public static MultilineLambda MultilineLambda()
        {
            return new MultilineLambda();
        }
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
        {
            return new This();
        }

        public static Base Base()
        {
            return new Base();
        }

        public static Value Value<T>(T value)
        {
            return new Value<T>(value);
        }

        public static VariableReference VariableReference(string variableName)
        {
            return new VariableReference(variableName);
        }

        public static Dot Dot(IExpression left, string right)
        {
            return new Dot(left, right);
        }

        public static Dot Dot(string left, string right)
        {
            return new Dot(VariableReference(left), right);
        }

        public static Return Return(IExpression expression)
        {
            return new Return(expression);
        }
    }
}
