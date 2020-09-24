using System;

namespace StarFruit2
{
    public   class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get;  }
    }
}
