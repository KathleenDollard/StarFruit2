namespace FluentDom
{
    public class Dot : IExpression
    {
        public Dot(IExpression left, string right)
        {
            Left = left;
            Right = right;
        }

        public IExpression Left { get; }
        public string Right { get; }
    }
}
