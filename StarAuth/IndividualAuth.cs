namespace StarAuth
{
    // These are from the blazorwasm template
    public class IndividualAuth : IAuth
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
    }

    public class SingleOrg : IAuth
    {
        public string AadInstance { get; set; }
        public string ClientId { get; set; }
        public string AppIdUri { get; set; }
        public string AppClientId { get; set; }
        public string DefaultScope { get; set; }
        public string TenantId { get; set; }
        public string OrgReacAccess { get; set; }

    }
}