using System.Text.Json.Serialization;

namespace Livefront.BusinessLogic.Models;

public class ReferralLinkDTO
{
    [JsonPropertyName("referral_link")]public string ReferralLink { get; set; }
    [JsonPropertyName("expiration_date")] public DateTime ExpirationDate { get; set; }
}