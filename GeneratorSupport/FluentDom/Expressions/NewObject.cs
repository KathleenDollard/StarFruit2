namespace GeneratorSupport.FluentDom
{
    public class NewObject : MethodCall 
    {
        private TypeRep typeRep;
        private string[] arguments;

        public NewObject(TypeRep typeRep, string[] arguments)
        {
            this.typeRep = typeRep;
            this.arguments = arguments;
        }
    }
}