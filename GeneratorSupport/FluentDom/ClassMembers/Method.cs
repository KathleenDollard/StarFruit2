using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public class Method : StatementContainer<Method>
    {

        public string Name { get; init; }
        public Expression ReturnStore { get; private set; }
        public TypeRep ReturnTypeStore { get; private set; }


        public Method(string name)
        {
            Name = name;
        }

        public List<Parameter> Parameters { get; } = new List<Parameter>();
        public bool IsAsync { get; init; }

        public Method ReturnType(TypeRep typeRep)
        {
            ReturnTypeStore = typeRep;
            return this;
        }

        public Method Return(Expression expression)
        {
            ReturnStore = expression;
            return this;
        }

        public Method Return(string expression)
        {
            ReturnStore = new Value(expression);
            return this;
        }


    }
}
