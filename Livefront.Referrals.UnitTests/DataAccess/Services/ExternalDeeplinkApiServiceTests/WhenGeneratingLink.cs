using System.Net;
using System.Text.Json;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Services;
using Livefront.Referrals.DataAccess.Services.Configurations;
using RichardSzalay.MockHttp;

namespace Livefront.Referrals.UnitTests.DataAccess.Services.ExternalDeeplinkApiServiceTests;

[TestFixture]
public class When_GenerateLink
{
    private IExternalDeeplinkApiService externalDeeplinkApiService;
    private MockHttpMessageHandler mockHttpMessageHandler;
    private CancellationToken cancellationToken = CancellationToken.None;
    private string referralCode;
    private string linkChannel;

    [SetUp]
    public void SetUp()
    {
        mockHttpMessageHandler = new MockHttpMessageHandler();
        var mockHttpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri("https://deeplink-api.com/")
        };
        externalDeeplinkApiService = new ExternalDeeplinkApiService(mockHttpClient);
    }

    [Test]
    public async Task GoldenPath()
    {
        //Arrange
        referralCode = "IREFERREDYOU";
        linkChannel = "SMS";

        var referralLink = new ReferralLink(1, DateTime.Now, new Uri("https://generated-link.com?channel=default"));
        
        var request = new ExternalDeeplinkApiRequest(referralCode, linkChannel);
        var responseStringContent = CreateApiResponse(referralLink);
        var mockedRequest = CreateMockedRequest(request, HttpStatusCode.OK, responseStringContent);
        
        //Act
        var generatedLink =
            await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken);

        //Assert
        Assert.That(generatedLink?.Id, Is.EqualTo(referralLink.Id));
        Assert.That(generatedLink?.DateCreated, Is.EqualTo(referralLink.DateCreated));
        Assert.That(generatedLink?.Link, Is.EqualTo(referralLink.Link));
        Assert.That(mockHttpMessageHandler.GetMatchCount(mockedRequest), Is.EqualTo(1));
    }

    [Test]
    public async Task GivenInvalidReferralCode_ThenThrowArgumentException()
    {
        //Arrange
        referralCode = "";
        linkChannel = "SMS";

        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referralCode"));
    }
    
    [Test]
    public async Task GivenInvalidChannel_ThenThrowArgumentException()
    {
        //Arrange
        referralCode = "IREFERREDYOU";
        linkChannel = "";

        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await externalDeeplinkApiService.GenerateLink(referralCode, linkChannel, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referralCode"));
    }

    private MockedRequest CreateMockedRequest(ExternalDeeplinkApiRequest request, HttpStatusCode responseCode,
        StringContent responseContent)
    {
        var mockedRequest = mockHttpMessageHandler
            .When($"https://deeplink-api.com/{DeeplinkApiConstants.DeeplinkGenerationUri}")
            .WithJsonContent(JsonSerializer.Serialize(request))
            .Respond(responseCode, responseContent);
        return mockedRequest;
    }

    private StringContent CreateApiResponse(ReferralLink referralLink)
    {
        return new StringContent(JsonSerializer.Serialize(referralLink));
    }

    [TearDown]
    public void TearDown()
    {
        mockHttpMessageHandler.Dispose();
        referralCode = string.Empty;
        linkChannel = string.Empty;
    }
}