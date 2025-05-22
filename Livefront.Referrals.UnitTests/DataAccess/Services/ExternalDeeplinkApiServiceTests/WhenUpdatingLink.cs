using System.Net;
using System.Text.Json;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi;
using Livefront.Referrals.DataAccess.Services;

namespace Livefront.Referrals.UnitTests.DataAccess.Services.ExternalDeeplinkApiServiceTests;

public class WhenUpdatingLink : BaseDeeplinkApiTestFixture
{
    private static readonly Uri LinkUpdateTtlEndpoint = new(LinkApiBaseAddress, DeeplinkApiConstants.UpdateTimeToLiveEndpoint);
    
    [SetUp]
    public void Setup()
    {
        CreateMockHttpHandlerAndHttpClient();
        externalDeeplinkApiService = new ExternalDeeplinkApiService(mockHttpClient, logger);
    }
    
    [Test]
    public async Task GoldenPath()
    {
        //Arrange
        var deepLink = new DeepLink(
            1,
            DateTime.UtcNow, 
            DateTime.UtcNow.AddDays(1), 
            "https://generated-link.com?channel=default");
        
        var deepLinkResponse = CreateDeepLinkWithUpdatedTTLResponse(deepLink);
        var updateLinkRequest = new UpdateDeeplinkApiRequest(deepLink);

        var mockedRequestEndpoint =
            SetRequestHandler(deepLinkResponse,
                updateLinkRequest,
                HttpStatusCode.OK,
                HttpMethod.Put,
                LinkUpdateTtlEndpoint);
        
        //Act
        var generatedLink = await externalDeeplinkApiService.UpdateLinkTimeToLive(deepLink, cancellationToken);

        //Assert
        Assert.That(generatedLink?.Id, Is.EqualTo(deepLinkResponse.Id));
        Assert.That(generatedLink?.DateCreated, Is.EqualTo(deepLinkResponse.DateCreated));
        Assert.That(generatedLink?.Id, Is.EqualTo(deepLinkResponse.Id));
        Assert.That(generatedLink?.ExpirationDate, Is.EqualTo(deepLinkResponse.ExpirationDate));
        Assert.That(generatedLink?.Link, Is.EqualTo(deepLinkResponse.Link));
        Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(1));
        mockHttpHandler.VerifyNoOutstandingExpectation();
    }

    [Test]
    public void GivenNullDeepLink_ThenThrowNullReferenceException()
    {
        //Arrange
        var mockedRequestEndpoint =
            SetRequestHandler<UpdateDeeplinkApiRequest, DeepLink>(
                null,
                null,
                HttpStatusCode.OK,
                HttpMethod.Put,
                LinkUpdateTtlEndpoint);
        //Act/Assert
        //Adding null to the update call in the place of the actual request object
        var exception = Assert.ThrowsAsync<NullReferenceException>(async () => await externalDeeplinkApiService.UpdateLinkTimeToLive(null, cancellationToken));
        Assert.That(exception.Message, Contains.Substring("deepLink"));
        Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(0));
    }
    
    [Test]
    public void GivenDeepLinkThirdPartyIdIsInvalid_ThenThrowArgumentException()
    {
        //Arrange
        var deepLink = new DeepLink(
            -3,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            "https://generated-link.com?channel=default"); 
        
        var deepLinkResponse = CreateDeepLinkWithUpdatedTTLResponse(deepLink);
        var updateLinkRequest = new UpdateDeeplinkApiRequest(deepLink);
        
        var mockedRequestEndpoint =
            SetRequestHandler(deepLinkResponse,
                updateLinkRequest,
                HttpStatusCode.OK,
                HttpMethod.Put,
                LinkUpdateTtlEndpoint);
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await externalDeeplinkApiService.UpdateLinkTimeToLive(deepLink, cancellationToken));
        Assert.That(exception.Message, Contains.Substring("Id"));
        Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(0));
    }
    
    [Test]
    public void GivenFailedGenerationOfCode_ThenThrowAExternalApiServiceException()
    {
        //Arrange
        var deepLink = new DeepLink(
            1,
            DateTime.UtcNow, 
            DateTime.UtcNow.AddDays(1), 
            "https://generated-link.com?channel=default");
        
        var mockedRequestEndpoint =
            SetExceptionThrowingRequestHandler(
                HttpMethod.Put,
                LinkUpdateTtlEndpoint,
                new HttpRequestException());
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ExternalApiServiceException>(async () => await externalDeeplinkApiService.UpdateLinkTimeToLive(deepLink, cancellationToken));
        Assert.That(exception, Is.TypeOf<ExternalApiServiceException>());
        Assert.That(exception.InnerException, Is.TypeOf<HttpRequestException>());
        Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(1));
    }
    
    private DeepLink CreateDeepLinkWithUpdatedTTLResponse(DeepLink deepLink)
    {
        var deepLinkResponse = new DeepLink(
            deepLink.Id,
            deepLink.DateCreated, 
            deepLink.DateCreated.AddDays(10), 
            deepLink.Link);
        return deepLinkResponse;
    }
    
    [TearDown]
    public void TearDown()
    {
        mockHttpClient.Dispose();
        mockHttpHandler.Dispose();
    }
}