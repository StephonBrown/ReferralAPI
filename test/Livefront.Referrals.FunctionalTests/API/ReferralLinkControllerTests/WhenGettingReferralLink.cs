using System.Net;
using System.Net.Http.Json;
using Livefront.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.FunctionalTests.API.ReferralLinkControllerTests;

public class WhenGettingReferralLink : BaseControllerTestFixture
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
        var response = await client
            .GetAsync($"/api/referrallinks");
        var referralLink = await response.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(referralLink!.ReferralLink, Is.EqualTo(createdLink!.ReferralLink));
        Assert.That(referralLink.ExpirationDate,
            Is.EqualTo(createdLink.ExpirationDate));
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
    public async Task GivenNoReferralLinkExistsForUser_ThenReturnNotFound()
    {
        // Arrange
        await CreateAndSetAuthToken(client);
        
        // Act
        var response = await client
            .GetAsync($"/api/referrallinks");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Title, Is.EqualTo("Not Found"));
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