using System.Collections.Generic;

namespace GeneratorSupport.FluentDom
{
    public class MultilineLambda : Expression
    {

        public MultilineLambda()
        {
        }

        public MultilineLambda Parameter(TypeRep type, string name)
        {
            return this;
        }

        public MultilineLambda Parameter(string type, string name)
        {
            return this;
        }

        public MultilineLambda Statements(params Expression[] statements)
        {
            return this;
        }

        public MultilineLambda Statements(params IEnumerable<Expression>[] statements)
        {
            return this;
        }
        public MultilineLambda Return(Expression value)
        {
            return this;
        }

        public MultilineLambda Return(string value)
        {
            return this;
        }
    }
}