using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
  public  class Field : MemberBase
    {
        public Field(string name, TypeRep typeRep, Scope scope = Scope.Public, bool readOnly = false)
        {
            Name = name;
            Scope = scope;
            ReadOnly = readOnly;
            TypeRep = typeRep;
        }

        public Field(string name, string typeName, Scope scope = Scope.Public, bool readOnly = false)
            : this(name, new TypeRep(typeName), scope, readOnly)
        { }

        public string Name { get; init; }
        public TypeRep TypeRep { get; init; }
        public bool ReadOnly { get; internal set; }

    }
}
