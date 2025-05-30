using System.Text.Json.Serialization;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

namespace Livefront.Referrals.DataAccess.Models.DeeplinkApi;

public class DeleteDeeplinkApiRequest
{
    [JsonPropertyName("third_party_id")]
    public int Id { get; set; }
    public DeleteDeeplinkApiRequest(DeepLink? deepLink)
    {
        if (deepLink != null) Id = deepLink.Id;
    }
}