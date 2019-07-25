namespace DotNetAuthorizeFunction.Config
{
    public class Settings
    {
        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string MetadataUrl { get; set; }
    }
}