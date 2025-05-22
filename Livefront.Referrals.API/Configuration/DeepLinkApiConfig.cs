using System.Security;

namespace Livefront.Referrals.API.Configuration;

public class DeepLinkApiConfig
{
    public string BaseAddress { get; set; }
    public string ApiToken { get; set; }
    public string ApiTokenSecret { get; init; }
    
    public DeepLinkApiConfig(string baseAddress, string apiToken, string apiTokenSecret)
    {
        BaseAddress = baseAddress;
        ApiToken = apiToken;
        ApiTokenSecret = apiTokenSecret;
    }
}