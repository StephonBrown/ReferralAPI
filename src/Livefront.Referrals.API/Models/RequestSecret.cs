using System.Text.Json.Serialization;

namespace Livefront.Referrals.API.Models;

// This is necessary to be able to deserialize the request body

public class RequestSecret(string SecretCode, bool IsEmptyUserId = false)
{
        [JsonPropertyName("secret_code")]
        public string SecretCode { get; set; } = SecretCode;
        
        // This is used to determine if the user ID is empty or not for testing purposes.
        [JsonPropertyName("is_empty_user_id")]
        public bool IsEmptyUserId { get; set; } = IsEmptyUserId;
}