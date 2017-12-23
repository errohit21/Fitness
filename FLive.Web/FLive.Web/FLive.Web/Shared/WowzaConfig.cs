namespace FLive.Web.Shared
{
    public class WowzaConfig
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string AccessKey { get; set; }
    }

    public class StripeConfig
    {
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}