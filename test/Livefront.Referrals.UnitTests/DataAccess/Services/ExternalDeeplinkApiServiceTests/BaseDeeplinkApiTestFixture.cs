using System.Net;
using System.Text.Json;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Livefront.Referrals.UnitTests.DataAccess.Services.ExternalDeeplinkApiServiceTests;

public class BaseDeeplinkApiTestFixture
{
    protected IExternalDeeplinkApiService externalDeeplinkApiService;
    protected readonly CancellationToken cancellationToken = CancellationToken.None;
    protected MockHttpMessageHandler mockHttpHandler;
    protected HttpClient mockHttpClient;
    protected static readonly Uri LinkApiBaseAddress = new("https://deeplink-api.com");
    protected ILogger<ExternalDeeplinkApiService> logger = Substitute.For<ILogger<ExternalDeeplinkApiService> >();
    
    
    protected MockedRequest SetExceptionThrowingRequestHandler(HttpMethod httpMethod, Uri endpoint, Exception exception)
    {
        return mockHttpHandler
            .Expect(httpMethod, endpoint.AbsoluteUri)
            .Throw(exception);
    }
    
    protected MockedRequest SetRequestHandler<TResponse, TRequest>(TResponse response, TRequest? request, HttpStatusCode responseCode, HttpMethod httpMethod, Uri endpoint)
    {
        var responseJson = JsonSerializer.Serialize(response);

        if (request == null)
        { 
            return mockHttpHandler
            .Expect(httpMethod, endpoint.AbsoluteUri)
            .Respond(responseCode,"application/json",responseJson);
        }
        return mockHttpHandler
            .Expect(httpMethod, endpoint.AbsoluteUri)
            .WithJsonContent(request)
            .Respond(responseCode,"application/json",responseJson);
    }
    
    protected void CreateMockHttpHandlerAndHttpClient()
    {
        mockHttpHandler  = new MockHttpMessageHandler();
        mockHttpClient = new HttpClient(mockHttpHandler)
        {
            BaseAddress = LinkApiBaseAddress
        };
    }
}