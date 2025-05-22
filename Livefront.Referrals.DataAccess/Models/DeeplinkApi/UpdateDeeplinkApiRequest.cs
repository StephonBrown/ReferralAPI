using System.Text.Json.Serialization;

namespace Livefront.Referrals.DataAccess.Models.DeeplinkApiRequests;

public class UpdateDeeplinkApiRequest
{
    [JsonPropertyName("third_party_id")]
    public int Id { get; set; }
    public UpdateDeeplinkApiRequest(DeepLink? deepLink)
    {
        if (deepLink != null) Id = deepLink.Id;
    }
}