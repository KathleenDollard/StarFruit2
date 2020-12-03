using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class Constructor : MethodBase<Constructor>
    {

        public Constructor(TypeRep containingType, Scope scope = Scope.Public)
           :  base()
        {
            ContainingType = containingType;
            Scope = scope;
        }

        public TypeRep ContainingType { get; }

        private MethodCallBase? baseOrThisCall;

        public MethodCallBase? BaseOrThisCallStore
        => baseOrThisCall;

        public Constructor BaseOrThisCall(MethodCallBase methodCall)
        {
            baseOrThisCall = methodCall;
            return this;
        }

        public Constructor BaseCall()
        {
            baseOrThisCall = new BaseConstructorCall();
            return this;
        }

        public Constructor BaseCall(params string[] arguments)
        {
            baseOrThisCall = new BaseConstructorCall(arguments);
            return this;
        }

        public Constructor ThisCall()
        {
            baseOrThisCall = new ThisConstructorCall();
            return this;
        }

        public Constructor ThisCall(params string[] arguments)
        {
            baseOrThisCall = new ThisConstructorCall(arguments);
            return this;
        }

        public Constructor BaseCall(params IExpression[] arguments)
        {
            baseOrThisCall = new BaseConstructorCall(arguments);
            return this;
        }

        public Constructor ThisCall(params IExpression[] arguments)
        {
            baseOrThisCall = new ThisConstructorCall(arguments);
            return this;
        }
    }
}
