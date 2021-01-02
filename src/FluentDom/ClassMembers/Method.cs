namespace FluentDom
{
    public class Method : MethodBaseWithReturn<Method>
    {

        public string Name { get; }
 
        public Method(string name, Scope scope = Scope.Public, MemberModifiers modifiers = MemberModifiers.None)
            : base(scope, modifiers)
        {
            Name = name;
        }
    }
}
