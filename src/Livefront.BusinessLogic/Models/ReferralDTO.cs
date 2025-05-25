using System.Text.Json.Serialization;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.BusinessLogic.Models;

public class ReferralDTO
{
    [JsonPropertyName("referral_id")] public Guid ReferralId { get; init; }
    [JsonPropertyName("user_id")] public Guid UserId { get; init; }
    [JsonPropertyName("first_name")] public string FirstName { get; init; }
    [JsonPropertyName("last_name")] public string LastName { get; init; }
    [JsonPropertyName("status")]public ReferralStatus Status { get; init; }
}