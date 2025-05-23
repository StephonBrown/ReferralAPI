using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Livefront.Referrals.DataAccess.Models;

[Table("Users")]
public class User
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [Required]
    [JsonPropertyName("first_name")]
    public string FirstName { get; init; }

    [Required]
    [JsonPropertyName("last_name")]
    public string LastName { get; init; }

    [Required] [JsonPropertyName("email")] public string Email { get; init; }

    [Required]
    [JsonPropertyName("referral_code")]
    public string ReferralCode { get; init; }

    public User() { }

    [method: JsonConstructor]
    public User(Guid id,
        string firstName,
        string lastName,
        string email,
        string referralCode)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        ReferralCode = referralCode;
    }
}