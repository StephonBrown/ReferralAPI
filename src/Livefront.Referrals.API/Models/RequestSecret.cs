using System.Text.Json.Serialization;

namespace Livefront.Referrals.API.Models;

// This is necessary to be able to deserialize the request body

public class RequestSecret(string SecretCode)
{
        [JsonPropertyName("secret_code")]
        public string SecretCode { get; set; } = SecretCode;
}