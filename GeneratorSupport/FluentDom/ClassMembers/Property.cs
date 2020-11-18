using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public class Property : MemberBase
    {
        public string Name { get; init; }

        public Property(string name, TypeRep typeRepresentation, Scope scope = Scope.Public)
        {
            Name = name;
            Scope = scope;
            TypeRepresentation = typeRepresentation;
        }

        public Property(string name, string typeName, Scope scope = Scope.Public)
        {
            Name = name;
            Scope = scope;
            if (typeName is not null)
            {
                TypeRepresentation = new TypeRep(typeName);
            }
        }

        public TypeRep TypeRepresentation { get; init; }
        public List<Expression> GetterStatements { get; } = new List<Expression>();
        public List<Expression> SetterStatements { get; } = new List<Expression>();
    }
}
