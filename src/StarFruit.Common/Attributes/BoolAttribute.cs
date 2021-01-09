using System;

namespace StarFruit2
{
    public abstract class BoolAttribute : Attribute
    {

        public bool Value { get; set; }

        protected BoolAttribute(bool value)
        {
            Value = value;
        }
    }
}
