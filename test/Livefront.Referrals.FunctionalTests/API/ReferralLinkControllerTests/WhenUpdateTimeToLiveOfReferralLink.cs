using System.Net;
using System.Net.Http.Json;
using Livefront.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.FunctionalTests.API.ReferralLinkControllerTests;

public class WhenUpdateTimeToLiveOfReferralLink : BaseControllerTestFixture
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
        var createLinkResponse = await client
            .PostAsync($"/api/referrallinks", null);
        var createdLink = await createLinkResponse.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Act
        var response = await client.PutAsync("/api/referrallinks", null);
        var updatedReferralLink = await response.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updatedReferralLink!.ReferralLink, Is.EqualTo(createdLink!.ReferralLink));
        Assert.That(updatedReferralLink.ExpirationDate,
            Is.GreaterThan(createdLink.ExpirationDate));
    }
    
    [Test]
    public async Task GivenUserIdIsEmpty_ShouldReturnUnauthorized()
    {
        // Arrange
        await CreateAndSetAuthToken(client, isEmptyUserId: true);

        // Act
        var response = await client
            .GetAsync($"/api/referrallinks");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Title, Is.EqualTo("Unauthorized"));
        Assert.That(errorResponse.Detail, Contains.Substring("Not authorized"));
    }   
    
    [Test]
    public async Task GivenReferralLinkDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await CreateAndSetAuthToken(client);
        
        // Act
        var response = await client.PutAsync("/api/referrallinks", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Title, Is.EqualTo("Not Found"));
        Assert.That(errorResponse.Detail, Contains.Substring("not be found"));
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