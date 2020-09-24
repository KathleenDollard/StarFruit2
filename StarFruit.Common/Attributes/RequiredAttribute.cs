using System;

namespace StarFruit2
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RequiredAttribute : Attribute 
    {
        public RequiredAttribute(bool value = true)
        {
            Value = value;
        }

        public bool Value { get; set; }
    }
}
