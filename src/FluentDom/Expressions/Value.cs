using System;

namespace FluentDom
{
    public abstract class Value : IExpression
    {
        protected Value(Type type, object? value)
        {
            Type = type;
            ValueStore = value;
        }

        public object? ValueStore { get; }

        public Type Type { get;  }
    }

    public class Value<T> : Value
    {
        public Value(T value)
            : base(typeof(T), value)
        { }

        public T? GetValue()
        => (T?)ValueStore;

        public override string ToString()
            => ValueStore switch
            {
                null => "null",
                string s => @$"""{s}""",
                _ => ValueStore.ToString()!
            };

    }
}
