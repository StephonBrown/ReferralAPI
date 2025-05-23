using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Models;
using NSubstitute;

namespace Livefront.Referrals.UnitTests.API.Services.ReferralLinkServiceTests;

[TestFixture]
public class WhenGettingReferralLink : BaseReferralLinkServiceTestFixture
{
    [SetUp]
    public void Setup()
    {
        referralLinkService = new ReferralLinkService(mockedReferralLinkRepository, 
            mockedUserRepository,
            mockedExternalDeeplinkApiService, 
            mockedLogger);
    }
    
    [Test]
    public async Task GoldenPath()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var referralLink = new ReferralLink
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BaseDeepLink = "https://example.com",
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            DateCreated = DateTime.UtcNow,
            ThirdPartyId = 43
        };
        
        var user = new User()
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            ReferralCode = "THISISMYCODE"
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsReferralLink(referralLink);
        // Act
        var result = await referralLinkService.GetReferralLink(userId, cancellationToken);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(referralLink.BaseDeepLink, Is.EqualTo(result!.ReferralLink));
        Assert.That(referralLink.ExpirationDate, Is.EqualTo(result.ExpirationDate));

        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
    }
    
    [Test]
    public async Task GivenReferralLinkDoesNotExist_ThenReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User()
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            ReferralCode = "THISISMYCODE"
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsNull();
        
        // Act
        var result = await referralLinkService.GetReferralLink(userId, cancellationToken);

        // Assert
        Assert.IsNull(result);
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
    }
    
    [Test]
    public void GivenUserIdIsEmpty_ThenThrowArgumentException()
    {
        // Arrange
        var userId = Guid.Empty;
        
        // Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>  await referralLinkService.GetReferralLink(userId, cancellationToken));
        Assert.That(exception!.ParamName, Is.EqualTo("userId"));
    }
    
    [Test]
    public async Task GivenUserDoesNotExist_ThenThrowUserNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        GivenUserRepositoryGetByIdReturnsNull();
        
        // Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>  await referralLinkService.GetReferralLink(userId, cancellationToken));
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
    }
    
    [TearDown]
    public void TearDown()
    {
        mockedReferralLinkRepository.ClearReceivedCalls();
        mockedUserRepository.ClearReceivedCalls();
        mockedExternalDeeplinkApiService.ClearReceivedCalls();
        mockedLogger.ClearReceivedCalls();
    }
}