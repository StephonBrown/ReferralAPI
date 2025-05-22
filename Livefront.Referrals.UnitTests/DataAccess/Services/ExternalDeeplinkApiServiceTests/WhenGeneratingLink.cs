using System.Net;

using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi;
using Livefront.Referrals.DataAccess.Services;

namespace Livefront.Referrals.UnitTests.DataAccess.Services.ExternalDeeplinkApiServiceTests;

[TestFixture]
public class WhenGeneratingLink : BaseDeeplinkApiTestFixture
{
    private static readonly Uri LinkGenerationUri = new(LinkApiBaseAddress, DeeplinkApiConstants.LinkGenerationEndpoint);
    private string referralCode;
    private string linkChannel;

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
        referralCode = "IREFERREDYOU";
        linkChannel = "SMS";
        var createLinkRequest = new CreateDeeplinkApiRequest(referralCode, linkChannel);
        var deepLink = new DeepLink( 
            1,
            DateTime.UtcNow, 
            DateTime.UtcNow.AddDays(1), 
            "https://generated-link.com?channel=default");

        var mockedRequestEndpoint =
            SetRequestHandler(deepLink,
                createLinkRequest,
                HttpStatusCode.OK,
                HttpMethod.Post,
                LinkGenerationUri);
        
        //Act
        var generatedLink = await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken);

        //Assert
        Assert.That(generatedLink?.Id, Is.EqualTo(deepLink.Id));
        Assert.That(generatedLink?.DateCreated, Is.EqualTo(deepLink.DateCreated));
        Assert.That(generatedLink?.Id, Is.EqualTo(deepLink.Id));
        Assert.That(generatedLink?.ExpirationDate, Is.EqualTo(deepLink.ExpirationDate));
        Assert.That(generatedLink?.Link, Is.EqualTo(deepLink.Link));
        Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(1));
        mockHttpHandler.VerifyNoOutstandingExpectation();
    }

    [Test]
    public void GivenInvalidReferralCode_ThenThrowArgumentException()
    {
        //Arrange
        referralCode = "";
        linkChannel = "SMS";

        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referralCode"));
    }
    
    [Test]
    public void GivenInvalidChannel_ThenThrowArgumentException()
    {
        //Arrange
        referralCode = "IREFERREDYOU";
        linkChannel = "";
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referralCode"));
    }
    
    [Test]
    public void GivenFailedGenerationOfCode_ThenThrowAExternalApiServiceException()
    {
        //Arrange
        referralCode = "IREFERREDYOU";
        linkChannel = "SMS";
        
        var mockedRequestEndpoint =
            SetExceptionThrowingRequestHandler(
                HttpMethod.Post,
                LinkGenerationUri,
        new HttpRequestException());
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ExternalApiServiceException>(async () => await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken));
        Assert.That(exception, Is.TypeOf<ExternalApiServiceException>());
        Assert.That(exception.InnerException, Is.TypeOf<HttpRequestException>());
        Assert.That(mockHttpHandler.GetMatchCount(mockedRequestEndpoint), Is.EqualTo(1));
    }
    


    [TearDown]
    public void TearDown()
    {
        mockHttpClient.Dispose();
        mockHttpHandler.Dispose();
        referralCode = string.Empty;
        linkChannel = string.Empty;
    }
}