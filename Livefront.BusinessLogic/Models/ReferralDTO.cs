using System.Text.Json.Serialization;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.API.Models;

public class ReferralDTO
{
    [JsonPropertyName("first_name")] public string FirstName { get; init; }
    [JsonPropertyName("last_name")] public string LastName { get; init; }
    [JsonPropertyName("status")]public ReferralStatus Status { get; init; }
}