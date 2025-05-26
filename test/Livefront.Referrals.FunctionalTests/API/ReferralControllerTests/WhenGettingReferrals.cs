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
[Category("Functional Tests")]
public class WhenGettingReferrals : BaseControllerTestFixture
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
        client = HttpClientSetup();
    }
    
    [Test]
    public async Task GoldenPath()
    {
        // Arrange
        var testUserResponse = await client.GetAsync("/api/usertest/get-test-user");
        var testUser = await testUserResponse.Content.ReadFromJsonAsync<UserDTO>();
        
        // Create users for referral
        var newRefereeUser = new User
        {
            FirstName = "Jimmy",
            LastName = "Doe",
            Email = "jimmy@email.com",
            ReferralCode = "REFERRALCODE",
        };
        
        var newRefereeUser2 = new User
        {
            FirstName = "Rachel",
            LastName = "Doe",
            Email = "rach@email.com",
            ReferralCode = "REFERRALCODE2",
        };
        
        await CreateAndSetAuthToken(client);
        var createdUsers = await CreateReferees(newRefereeUser, newRefereeUser2);
        await CreateReferrals(testUser!.ReferralCode, createdUsers);
        // Act
        
        //Call the API to create/complete a referral
        var response = await client.GetAsync($"/api/referrals");
        var referralDtos= await response.Content.ReadFromJsonAsync<IEnumerable<ReferralDTO>>();
        referralDtos = referralDtos?.ToList();

        // Assert
        
        // Response assertions
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        // Referral object assertions
        Assert.That(referralDtos, Is.Not.Null);
        Assert.That(referralDtos!.Count, Is.EqualTo(2));
        
        // Assert that each referral matches the created users
        foreach (var referralDto in referralDtos)
        {
            var assertionReferee = createdUsers.FirstOrDefault(user => user.Id == referralDto.UserId);
            
            Assert.That(assertionReferee!.FirstName, Is.EqualTo(referralDto.FirstName));
            Assert.That(assertionReferee.LastName, Is.EqualTo(referralDto.LastName));
            Assert.That(referralDto.Status, Is.EqualTo(ReferralStatus.Complete));
        }
    }
    [Test]
    public async Task GivenNoReferralsExist_ReturnEmptyEnumerable()
    {
        // Arrange
        // The token is created with the test user's id
        await CreateAndSetAuthToken(client);
        
        // Act
        
        //Call the API to create/complete a referral
        var response = await client.GetAsync($"/api/referrals");
        var referralDtos= await response.Content.ReadFromJsonAsync<IEnumerable<ReferralDTO>>();
        referralDtos = referralDtos?.ToList();

        // Assert
        
        // Response assertions
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        // Referral object assertions
        Assert.That(referralDtos, Is.Empty);
    }
    
    [Test]
    public async Task GivenInvalidUserIdOnToken_ThrowUnauthorizedException()
    {
        // Arrange
        // The token is created with the test user's id
        await CreateAndSetAuthToken(client:client, isEmptyUserId:true);

        // Act
        
        //Call the API to create/complete a referral
        var response = await client.GetAsync($"/api/referrals");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(errorResponse!.Title, Is.EqualTo("Unauthorized"));
        
    }
    

    private async Task CreateReferrals(string referralCode, params UserDTO[] newRefereeUsers)
    {
        foreach (var referee in newRefereeUsers)
        {
            var createReferralRequest = new CreateReferralRequest(referee.Id, referralCode);
            await client.PostAsync($"/api/referrals",
                new StringContent(JsonSerializer
                        .Serialize(createReferralRequest),
                    Encoding.UTF8,
                    "application/json"));
            
        }
    }

    private async Task<UserDTO[]> CreateReferees(params User[] users)
    {
        var userList = new List<UserDTO>();
        foreach (var user in users)
        {
            var refereeResponse =
                await client.PostAsync("/api/usertest",
                    new StringContent(
                        JsonSerializer.Serialize(user),
                        Encoding.UTF8,
                        "application/json"));
            var createdUser = await refereeResponse.Content.ReadFromJsonAsync<UserDTO>();
            userList.Add(createdUser!);
        }

        return userList.ToArray();
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