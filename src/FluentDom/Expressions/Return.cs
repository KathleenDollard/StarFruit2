namespace FluentDom
{
    public class Return : IExpression
    {
        public Return(IExpression returnExpression)
        {
            ReturnExpression = returnExpression;
        }

        public IExpression ReturnExpression { get; }
    }
}
