namespace FluentDom
{
    public class As:IExpression 
    {
        public As(IExpression expression, TypeRep typeRep)
        {
            Expression = expression;
            TypeRep = typeRep;
        }

        public IExpression Expression { get; }
        public TypeRep TypeRep { get; }
    }
}