using Livefront.BusinessLogic.Exceptions;
using Livefront.BusinessLogic.Services;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using NSubstitute;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Services.ReferralServiceTests;

[TestFixture]
public class WhenCreatingReferral : BaseReferralServiceTestFixture
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
        var refereeId = Guid.NewGuid();
        var referralCode = "THISISMYCODE";
        
        var referrerUser = new User
        {
            Id = referrerId,
            FirstName = "John",
            LastName = "Doe",
            Email ="john@email.com",
            ReferralCode = referralCode,
        };
        
        var refereeUser = new User
        {
            Id = refereeId,
            FirstName = "Jane",
            LastName = "Doe",
            Email ="jane@email.com",
            ReferralCode = referralCode,
        };
        
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = referrerId,
            RefereeId = refereeId,
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        
        GivenUserRepositoryGetByReferralCodeReturnsUser(referrerUser);
        GivenUserRepositoryGetByUserIdReturnsUser(refereeUser);
        GivenReferralRepositoryCreateReturnsReferral(referral);
        
        // Act
        var result = await referralService.CreateReferral(refereeId, referralCode, cancellationToken);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.That(refereeUser.FirstName,Is.EqualTo( result!.FirstName));
        Assert.That(refereeUser.LastName,Is.EqualTo( result.LastName));
        Assert.That(result.Status,Is.EqualTo( ReferralStatus.Complete));
        
        await ThenUserRespositoryGetByReferralCodeShouldBeCalled(referralCode, 1);
        await ThenUserRepositoryGetByIdShouldBeCalled(refereeId, 1);
        await ThenReferralRepositoryCreateShouldBeCalled(referral, referrerId, refereeId, 1);
    }
    

    [Test]
    public async Task GivenReferrerDoesNotExist_ThenThrowUserNotFoundException()
    {
        //Arrange
        var referee = Guid.NewGuid();
        var referralCode = "THISISMYCODE2";
        GivenUserRepositoryGetByIdReturnsNull();
        //Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>  await referralService.CreateReferral(referee, referralCode, cancellationToken));
        await ThenUserRepositoryGetByIdShouldBeCalled(referee, 1);
    }
    
    [Test]
    public async Task GivenRefereeDoesNotExist_ThenThrowUserNotFoundException()
    {
        //Arrange
        var referrerId = Guid.NewGuid();
        var refereeId = Guid.NewGuid();
        var referralCode = "THISISMYCODE2";
        
        var referrerUser = new User
        {
            Id = referrerId,
            FirstName = "John",
            LastName = "Doe",
            Email ="john@email.com",
            ReferralCode = referralCode,
        };
        
        GivenUserRepositoryGetByReferralCodeReturnsUser(referrerUser);
        GivenUserRepositoryGetByIdReturnsNull();
        
        //Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>  await referralService.CreateReferral(refereeId, referralCode, cancellationToken));
        await ThenUserRespositoryGetByReferralCodeShouldBeCalled(referralCode, 1);
        await ThenUserRepositoryGetByIdShouldBeCalled(refereeId, 1);
    }
    
    [Test]
    public void GivenRefereeIdIsEmpty_ThenThrowArgumentException()
    {
        //Arrange
        var refereeId = Guid.Empty;
        var referralCode = "THISISMYCODE2";
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>  await referralService.CreateReferral(refereeId, referralCode, cancellationToken));
        Assert.That(exception!.ParamName, Is.EqualTo("refereeUserId"));
    }
    
    [Test]
    public void GivenReferralCodeIsInvalid_ThenThrowArgumentException()
    {
        //Arrange
        var refereeId = Guid.NewGuid();
        var referralCode = "";
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>  await referralService.CreateReferral(refereeId, referralCode, cancellationToken));
        Assert.That(exception!.ParamName, Is.EqualTo("referralCode"));
    }

    [Test]
    public async Task GivenReferralDoesNotExists_AndReferralRepositoryCreateCallReturnsNull_ThenThrowDataPersistenceError()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        var refereeId = Guid.NewGuid();
        var referralCode = "THISISMYCODE";
        
        var referrerUser = new User
        {
            Id = referrerId,
            FirstName = "John",
            LastName = "Doe",
            Email ="john@email.com",
            ReferralCode = referralCode,
        };
        
        var refereeUser = new User
        {
            Id = refereeId,
            FirstName = "Jane",
            LastName = "Doe",
            Email ="jane@email.com",
            ReferralCode = referralCode,
        };
        
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = referrerId,
            RefereeId = refereeId,
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        
        GivenUserRepositoryGetByReferralCodeReturnsUser(referrerUser);
        GivenUserRepositoryGetByUserIdReturnsUser(refereeUser);
        GivenReferralRepositoryCreateReturnsNull();
        
        //Act/Assert
        Assert.ThrowsAsync<DataPersistenceException>(async () =>  await referralService.CreateReferral(refereeId, referralCode, cancellationToken));
        
        await ThenUserRespositoryGetByReferralCodeShouldBeCalled(referralCode, 1);
        await ThenUserRepositoryGetByIdShouldBeCalled(refereeId, 1);
        await ThenReferralRepositoryCreateShouldBeCalled(referral, referrerId, refereeId, 1);

    }
    
    [TearDown]
    public void TearDown()
    {
        mockedReferralRepository.ClearReceivedCalls();
        mockedUserRepository.ClearReceivedCalls();
        mockedLogger.ClearReceivedCalls();
    }
    
}