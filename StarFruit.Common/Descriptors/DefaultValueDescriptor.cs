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
            Int16 i => i.ToString(),
            Int32 i => i.ToString(),
            Int64 i => i.ToString(),
            bool b => b.ToString(),
            string s => $@"""{s}""",
            _ => throw new NotImplementedException()
        };

        public object? DefaultValue { get; set; }
    }
}
