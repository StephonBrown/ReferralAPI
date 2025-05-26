using System.Net;
using System.Net.Http.Json;
using Livefront.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.FunctionalTests.API.ReferralLinkControllerTests;

public class WhenCreatingReferralLink : BaseControllerTestFixture
{
    private HttpClient client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        WebApplicationFactorySetup();
    }


    [SetUp]
    public void SetUp()
    {
        client = webApplicationFactory!.CreateClient();
    }

    [Test]
    public async Task GoldenPath()
    {
        // Arrange
        await CreateAndSetAuthToken(client);

        // Act
        var response = await client
            .PostAsync($"/api/referrallinks", null);
        var referralLink = await response.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(referralLink!.ReferralLink, Is.Not.Null.And.Not.Empty);
        Assert.That(referralLink.ExpirationDate,
            Is.Not.EqualTo(default)
                .And.GreaterThan(DateTime.UtcNow));
    }
    
    [Test]
    public async Task GivenReferralLinkExistsForUser_ThenReturnTheExistingLink()
    {
        // Arrange
        await CreateAndSetAuthToken(client);
        var response = await client
            .PostAsync($"/api/referrallinks", null);
        var referralLink = await response.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Act
        var response2 = await client
            .PostAsync($"/api/referrallinks", null);
        var referralLink2 = await response2.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        Assert.That(referralLink!.ReferralLink, Is.EqualTo(referralLink2!.ReferralLink));
        Assert.That(referralLink.ExpirationDate, Is.EqualTo(referralLink2.ExpirationDate));
    }
    
    [Test]
    public async Task GivenUserIdIsEmpty_ShouldReturnUnauthorized()
    {
        // Arrange
        await CreateAndSetAuthToken(client, isEmptyUserId: true);

        // Act
        var response = await client
            .PostAsync($"/api/referrallinks", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Title, Is.EqualTo("Unauthorized"));
        Assert.That(errorResponse.Detail, Contains.Substring("Not authorized "));
    }


    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Cleanup();
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
    }
}