using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public class Constructor : StatementContainer<Constructor >, IConstructor
    {
        public Constructor()
        {        }

        private MethodCall? baseOrThisCall;

        public Constructor BaseOrThisCall(MethodCall methodCall)
        {
            baseOrThisCall = methodCall;
            return this;
        }

        public Constructor BaseOrThisCall(BaseOrThis baseOrThis, params string[] arguments )
        {
            baseOrThisCall = new MethodCall(baseOrThis, arguments);
            return this;
        }

        public Constructor BaseCall( params Expression[] arguments)
        {
            baseOrThisCall = new MethodCall(BaseOrThis.Base, arguments);
            return this;
        }

        public Constructor ThisCall( params Expression[] arguments)
        {
            baseOrThisCall = new MethodCall(BaseOrThis.This, arguments);
            return this;
        }
    }
}
