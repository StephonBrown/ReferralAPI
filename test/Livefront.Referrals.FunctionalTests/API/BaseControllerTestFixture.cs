using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Livefront.Referrals.API.Models;

namespace Livefront.Referrals.FunctionalTests.API;

public class BaseControllerTestFixture
{
    protected TestWebApplicationFactory? webApplicationFactory;
    
    protected async Task CreateAndSetAuthToken(HttpClient client)
    {
        var secret = new RequestSecret("TEST");
        var authToken = await client.PostAsync("/api/authtest/get-bearer-token",
            new StringContent(JsonSerializer.Serialize(secret),
                Encoding.UTF8,
                "application/json"));
        var accessToken = await authToken.Content.ReadFromJsonAsync<AccessToken>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken!.Token);
    }
}