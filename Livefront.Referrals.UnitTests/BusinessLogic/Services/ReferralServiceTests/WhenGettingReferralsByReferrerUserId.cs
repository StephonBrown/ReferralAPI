using Livefront.BusinessLogic.Exceptions;
using Livefront.BusinessLogic.Services;
using Livefront.Referrals.DataAccess.Models;
using NSubstitute;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Services.ReferralServiceTests;

public class WhenGettingReferralsByReferrerUserId : BaseReferralServiceTestFixture
{
    
    [SetUp]
    public void SetUp()
    {
        referralService = new ReferralService(mockedReferralRepository,
            mockedUserRepository,
            mockedLogger);
    }

    [Test]
    public async Task GoldenPath()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        var referrerUser = new User
        {
            Id = referrerId,
            FirstName = "John",
            LastName = "Doe",
            Email = "email@email.com",
            ReferralCode = "THISISMYCODE3",
        };
        
        var referee1 = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "email@email.com",
            ReferralCode = "THISISMYCODE3",
        };
        
        var referee2 = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "email@email.com",
            ReferralCode = "THISISMYCODE3",
        };

        var referrals = new List<Referral>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                ReferrerId = referrerId,
                RefereeId = referee1.Id,
                DateCreated = DateTime.UtcNow,
                Status = ReferralStatus.Complete
            },
            new()
            {
                Id = Guid.NewGuid(),
                ReferrerId = referrerId,
                RefereeId = referee1.Id,
                DateCreated = DateTime.UtcNow,
                Status = ReferralStatus.Complete
            },
        };
        
        var refereeList = new [] { referee1, referee1 };
        GivenUserRepositoryGetByUserIdBySpecificUserIdReturnsUser(referrerId, referrerUser);
        GivenUserRepositoryGetByIdsReturnsUsers(refereeList);
        GivenReferralRepositoryGetReferralsByReferrerIdReturnsReferral(referrals);
        
        // Act
        var result = await referralService
            .GetReferralsByReferrerUserId(referrerId, cancellationToken);
        var resultList = result.ToList();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.That(resultList.Count, Is.EqualTo(2));
        
        // Referee 1
        Assert.That(resultList[0].FirstName, Is.EqualTo(referee1.FirstName));
        Assert.That(resultList[0].LastName, Is.EqualTo(referee1.LastName));
        Assert.That(resultList[0].Status, Is.EqualTo(ReferralStatus.Complete));
        // Referee 2
        Assert.That(resultList[1].FirstName, Is.EqualTo(referee2.FirstName));
        Assert.That(resultList[1].LastName, Is.EqualTo(referee2.LastName));
        Assert.That(resultList[1].Status, Is.EqualTo(ReferralStatus.Complete));
        
        await ThenUserRepositoryGetByIdShouldBeCalled(referrerId, 1);
        await ThenUserRepositoryGetByIdsShouldBeCalled(refereeList.Select(x => x.Id).ToArray(), 1);
        await ThenReferralRepositoryGetReferralsByReferrerIdShouldBeCalled(referrerId, 1);
    }
    
    [Test]
    public void  GivenReferrerIdIsEmpty_ThenThrowUserNotFoundException()
    {
        // Arrange
        var referrerId = Guid.Empty;
        
        // Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralService
            .GetReferralsByReferrerUserId(referrerId, cancellationToken));
    }
    
    [Test]
    public async Task GivenReferrerDoesNotExist_ThenThrowUserNotFoundException()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        GivenUserRepositoryGetByIdReturnsNull();
        
        // Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () => await referralService
            .GetReferralsByReferrerUserId(referrerId, cancellationToken));
        
        await ThenUserRepositoryGetByIdShouldBeCalled(referrerId, 1);
    }
    

    [Test]
    public async Task GivenReferrerHasNoReferrals_ThenReturnAnEmptyEnumerable()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        var referrerUser = new User
        {
            Id = referrerId,
            FirstName = "John",
            LastName = "Doe",
            Email = "email@email.com",
            ReferralCode = "THISISMYCODE3",
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(referrerUser);
        GivenReferralRepositoryGetReferralsByUserIdReturnEmptyIEnumerable();
        
        // Act
        var result = await referralService
            .GetReferralsByReferrerUserId(referrerId, cancellationToken);
        
        // Assert
        Assert.That(result, Is.Empty);
        await ThenUserRepositoryGetByIdShouldBeCalled(referrerId, 1);
        await ThenReferralRepositoryGetReferralsByReferrerIdShouldBeCalled(referrerId, 1);
    }

    [Test]
    public async Task GivenRefereesOnReferralDoNotExist_ThenReturnAnEmptyEnumerable()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        var referrerUser = new User
        {
            Id = referrerId,
            FirstName = "John",
            LastName = "Doe",
            Email = "email@email.com",
            ReferralCode = "THISISMYCODE3",
        };
        var referrals = new List<Referral>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                ReferrerId = referrerId,
                RefereeId = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Status = ReferralStatus.Complete
            },
            new()
            {
                Id = Guid.NewGuid(),
                ReferrerId = referrerId,
                RefereeId = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Status = ReferralStatus.Complete
            },
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(referrerUser);
        GivenReferralRepositoryGetReferralsByReferrerIdReturnsReferral(referrals);
        GivenUserRepositoryGetByIdsReturnsEmpty();
        
        // Act
        var result = await referralService
            .GetReferralsByReferrerUserId(referrerId, cancellationToken);
        
        // Assert
        Assert.That(result, Is.Empty);
        await ThenUserRepositoryGetByIdShouldBeCalled(referrerId, 1);
        await ThenReferralRepositoryGetReferralsByReferrerIdShouldBeCalled(referrerId, 1);
        await ThenUserRepositoryGetByIdsShouldBeCalled(referrals.Select(x => x.RefereeId).ToArray(), 1);

    }
    
    [TearDown]
    public void TearDown()
    {
        mockedReferralRepository.ClearReceivedCalls();
        mockedUserRepository.ClearReceivedCalls();
        mockedLogger.ClearReceivedCalls();
    }
}