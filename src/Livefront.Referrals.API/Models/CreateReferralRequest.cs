using System.Text.Json.Serialization;

namespace Livefront.Referrals.API.Models;

[method: JsonConstructor]
public class CreateReferralRequest(Guid refereeId, string referralCode)
{
    [JsonPropertyName("referee_id")] public Guid RefereeId { get; init; } = refereeId;
    [JsonPropertyName("referral_code")] public string ReferralCode { get; init; } = referralCode;
}