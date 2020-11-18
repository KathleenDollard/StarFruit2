using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratorSupport.FluentDom
{
    public class StatementContainer<T> : MemberBase 
        where T : StatementContainer<T>
    {
        private List<Expression> statements = new List<Expression>();
        private List<Parameter> parameters = new List<Parameter>();

        public T Parameter(string name, TypeRep type)
        {
            parameters.Add(new Parameter(name,type));
            return (T)this;
        }

        public T Parameter(string name, string type)
        {
            parameters.Add(new Parameter(name, new TypeRep(type)));
            return (T)this;
        }

        public T Statements(params Expression[] expressions)
        {
            statements.AddRange(expressions);
            return (T)this;
        }

        public T Statements(params IEnumerable<Expression>[] expressions)
        {
            statements.AddRange(expressions.SelectMany(x => x));
            return (T)this;
        }

        public T Statements<TItem>(IEnumerable<TItem> items, params Func<TItem, Expression>[] expressionMakers)
        {
            statements.AddRange(items.SelectMany(
                            item => expressionMakers.Select(lambda => lambda(item))));
            return (T)this;
        }

        public T Statements(params object[] expressions)
        {
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case IEnumerable<Expression> ex:
                        Statements(ex);
                        break;
                    case Expression ex:
                        Statements(ex);
                        break;
                    default:
                        break;
                }
            }
            return (T)this;
        }

        public T Optional(bool condition, Func< Expression> expressionMaker)
        {
            if (condition)
            {
                statements.Add(expressionMaker());
            }
            throw new NotImplementedException();
        }
    }

    public class MethodBaseWithReturn<T> : StatementContainer<T>
        where T : MethodBaseWithReturn<T>
    {

        public T Return(Expression value)
        {
            return (T)this;
        }

        public T Return(string value)
        {
            return (T)this;
        }
    }
}