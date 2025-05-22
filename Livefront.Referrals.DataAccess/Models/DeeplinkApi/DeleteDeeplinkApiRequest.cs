using System.Text.Json.Serialization;

namespace Livefront.Referrals.DataAccess.Models.DeeplinkApiRequests;

public class DeleteDeeplinkApiRequest
{
    [JsonPropertyName("third_party_id")]
    public int Id { get; set; }
    public DeleteDeeplinkApiRequest(DeepLink? deepLink)
    {
        if (deepLink != null) Id = deepLink.Id;
    }
}