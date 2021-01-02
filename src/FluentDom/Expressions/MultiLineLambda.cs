namespace FluentDom
{
    public class MultilineLambda : MethodBaseWithReturn<MultilineLambda>, IExpression
    {
        public MultilineLambda() 
            : base(Scope.Public , MemberModifiers.None)
        { }
    }
}