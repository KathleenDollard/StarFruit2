using System;

namespace StarFruit2
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Struct)]
    public class TreatUnmatchedTokensAsErrorsAttribute : BoolAttribute
    {
        public TreatUnmatchedTokensAsErrorsAttribute(bool value = true)
             : base(value)
        { }     
    }
}
