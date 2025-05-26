using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.ReferralLinkRepositoryTests;

[TestFixture]
public class WhenCreatingReferralLink : BaseRepositoryTestFixture
{
    private IReferralLinkRepository referralLinkRepository;
    private ILogger<IReferralLinkRepository> logger = Substitute.For<ILogger<IReferralLinkRepository>>();

    
    [SetUp]
    public async Task SetUp()
    {
        await ConfigureDbContextAsync();
        referralLinkRepository = new ReferralLinkRepository(dbContext, logger);
    }

    [Test]
    public async Task GoldenPath()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.NewGuid(),
            BaseDeepLink = "https://generated-link.com/GGGG",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        
        //Act
        var createdLink = await referralLinkRepository.Create(referralLink, cancellationToken);
        
        //Assert
        Assert.That(createdLink.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(createdLink.BaseDeepLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(createdLink.DateCreated,Is.EqualTo(referralLink.DateCreated));
        Assert.That(createdLink.UserId, Is.EqualTo(referralLink.UserId));
        Assert.That(createdLink.ThirdPartyId, Is.EqualTo(referralLink.ThirdPartyId));
    }

    [Test]
    public void GivenUserGuidIsEmpty_ThenThrowsArgumentException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.Empty,
            BaseDeepLink = "https://generated-link.com",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        
        //Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.Create(referralLink, cancellationToken));
    }
    
    [Test]
    public void GivenReferralLinkNull_ThenThrowsArgumentNullException()
    {
        //Act/Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await referralLinkRepository.Create(null!, cancellationToken));
    }
    
    [Test]
    public void GivenLinkIsEmpty_ThenThrowsArgumentException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.NewGuid(),
            BaseDeepLink = String.Empty,
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        
        //Act/Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await referralLinkRepository.Create(referralLink, cancellationToken));
    }
    
    [Test]
    public void GivenThirdPartyIdIsInvalid_ThenThrowsArgumentException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.Empty,
            BaseDeepLink = "https://generated-link.com",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = -2
        };
        
        //Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.Create(referralLink, cancellationToken));
    }
    
    [Test]
    public void GivenDateCreatedIsInvalid_ThenThrowsArgumentException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.Empty,
            BaseDeepLink = "https://generated-link.com",
            DateCreated = default,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        
        //Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.Create(referralLink, cancellationToken));
    }
    
    [Test]
    public void GivenExpirationDateIsInvalid_ThenThrowsArgumentException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.Empty,
            BaseDeepLink = "https://generated-link.com",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = default,
            ThirdPartyId = 50
        };
        
        //Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.Create(referralLink, cancellationToken));
    }
    
    [Test]
    public void GivenExpirationDateIsBeforeDateCreated_ThenThrowsArgumentException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.Empty,
            BaseDeepLink = "https://generated-link.com",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(-30),
            ThirdPartyId = 50
        };
        
        //Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.Create(referralLink, cancellationToken));
    }
    
    [Test]
    public async Task GivenReferralLinkExists_And_WeAddAReferralLinkWithTheSameUserId_ThenThrowsReferralLinkAlreadyExistsException()
    {
        //Arrange
        var referralLink = new ReferralLink
        {
            UserId = Guid.NewGuid(),
            BaseDeepLink = "https://generated-link.com/AAAA",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        
        var referralLink2 = new ReferralLink
        {
            UserId = referralLink.UserId,
            BaseDeepLink = "https://generated-link.com/BBBB",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 43
        };
        
        //Act
        await referralLinkRepository.Create(referralLink, cancellationToken);
        
        //Assert
        var exception = Assert.ThrowsAsync<ReferralLinkAlreadyExistsException>(async () => await referralLinkRepository.Create(referralLink2, cancellationToken));
        Assert.That(exception.InnerException, Is.TypeOf<DbUpdateException>());
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }


}