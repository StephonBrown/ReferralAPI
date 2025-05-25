using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.FunctionalTests.API.ReferralControllerTests;

public class WhenCompletingReferral : BaseControllerTestFixture
{
    private HttpClient client;
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
        var referrersReferralCode = "TESTCODE";
        var newRefereeUser = new User
        {
            FirstName = "Jimmy",
            LastName = "Doe",
            Email = "jimmy@email.com",
            ReferralCode = "REFERRALCODE",
        };
        
        var accessToken = await GetAuthToken(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken!.Token);
        
        var referee = await CreateReferee(newRefereeUser);
        var createReferralRequest = new CreateReferralRequest(referee!.Id, referrersReferralCode);
        
        // Act
        //Call the API to create/complete a referral
        var response = await client.PostAsync($"/api/referrals",             
            new StringContent(JsonSerializer
                    .Serialize(createReferralRequest), 
                Encoding.UTF8, 
                "application/json"));;
        var referralDto = await response.Content.ReadFromJsonAsync<ReferralDTO>();

        // Assert
        
        // Response assertions
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        
        // Referral object assertions
        Assert.That(referralDto!.FirstName, Is.EqualTo(referee.FirstName));
        Assert.That(referralDto.LastName, Is.EqualTo(referee.LastName));
        Assert.That(referralDto.Status, Is.EqualTo(ReferralStatus.Complete));
    }

    private async Task<User?> CreateReferee(User user)
    {
        var refereeResponse =
            await client.PostAsync($"/api/usertest",
                new StringContent(
                    JsonSerializer.Serialize(user),
                    Encoding.UTF8,
                    "application/json"));

        var referee = await refereeResponse.Content.ReadFromJsonAsync<User>();
        return referee;
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