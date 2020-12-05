namespace FluentDom
{
    public class Assign : IExpression
    {
   
        public Assign(IExpression leftHand, IExpression expression)
        {
            LeftHand = leftHand;
            Expression = expression;
        }

        public IExpression LeftHand { get; }
        public IExpression Expression { get; }
    }
}