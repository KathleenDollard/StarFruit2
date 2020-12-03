using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDom
{
    public class StatementContainer<T> : MemberBase
        where T : StatementContainer<T>
    {
        private List<IExpression> statements = new List<IExpression>();

        public IEnumerable<IExpression> StatementStore
        => statements;

        public bool AnyStatements()
        => statements.Any();

        public int StatementCount()
        => statements.Count();

        public T Statements(params IExpression[] expressions)
        {
            statements.AddRange(expressions);
            return (T)this;
        }

        public T Statements(params IEnumerable<IExpression>[] expressions)
        {
            statements.AddRange(expressions.SelectMany(x => x));
            return (T)this;
        }

        public T Statements<TItem>(IEnumerable<TItem> items, params Func<TItem, IExpression>[] expressionMakers)
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

        public T OptionalStatements(bool condition, params  Func<IExpression>[] expressionMakers)
        {
            if (condition)
            {
                foreach (var expressionMaker in expressionMakers)
                {
                    statements.Add(expressionMaker());
                }
            }
            return (T)this;
        }
    }

    public class ParameterStore
    {
        private List<Parameter> parameters = new List<Parameter>();

        public IEnumerable<Parameter> Parameters
        => parameters;

        public void Parameter(string name, TypeRep type)
        {
            parameters.Add(new Parameter(name, type));
        }

        public void Parameter(string name, string type)
        {
            parameters.Add(new Parameter(name, new TypeRep(type)));
        }

        public void Parameter(string name, Type type)
        {
            parameters.Add(new Parameter(name, new TypeRep(type)));
        }
    }

    public abstract class MethodBase<T> : StatementContainer<T>
        where T : MethodBase<T>
    {
        public ParameterStore ParameterStore { get; } = new ParameterStore();

        public T Parameter(string name, TypeRep type)
        {
            ParameterStore.Parameter(name, type);
            return (T)this;
        }

        public T Parameter(string name, string type)
        {
            ParameterStore.Parameter(name, type);
            return (T)this;
        }

        public T Parameter(string name, Type type)
        {
            ParameterStore.Parameter(name, type);
            return (T)this;
        }
    }

    public abstract class MethodBaseWithReturn<T> : MethodBase<T>
        where T : MethodBaseWithReturn<T>
    {

        public Return? ReturnStore { get; private set; }
        public TypeRep? ReturnTypeStore { get; private set; }

        public T ReturnType(TypeRep typeRep)
        {
            ReturnTypeStore = typeRep;
            return (T)this;
        }
    }

    public class PropertyGetter : StatementContainer<PropertyGetter>
    { }

    public class PropertySetter : StatementContainer<PropertySetter>
    { }

}