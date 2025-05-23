using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;
using NSubstitute;

namespace Livefront.Referrals.UnitTests.API.Services.ReferralLinkServiceTests;

public class WhenExtendingReferralLinkTimeToLive : BaseReferralLinkServiceTestFixture
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
        //Arrange
        var userId = Guid.NewGuid();
        var thirdPartyId = 1003;
        var link = "https://my-deeplink.com/";
        var dateCreated = DateTime.UtcNow.AddDays(30);
        var dateExpires = dateCreated.AddDays(1);

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
            DateCreated = dateCreated,
            ExpirationDate = dateExpires,
            ThirdPartyId = thirdPartyId
        };

        var updatedDeepLink = new DeepLink(
            thirdPartyId,
            DateTime.UtcNow,
            dateExpires.AddDays(31),
            link);

        var updatedReferralLink = new ReferralLink()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BaseDeepLink = link,
            DateCreated = dateCreated,
            ExpirationDate = updatedDeepLink.ExpirationDate,
            ThirdPartyId = thirdPartyId
        };

        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsReferralLink(referralLink);
        GivenExternalDeeplinkApiServiceUpdateLinkTimeToLiveReturnsDeeplink(updatedDeepLink);
        GivenReferralLinkRepositoryUpdateExpirationDateReturnsReferralLink(updatedReferralLink);

        //Act
        var returnedReferralLinkDto = await referralLinkService.ExtendReferralLinkTimeToLive(userId, cancellationToken);

        //Assert
        Assert.That(returnedReferralLinkDto, Is.Not.Null);
        Assert.That(returnedReferralLinkDto.ReferralLink, Is.EqualTo(updatedReferralLink.BaseDeepLink));
        Assert.That(returnedReferralLinkDto.ExpirationDate, Is.EqualTo(updatedReferralLink.ExpirationDate));

        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
        await ThenExternalDeeplinkApiServiceUpdateLinkTimeToLiveShouldBeCalled(thirdPartyId, dateCreated, dateExpires,
            link, 1);
        await ThenReferralLinkRepositoryUpdateExpirationDateShouldBeCalled(user, updatedDeepLink, 1);
    }

    private async Task ThenReferralLinkRepositoryUpdateExpirationDateShouldBeCalled(User user, DeepLink updatedDeepLink,
        int numberOfCalls)
    {
        await mockedReferralLinkRepository
            .Received(numberOfCalls)
            .UpdateExpirationDate(
                Arg.Is<Guid>(id => id == user.Id),
                Arg.Is<DateTime>(date => date == updatedDeepLink.ExpirationDate),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
    }


    [Test]
    public async Task GivenUserDoesNotExist_ThenThrowUserNotFoundException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        GivenUserRepositoryGetByIdReturnsNull();
        //Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>
            await referralLinkService.ExtendReferralLinkTimeToLive(userId, cancellationToken));
        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);

    }


    [Test]
    public void GivenUserIdIsEmpty_ThenThrowArgumentException()
    {
        //Arrange
        var userId = Guid.Empty;

        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await referralLinkService.ExtendReferralLinkTimeToLive(userId, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }

    [Test]
    public async Task GivenReferralLinkIsNull_ThenThrowReferralLinkNotFoundException()
    {
        //Arrange
        var userId = Guid.NewGuid();

        var user = new User()
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            ReferralCode = "THISISMYCODE"
        };

        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsNull();

        //Act/Assert
        var exception = Assert.ThrowsAsync<ReferralLinkNotFoundException>(async () =>
            await referralLinkService.ExtendReferralLinkTimeToLive(userId, cancellationToken));
        Assert.That(exception.UserId, Is.EqualTo(userId));

        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);

    }


    [Test]
    public async Task GivenExternalDeeplinkIsNull_ThenThrowExternalApiServiceException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var thirdPartyId = 1003;
        var link = "https://my-deeplink.com/";
        var dateCreated = DateTime.UtcNow.AddDays(30);
        var dateExpires = dateCreated.AddDays(1);

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
            DateCreated = dateCreated,
            ExpirationDate = dateExpires,
            ThirdPartyId = thirdPartyId
        };

        GivenUserRepositoryGetByUserIdReturnsUser(user);
        GivenReferralLinkRepositoryGetByUserIdReturnsReferralLink(referralLink);
        GivenExternalDeeplinkApiServiceUpdateLinkTimeToLiveReturnsNull();

        //Act/Assert
        Assert.ThrowsAsync<ExternalApiServiceException>(async () =>
            await referralLinkService.ExtendReferralLinkTimeToLive(userId, cancellationToken));

        await ThenUserRepositoryGetByIdShouldBeCalled(userId, 1);
        await ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(userId, 1);
        await ThenExternalDeeplinkApiServiceShouldBeCalled(thirdPartyId, dateCreated, dateExpires, link, 1);

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