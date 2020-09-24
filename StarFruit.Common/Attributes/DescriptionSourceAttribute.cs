using System;

namespace StarFruit2.Common
{
    public  class DescriptionSourceAttribute : Attribute 
    {
        public DescriptionSourceAttribute(Type descriptionSource)
        {
            if (!typeof(IDescriptionSource ).IsAssignableFrom (descriptionSource ))
            {
                throw new InvalidOperationException("Description source must implement IDescriptionSource interface");
            }
            DescriptionSource = descriptionSource;
        }

        public Type DescriptionSource { get; }
    }
}
