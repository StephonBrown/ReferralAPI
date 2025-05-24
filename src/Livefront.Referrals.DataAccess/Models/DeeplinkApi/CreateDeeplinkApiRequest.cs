using System.Text.Json.Serialization;

namespace Livefront.Referrals.DataAccess.Models.DeeplinkApi;

public class CreateDeeplinkApiRequest
{
    [JsonPropertyName("referral_code")]
    public string UserReferralCode { get; set; }

    public CreateDeeplinkApiRequest(string userReferralCode)
    {
        UserReferralCode = userReferralCode;
    }
}