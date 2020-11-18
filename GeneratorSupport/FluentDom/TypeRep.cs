using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public class TypeRep : ICtorPart
    {
        public static TypeRep Var { get; } = new TypeRep(true) { };
        public static TypeRep Implicit { get; } = new TypeRep(true) { };

        public TypeRep(string name)
        {
            Name = name;
        }
        public TypeRep(string name, params TypeRep[] genericTypeArguments)
            : this(name)
        {
            GenericTypeArguments.AddRange(genericTypeArguments);
        }
        public TypeRep(string name, params string[] genericTypeArguments)
              : this(name)
        {
            GenericTypeArguments.AddRange(genericTypeArguments.Select(x => new TypeRep(x)));
        }
        public string Name { get; init; }
        public bool IsImplicit { get; set; }

        private TypeRep(bool isImplicit)
        {
            IsImplicit = isImplicit;
            Name = "";
        }


        public List<TypeRep> GenericTypeArguments { get; } = new List<TypeRep>();
    }
}
