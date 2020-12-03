using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class Property : MemberBase
    {
        private PropertySetter setterStatements = new();
        private PropertyGetter getterStatements = new();

        public Property(string name, TypeRep typeRep, Scope scope = Scope.Public, bool readOnly = false)
        {
            Name = name;
            Scope = scope;
            ReadOnly = readOnly;
            TypeRep = typeRep;
        }

        public Property(string name, string typeName, Scope scope = Scope.Public, bool readOnly = false)
            : this(name,  new TypeRep(typeName), scope, readOnly)
        { }

        public string Name { get; init; }
        public TypeRep TypeRep { get; init; }
        public PropertyGetter GetterStatementStore
            => getterStatements;
        public PropertySetter SetterStatementStore
            => setterStatements;
        public bool ReadOnly { get; internal set; }

        public Property GetterStatements(params IExpression[] statements)
        {
            getterStatements.Statements(statements);
            return this;
        }

        public Property GetterReturn( IExpression returnExpression)
        {
            if(returnExpression is not Return)
            {
                returnExpression = new Return(returnExpression);
            }
            getterStatements.Statements(returnExpression);
            return this;
        }

        public Property SetterStatements(params IExpression[] statements)
        {
            setterStatements.Statements(statements);
            return this;
        }
    }
}
