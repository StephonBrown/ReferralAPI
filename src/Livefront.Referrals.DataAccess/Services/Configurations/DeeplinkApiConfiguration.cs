using System.Security;

namespace Livefront.Referrals.DataAccess.Services.Configurations;

public class DeeplinkApiConfiguration
{
    public string BaseAddress { get; set; }
    public SecureString ApiToken { get; set; }
    public string ApiTokenSecret { get; set; }
}