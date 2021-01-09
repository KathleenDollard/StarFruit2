using System;

namespace StarFruit2.Common
{


    /// <summary>
    /// Derived classes supply specific technology implementation for the 
    /// ArgumentType. 
    /// <br/>
    /// For exmple: In Reflection, this provides a Type object. In Roslyn it provides
    /// a syntax node that represents the type.
    /// </summary>
    public abstract class ArgTypeInfoBase
    {
        public ArgTypeInfoBase(object? typeRepresentation)
        {
            TypeRepresentation = typeRepresentation;
        }

        public object? TypeRepresentation { get; }

        public virtual string TypeAsString()
        => TypeRepresentation switch
        {
            Type t => t.Name,
            _ => TypeRepresentation?.ToString() ?? ""
        };
    }

}