namespace FluentDom
{
    public class NewObject : MethodCallBase 
    {

        public NewObject(TypeRep typeRep, IExpression[] arguments)
            : base(arguments)
        {
            TypeRep = typeRep;
            Arguments = arguments;
        }

        public TypeRep TypeRep { get; }
        public IExpression[] Arguments { get; }
    }
}