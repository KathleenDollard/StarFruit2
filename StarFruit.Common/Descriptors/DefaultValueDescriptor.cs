namespace StarFruit2.Common.Descriptors
{
    public class DefaultValueDescriptor
    {

        public DefaultValueDescriptor(object? defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object? DefaultValue { get; set; }
    }
}
