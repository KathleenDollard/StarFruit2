using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public class Expression : ICtorPart
    {
        public static NewObject NewObject(TypeRep typeRep, params Expression[] arguments)
        {
            throw new NotImplementedException();
        }

        public static NewObject NewObject(string typeRep, params Expression[] arguments)
        {
            throw new NotImplementedException();
        }

        public static NewObject NewObject(string typeRep, params string[] arguments)
        {
            throw new NotImplementedException();
        }

        public static MethodCall MethodCall(string Name, params Expression[] arguments)
        {
            throw new NotImplementedException();
        }

        public static MethodCall MethodCall(string Name, params string[] arguments)
        {
            throw new NotImplementedException();
        }

        public static MethodCall MethodCall(string Name)
        {
            throw new NotImplementedException();
        }

        public static Assign Assign(string leftHand, Expression rightHand)
        {
            throw new NotImplementedException();
        }

        public static Assign Assign(string leftHand, string rightHand)
        {
            throw new NotImplementedException();
        }

        public static LocalDeclaration AssignVar(string localName, Expression rightHand)
        {
            throw new NotImplementedException();
        }

        public static Expression This()
        {
            throw new NotImplementedException();
        }

        public static Expression Base()
        {
            throw new NotImplementedException();
        }

        public static MultilineLambda MultilineLambda()
        {
            throw new NotImplementedException();
        }

        public static Value Value(object value)
        {
            throw new NotImplementedException();
        }

    }
}
