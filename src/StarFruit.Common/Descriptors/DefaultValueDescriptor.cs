using System;

namespace StarFruit2.Common.Descriptors
{
    public class DefaultValueDescriptor
    {

        public DefaultValueDescriptor(object? defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public virtual string CodeRepresentation => DefaultValue switch
        {
            short i => i.ToString(),
            int i => i.ToString(),
            long i => i.ToString(),
            bool b => b.ToString(),
            string s => $@"""{s}""",
            _ => throw new NotImplementedException($"Not implementd DefaultValue type in {nameof(DefaultValueDescriptor)}.{nameof(CodeRepresentation)}")
        };

        public object? DefaultValue { get; set; }
    }

    public class ExplicitDefaultValueDescriptor : DefaultValueDescriptor
    {
        private string explicitDefaultValue;
        public ExplicitDefaultValueDescriptor(string defaultValue)
            : base(defaultValue)
        {
            explicitDefaultValue = defaultValue;
        }

        public override string CodeRepresentation => explicitDefaultValue;

    }
}
