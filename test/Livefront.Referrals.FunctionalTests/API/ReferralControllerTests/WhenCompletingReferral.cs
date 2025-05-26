using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.FunctionalTests.API.ReferralControllerTests;

[TestFixture]
[Category("Functional")]
public class WhenCompletingReferral : BaseControllerTestFixture
{
    private HttpClient client = null!;
    private string referrersReferralCode = string.Empty;
    private User? referee;
    private CreateReferralRequest? createReferralRequest;
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        webApplicationFactory = new TestWebApplicationFactory();
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
        referrersReferralCode = "TESTCODE";
        var newRefereeUser = new User
        {
            FirstName = "Jimmy",
            LastName = "Doe",
            Email = "jimmy@email.com",
            ReferralCode = "REFERRALCODE",
        };
        
        await CreateAndSetAuthToken(client);
        referee = await CreateReferee(newRefereeUser);
        createReferralRequest = new CreateReferralRequest(referee!.Id, referrersReferralCode);
        
        // Act
        
        //Call the API to create/complete a referral
        var response = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));
        var referralDto = await response.Content.ReadFromJsonAsync<ReferralDTO>();

        // Assert
        
        // Response assertions
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        // Referral object assertions
        Assert.That(referralDto!.FirstName, Is.EqualTo(referee.FirstName));
        Assert.That(referralDto.LastName, Is.EqualTo(referee.LastName));
        Assert.That(referralDto.Status, Is.EqualTo(ReferralStatus.Complete));
    }
    
    [Test]
    public async Task GivenReferralCodeIsInvalid_ThenReturnBadRequest()
    {
        // Arrange
        referrersReferralCode = "";
        var newRefereeUser = new User
        {
            FirstName = "Jimmy",
            LastName = "Doe",
            Email = "jimmy@email.com",
            ReferralCode = "REFERRALCODE",
        };
        
        await CreateAndSetAuthToken(client);
        
        referee = await CreateReferee(newRefereeUser);
        createReferralRequest = new CreateReferralRequest(referee!.Id, referrersReferralCode);

        // Act
        var response = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task GivenReferralCodeUserDoesNotExist_ThenReturnBadRequest()
    {
        
        // Arrange
        referrersReferralCode = "FAKECODE";
        var newRefereeUser = new User
        {
            FirstName = "Jimmy",
            LastName = "Doe",
            Email = "jimmy@email.com",
            ReferralCode = "REFERRALCODE",
        };
        
        await CreateAndSetAuthToken(client);
        referee = await CreateReferee(newRefereeUser);
        createReferralRequest = new CreateReferralRequest(referee!.Id, referrersReferralCode);

        // Act
        var response = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Detail, Is.EqualTo($"User with referral code {referrersReferralCode} was not found"));
    }

    [Test]
    public async Task GivenRefereeDoesNotExist_ThenReturnBadRequest()
    {
        // Arrange
        referrersReferralCode = "TESTCODE";
        var fakeRefereeID = Guid.NewGuid();
        await CreateAndSetAuthToken(client);
        createReferralRequest = new CreateReferralRequest(fakeRefereeID, referrersReferralCode);

        // Act
        var response = await client.PostAsync($"/api/referrals",
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest),
                Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Detail, Is.EqualTo($"User {fakeRefereeID} was not found"));
    }

    [Test]
    public async Task GivenRefereeIdIsInvalid_ReturnBadRequest()
    {
        // Arrange
        await CreateAndSetAuthToken(client);
        referrersReferralCode = "TESTCODE";
        createReferralRequest = new CreateReferralRequest(Guid.Empty, referrersReferralCode);
        
        // Act
        var response = await client.PostAsync("/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Detail, Contains.Substring("ID cannot be empty"));
    }

    [Test]
    public async Task GivenReferralForReferrerAndRefereeAlreadyExists_ThenReturnConflict()
    {
        // Arrange
        referrersReferralCode = "TESTCODE";
        var newRefereeUser = new User
        {
            FirstName = "Jimmy",
            LastName = "Doe",
            Email = "jimmy@email.com",
            ReferralCode = "REFERRALCODE",
        };
        
        await CreateAndSetAuthToken(client);
        referee = await CreateReferee(newRefereeUser);
        createReferralRequest = new CreateReferralRequest(referee!.Id, referrersReferralCode);
        
        // Act
        //Call the API to create/complete a referral
        var response = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));
        
        // Call the API again to create/complete a referral
        var response2 = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));

        // Assert
        
        // Response assertions
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        
        // Referral Error assertions
        var errorResponse = await response2.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Detail, Contains.Substring("already exists"));
    }

    [Test]
    public async Task GivenRefereeAndReferralCodeAreTheSameUser_ThenReturnBadRequest()
    {
        // Arrange
        referrersReferralCode = "TESTCODE";
        var testUserResponse = await client.GetAsync("/api/usertest/get-test-user");
        var testUser = await testUserResponse.Content.ReadFromJsonAsync<User>();
        
        await CreateAndSetAuthToken(client);
        createReferralRequest = new CreateReferralRequest(testUser!.Id, referrersReferralCode);
        
        // Act
        
        //Call the API to create/complete a referral
        var response = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));

        // Assert
            
        // Response assertions
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Detail, Is.EqualTo("Referrer and referee cannot be the same user"));
    }

    private async Task<User?> CreateReferee(User user)
    {
        var refereeResponse =
            await client.PostAsync("/api/usertest",
                new StringContent(
                    JsonSerializer.Serialize(user),
                    Encoding.UTF8,
                    "application/json"));

        return await refereeResponse.Content.ReadFromJsonAsync<User>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        webApplicationFactory?.Dispose();
    }
    
    [TearDown]
    public void TearDown()
    {
        client.Dispose();
    }

}