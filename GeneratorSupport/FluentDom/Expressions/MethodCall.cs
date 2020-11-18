namespace GeneratorSupport.FluentDom
{
    public class MethodCall : Expression
    {
        private BaseOrThis baseOrThis;
        private Expression[] arguments;

        public MethodCall(BaseOrThis baseOrThis, Expression[] arguments)
        {
            this.baseOrThis = baseOrThis;
            this.arguments = arguments;
        }
    }
}