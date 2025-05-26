using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Livefront.Referrals.API.Models;

namespace Livefront.Referrals.FunctionalTests.API;

public class BaseControllerTestFixture
{
    protected TestWebApplicationFactory? webApplicationFactory;
    protected AccessToken? accessToken;

    protected void WebApplicationFactorySetup()
    {
        webApplicationFactory = new TestWebApplicationFactory();
    }

    protected HttpClient HttpClientSetup()
    {
        return webApplicationFactory!.CreateClient();
    }
    
    protected void Cleanup()
    {
        webApplicationFactory?.Dispose();
    }
    
    protected async Task CreateAndSetAuthToken(HttpClient client, bool isEmptyUserId = false)
    {
        var secret = new RequestSecret("TEST", isEmptyUserId);
        var authToken = await client.PostAsync("/api/authtest/get-bearer-token",
            new StringContent(JsonSerializer.Serialize(secret),
                Encoding.UTF8,
                "application/json"));
        accessToken = await authToken.Content.ReadFromJsonAsync<AccessToken>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken!.Token);
    }
}