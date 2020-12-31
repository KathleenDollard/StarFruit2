namespace FluentDom
{
    public class VariableReference : IExpression
    {
        public VariableReference(string value)
        {
            ValueStore = value;
        }

        public string ValueStore { get; }

    }
}