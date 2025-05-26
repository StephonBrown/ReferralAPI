using System.Net;
using System.Net.Http.Json;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Models;
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
        var testUserResponse = await client.GetAsync("/api/usertest/get-test-user");
        var testUser = await testUserResponse.Content.ReadFromJsonAsync<UserDTO>();
        
        var createLinkResponse = await client
            .PostAsync($"/api/referrallinks", null);
        var createdLink = await createLinkResponse.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Act
        var response = await client.PutAsync($"/api/referrallinks/{testUser!.Id}", null);
        var updatedReferralLink = await response.Content.ReadFromJsonAsync<ReferralLinkDTO>();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updatedReferralLink!.ReferralLink, Is.EqualTo(createdLink!.ReferralLink));
        Assert.That(updatedReferralLink.ExpirationDate,
            Is.GreaterThan(createdLink.ExpirationDate));
    }
    
    [Test]
    public async Task GivenUserIdIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        await CreateAndSetAuthToken(client);
        // Act
        var response = await client.PutAsync($"/api/referrallinks/{Guid.Empty}", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Title, Is.EqualTo("Bad Request"));
        Assert.That(errorResponse.Detail, Contains.Substring("User ID must not be empty"));
    }   
    
    [Test]
    public async Task GivenReferralLinkDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await CreateAndSetAuthToken(client);
        var testUserResponse = await client.GetAsync("/api/usertest/get-test-user");
        var testUser = await testUserResponse.Content.ReadFromJsonAsync<UserDTO>();

        // Act
        var response = await client.PutAsync($"/api/referrallinks/{testUser!.Id}", null);

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