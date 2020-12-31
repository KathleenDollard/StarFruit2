namespace FluentDom
{
    public class LocalDeclaration : IExpression
    {
        public LocalDeclaration(string localName, TypeRep typeRep, IExpression rightHand)
        {
            LocalName = localName;
            TypeRep = typeRep;
            RightHand = rightHand;
        }

        public string LocalName { get; }
        public TypeRep TypeRep { get; }
        public IExpression RightHand { get; }
    }
}
