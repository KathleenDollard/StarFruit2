using System;

namespace StarFruit2.Common.Descriptors
{
    public class DefaultValueDescriptor
    {

        public DefaultValueDescriptor(object? defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public string CodeRepresentation => DefaultValue switch
        {
            short i => i.ToString(),
            int i => i.ToString(),
            long i => i.ToString(),
            bool b => b.ToString(),
            string s => $@"""{s}""",
            _ => throw new NotImplementedException()
        };

        public object? DefaultValue { get; set; }
    }
}
