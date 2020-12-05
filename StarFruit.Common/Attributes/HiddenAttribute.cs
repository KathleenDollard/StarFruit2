using System;

namespace StarFruit2
{
    public class HiddenAttribute : BoolAttribute 
    {
        public HiddenAttribute(bool value = true)
            : base(value)
        { }
  
    }
}
