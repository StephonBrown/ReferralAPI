using System.Text.Json.Serialization;

namespace Livefront.Referrals.API.Models;

[method: JsonConstructor]
public class User(
    Guid id,
    string firstName,
    string lastName,
    string email,
    string referralCode
)
{
    [JsonPropertyName("id")] public Guid Id { get; init; } = id;
    [JsonPropertyName("first_name")] public string FirstName { get; init; } = firstName;
    [JsonPropertyName("last_name")] public string LastName { get; init; } = lastName;
    [JsonPropertyName("email")] public string Email { get; init; } = email;
    [JsonPropertyName("referral_code")] public string ReferralCode { get; init; } = referralCode;
}