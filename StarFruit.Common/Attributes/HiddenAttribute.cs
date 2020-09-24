using System;

namespace StarFruit2
{
    public class HiddenAttribute : Attribute 
    {
        public HiddenAttribute(bool value = true)
        {
            Value = value;
        }

        public bool Value { get; set; }
    }
}
