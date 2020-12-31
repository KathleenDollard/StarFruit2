using System;
using System.Collections.Generic;
using System.Text;

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
