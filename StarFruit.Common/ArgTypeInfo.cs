using System;
using System.Net.Mime;

namespace StarFruit2.Common
{
    /// <summary>
    /// Derived classes supply specific technology implementation for the 
    /// ArgumentType. 
    /// <br/>
    /// For exmple: In Reflection, this provides a Type object. In Roslyn it provides
    /// a syntax node that represents the type.
    /// </summary>
    public class ArgTypeInfo
    {
        public ArgTypeInfo(object? typeRepresentation)
        {
            TypeRepresentation = typeRepresentation;
        }

        public object? TypeRepresentation { get; }


        public T GetArgumentType<T>()
            where T : class 
            => TypeRepresentation switch
            {
                T t => t,
                _ => throw new NotImplementedException("Add other outputs like a Roslyn SyntaxNode")
            };

     }
}
