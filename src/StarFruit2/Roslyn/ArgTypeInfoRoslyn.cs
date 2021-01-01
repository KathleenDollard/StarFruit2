using StarFruit2.Common;

namespace StarFruit2
{


    /// <summary>
    /// Derived classes supply specific technology implementation for the 
    /// ArgumentType. 
    /// <br/>
    /// For exmple: In Reflection, this provides a Type object. In Roslyn it provides
    /// a syntax node that represents the type.
    /// </summary>
    public class ArgTypeInfoRoslyn : ArgTypeInfoBase
    {
        public ArgTypeInfoRoslyn(object? typeRepresentation)
            : base(typeRepresentation)
        { }

        public override string TypeAsString()
        => LanguageHelper.ArgTypeInfoAsString(TypeRepresentation );
    }

}