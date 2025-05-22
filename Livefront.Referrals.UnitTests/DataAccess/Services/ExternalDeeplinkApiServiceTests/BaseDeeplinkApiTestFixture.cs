using System.Net;
using System.Text.Json;
using Livefront.Referrals.DataAccess.Services;
using RichardSzalay.MockHttp;

namespace Livefront.Referrals.UnitTests.DataAccess.Services.ExternalDeeplinkApiServiceTests;

public class BaseDeeplinkApiTests
{
    private IExternalDeeplinkApiService externalDeeplinkApiService;
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private MockHttpMessageHandler mockHttpHandler;
    private HttpClient mockHttpClient;
    private static readonly Uri LinkApiBaseAddress = new("https://deeplink-api.com");
    
    
    private MockedRequest SetExceptionThrowingRequestHandler(HttpMethod httpMethod, Uri endpoint, Exception exception)
    {
        return mockHttpHandler
            .Expect(httpMethod, endpoint.AbsoluteUri)
            .Throw(exception);
    }
    private MockedRequest SetRequestHandler<TResponse, TRequest>(TResponse response, TRequest request, HttpStatusCode responseCode, HttpMethod httpMethod, Uri endpoint)
    {
        var responseJson = JsonSerializer.Serialize(response);
        return mockHttpHandler
            .Expect(httpMethod, endpoint.AbsoluteUri)
            .WithJsonContent(request)
            .Respond(responseCode,"application/json",responseJson);
    }
}