using System.Text.Json.Serialization;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.API.Models;

public class ReferralLinkDTO
{
    [JsonPropertyName("referral_link")]public string ReferralLink { get; set; }
    [JsonPropertyName("expiration_date")] public DateTime ExpirationDate { get; set; }
}