using System.Text.Json.Serialization;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.API.Models;

public class ReferralDTO(string firstName, string lastName)
{
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = firstName;
    [JsonPropertyName("last_name")] public string LastName { get; set; } = lastName;
    [JsonPropertyName("status")]public ReferralStatus Status { get; set; } = ReferralStatus.Complete;
}