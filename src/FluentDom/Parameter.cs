namespace FluentDom
{
    public class Parameter 
    {
        public  Parameter(string name, TypeRep type)
        {
            Type = type;
            Name = name;
        }

        public Parameter(string name, TypeRep type, IExpression defaultExpression)
        {
            Type = type;
            Name = name;
            HasDefault = true;
            DefaultExpression = defaultExpression;
        }

        public bool HasDefault { get; }
        public IExpression? DefaultExpression { get; }
        public TypeRep Type { get; }
        public string Name { get; }
    }
}
