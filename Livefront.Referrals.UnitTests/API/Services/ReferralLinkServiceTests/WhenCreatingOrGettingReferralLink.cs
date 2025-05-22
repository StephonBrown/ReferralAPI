using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Livefront.Referrals.UnitTests.API.Services.ReferralLinkServiceTests;

[TestFixture]
public class WhenCreatingOrGettingReferralLink
{
    private IExternalDeeplinkApiService mockedExternalDeeplinkApiService =
        Substitute.For<IExternalDeeplinkApiService>();
    private IReferralLinkRepository mockedReferralLinkRepository = Substitute.For<IReferralLinkRepository>();
    private IUserRepository mockedUserRepository = Substitute.For<IUserRepository>();
    private ILogger<IReferralLinkService> mockedLogger = Substitute.For<ILogger<IReferralLinkService>>();
    private CancellationToken cancellationToken = CancellationToken.None;
    private IReferralLinkService referralLinkService;

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
        
        var deepLink = new DeepLink(
            thirdPartyId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(30),
            link);
        
        var referralLink = new ReferralLink()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BaseDeepLink = deepLink.Link,
            DateCreated = deepLink.DateCreated,
            ExpirationDate = deepLink.ExpirationDate,
            ThirdPartyId = deepLink.Id
        };
        
        mockedUserRepository
            .GetById(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .Returns(user);
        mockedReferralLinkRepository
            .GetByUserId(Arg.Any<Guid>(), 
            Arg.Any<CancellationToken>())
            .ReturnsNull();
        mockedExternalDeeplinkApiService.GenerateLink(
            Arg.Any<string>(), 
            Arg.Any<CancellationToken>())
            .Returns(deepLink);
        mockedReferralLinkRepository
            .Create(Arg.Any<ReferralLink>(), 
                Arg.Any<CancellationToken>())
            .Returns(referralLink);
        
        //Act
        var newReferralLink = await referralLinkService.CreateOrGetReferralLink(userId, cancellationToken);

        //Assert
        Assert.That(newReferralLink, Is.Not.Null);
        Assert.That(newReferralLink.ReferralLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(newReferralLink.ExpirationDate, Is.EqualTo(referralLink.ExpirationDate));

        await mockedUserRepository
            .Received(1)
            .GetById(Arg.Is<Guid>(id => id == userId),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));

        await mockedReferralLinkRepository
            .Received(1)
            .GetByUserId(Arg.Is<Guid>(id => id == userId),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));

        await mockedReferralLinkRepository
            .Received(1)
            .Create(
                Arg.Is<ReferralLink>(submittedReferral =>
                    submittedReferral.BaseDeepLink == referralLink.BaseDeepLink &&
                    submittedReferral.ExpirationDate == referralLink.ExpirationDate &&
                    submittedReferral.ThirdPartyId == referralLink.ThirdPartyId &&
                    submittedReferral.DateCreated == referralLink.DateCreated &&
                    submittedReferral.Id == Guid.Empty),
                cancellationToken);

        await mockedExternalDeeplinkApiService
            .Received(1)
            .GenerateLink(
                Arg.Is<string>(referralCode => referralCode == user.ReferralCode), cancellationToken);
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
        
        mockedUserRepository
            .GetById(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .Returns(user);
        mockedReferralLinkRepository
            .GetByUserId(Arg.Any<Guid>(), 
            Arg.Any<CancellationToken>())
            .Returns(referralLink);
        
        //Act
        var newReferralLink = await referralLinkService.CreateOrGetReferralLink(userId, cancellationToken);

        //Assert
        Assert.That(newReferralLink, Is.Not.Null);
        Assert.That(newReferralLink.ReferralLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(newReferralLink.ExpirationDate, Is.EqualTo(referralLink.ExpirationDate));
        
        await mockedUserRepository
            .Received(1)
            .GetById(Arg.Is<Guid>(id => id == userId),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));

        await mockedReferralLinkRepository
            .Received(1)
            .GetByUserId(Arg.Is<Guid>(id => id == userId),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
    }
    
    [Test]
    public async Task GivenUserDoesNotExist_ThenThrowUserNotFoundException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        
        mockedUserRepository
            .GetById(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .ReturnsNull();
        
        //Act/Assert
        Assert.ThrowsAsync<UserNotFoundException>(async () =>  await referralLinkService.CreateOrGetReferralLink(userId, cancellationToken));
        await mockedUserRepository
            .Received(1)
            .GetById(Arg.Is<Guid>(id => id == userId),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
        
    }
    
    [Test]
    public void GivenUserIdIsEmpty_ThenThrowArgumentException()
    {
        //Arrange
        var userId = Guid.Empty;
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>  await referralLinkService.CreateOrGetReferralLink(userId, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }
}