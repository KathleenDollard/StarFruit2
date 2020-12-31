using System;

namespace StarFruit2
{
    public   class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get;  }
    }
}
