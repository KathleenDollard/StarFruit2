using System;

namespace StarFruit2
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RequiredAttribute : BoolAttribute 
    {
        public RequiredAttribute(bool value = true)
            : base(value)
        { }
    }
}
