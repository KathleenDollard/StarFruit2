﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDom
{
    public class StatementContainer<T> : MemberBase
        where T : StatementContainer<T>
    {
        private List<IExpression> statements = new List<IExpression>();

        public StatementContainer(Scope scope = Scope.Public, MemberModifiers modifiers = MemberModifiers.None)
           : base(scope, modifiers)
        { }

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

        public T OptionalStatements(bool condition, params Func<IExpression>[] expressionMakers)
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

        public void ParameterWithDefault(string name, TypeRep type, IExpression defaultExpression)
        {
            parameters.Add(new Parameter(name, type, defaultExpression));
        }


        public void Parameter(Parameter parameter)
        {
            parameters.Add(parameter);
        }
    }

    public abstract class MethodBase<T> : StatementContainer<T>
        where T : MethodBase<T>
    {
        protected MethodBase(Scope scope = Scope.Public, MemberModifiers modifiers = MemberModifiers.None)
            : base(scope, modifiers)
        { }

        public ParameterStore ParameterStore { get; } = new ParameterStore();

        public T Parameter(string name, TypeRep type)
        {
            ParameterStore.Parameter(name, type);
            return (T)this;
        }

        public T ParameterWithDefault(string name, TypeRep type, IExpression defaultExpression)
        {
            ParameterStore.ParameterWithDefault(name, type, defaultExpression);
            return (T)this;
        }

        public T Parameters<TItem>(IEnumerable<TItem> items, Func<TItem, Parameter> parameterMaker)
        {
            foreach (var item in items)
            {
                ParameterStore.Parameter(parameterMaker(item));
            }
            return (T)this;
        }

    }

    public abstract class MethodBaseWithReturn<T> : MethodBase<T>
        where T : MethodBaseWithReturn<T>
    {
        public MethodBaseWithReturn(Scope scope, MemberModifiers modifiers = MemberModifiers.None)
            : base(scope, modifiers)
        { }

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