using Livefront.BusinessLogic.Exceptions;
using Livefront.BusinessLogic.Services;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;
using NSubstitute;

namespace Livefront.Referrals.UnitTests.API.Services.ReferralLinkServiceTests;

[TestFixture]
public class WhenCreatingReferralLink : BaseReferralLinkServiceTestFixture
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
    public async Task GivenReferralLinkDoesNotExist_ThenCreateNewReferralLinkAndReturnTheNewReferralLink()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var thirdPartyId = 1003;
        var link = "https://my-deeplink.com/";

        var user = new User()
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            ReferralCode = "THISISMYCODE"
        };

        var deepLink = new DeepLink
        {
            DateCreated = DateTime.UtcNow,
            Id = thirdPartyId,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            Link = link
        };

        var referralLink = new ReferralLink()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BaseDeepLink = deepLink.Link,
            DateCreated = deepLink.DateCreated,
            ExpirationDate = deepLink.ExpirationDate,
            ThirdPartyId = deepLink.Id
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsNull();
        GivenExternalDeeplinkApiServiceGenerateLinkReturnsDeeplink(deepLink);
        GivenReferralLinkRepositoryCreateReturnsReferralLink(referralLink);
        
        //Act
        var newReferralLink = await referralLinkService.CreateReferralLink(userId, cancellationToken);

        //Assert
        Assert.That(newReferralLink, Is.Not.Null);
        Assert.That(newReferralLink!.ReferralLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(newReferralLink.ExpirationDate, Is.EqualTo(referralLink.ExpirationDate));
        
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryCreateShouldBeCalled(referralLink, 1);
        await ThenExternalDeeplinkApiServiceGenerateLinkShouldBeCalled(user, 1);
    }


    
    [Test]
    public async Task GivenReferralLinkDoesExist_ThenReturnReferralLink()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var thirdPartyId = 1003;
        var link = "https://my-deeplink.com/";
        
        var user = new User()
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            ReferralCode = "THISISMYCODE"
        };
        
        var referralLink = new ReferralLink()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BaseDeepLink = link,
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = thirdPartyId
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsReferralLink(referralLink);
        //Act
        var newReferralLink = await referralLinkService.CreateReferralLink(userId, cancellationToken);

        //Assert
        Assert.That(newReferralLink, Is.Not.Null);
        Assert.That(newReferralLink!.ReferralLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(newReferralLink.ExpirationDate, Is.EqualTo(referralLink.ExpirationDate));
        
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
    }
    
    [Test]
    public async Task GivenUserDoesNotExist_ThenThrowUserNotFoundException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        GivenUserRepositoryGetByIdReturnsNull();
        //Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>  await referralLinkService.CreateReferralLink(userId, cancellationToken));
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
    }
    
    [Test]
    public void GivenUserIdIsEmpty_ThenThrowArgumentException()
    {
        //Arrange
        var userId = Guid.Empty;
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>  await referralLinkService.CreateReferralLink(userId, cancellationToken));
        Assert.That(exception!.ParamName, Is.EqualTo("userId"));
    }

    [Test]
    public async Task GivenReferralLinkDoesNotExists_AndReferralRepositoryCreateCallReturnsNull_ThenThrowDataPersistenceError()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var thirdPartyId = 1003;
        var link = "https://my-deeplink.com/";

        var user = new User()
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            ReferralCode = "THISISMYCODE"
        };

        var deepLink = new DeepLink
        {
            DateCreated = DateTime.UtcNow,
            Id = thirdPartyId,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            Link = link
        };

        var referralLink = new ReferralLink()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BaseDeepLink = deepLink.Link,
            DateCreated = deepLink.DateCreated,
            ExpirationDate = deepLink.ExpirationDate,
            ThirdPartyId = deepLink.Id
        };
        
        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsNull();
        GivenExternalDeeplinkApiServiceGenerateLinkReturnsDeeplink(deepLink);
        GivenReferralLinkRepositoryCreateReturnsNull();
        
        //Act/Assert
        Assert.ThrowsAsync<DataPersistenceException>(async () =>  await referralLinkService.CreateReferralLink(userId, cancellationToken));
        
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryCreateShouldBeCalled(referralLink, 1);
        await ThenExternalDeeplinkApiServiceGenerateLinkShouldBeCalled(user, 1);
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