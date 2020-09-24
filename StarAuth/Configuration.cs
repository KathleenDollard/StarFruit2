namespace StarAuth
{
    public class Configuration : DefaultConfiguration
    {
        // From the following, all files are created needed for auth
        // * They are isolated, so changing the value here can regenerate for a different auth style
        // * Generation recognizes if the user has supplied and implementation and uses it
        // * When the user wants to see the code, it's in the obj folder (normal source gen)
        public IAuth Authorization = new IndividualAuth
        {
            Authority = "ISaidSo",
            ClientId = "Bill"
        };
    }
}
