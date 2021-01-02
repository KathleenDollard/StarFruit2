namespace FluentDom
{
    public  class Field : MemberBase
    {
        public Field(string name, TypeRep typeRep, Scope scope = Scope.Public, bool readOnly = false, MemberModifiers modifiers = MemberModifiers.None)
            :base(scope,modifiers)
        {
            Name = name;
            ReadOnly = readOnly;
            TypeRep = typeRep;
        }

        public Field(string name, string typeName, Scope scope = Scope.Public, bool readOnly = false)
            : this(name, new TypeRep(typeName), scope, readOnly)
        { }

        public string Name { get; }
        public TypeRep TypeRep { get;  }
        public bool ReadOnly { get;  }

    }
}
