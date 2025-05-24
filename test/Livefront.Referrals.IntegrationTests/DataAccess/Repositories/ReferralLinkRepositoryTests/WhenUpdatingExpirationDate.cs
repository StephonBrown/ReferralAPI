using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.ReferralLinkRepositoryTests;

[TestFixture]
public class WhenUpdatingExpirationDate : BaseRepositoryTestFixture
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
            BaseDeepLink = "https://generated-link.com",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        
        var createdLink = await referralLinkRepository.Create(referralLink, cancellationToken);
        
        //Assert
        Assert.That(referralLink.ExpirationDate, Is.EqualTo(createdLink.ExpirationDate));
        
        //Act
        var newExpirationDate = createdLink.ExpirationDate.AddDays(30);
        var updatedLink = await referralLinkRepository.UpdateExpirationDate(createdLink.UserId, newExpirationDate, cancellationToken);
        
        //Assert
        Assert.That(updatedLink?.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(updatedLink.BaseDeepLink, Is.EqualTo(createdLink.BaseDeepLink));
        Assert.That(updatedLink.DateCreated,Is.EqualTo(createdLink.DateCreated));
        Assert.That(updatedLink.UserId, Is.EqualTo(createdLink.UserId));
        Assert.That(updatedLink.ThirdPartyId, Is.EqualTo(createdLink.ThirdPartyId));
        Assert.That(updatedLink.ExpirationDate, Is.EqualTo(newExpirationDate));
    }
    
    [Test]
    public void GivenInvalidUserId_ThenThrowsArgumentException()
    {
        //Arrange/Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.UpdateExpirationDate(Guid.Empty, DateTime.UtcNow, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }
    
    [Test]
    public void GivenInvalidExpirationDate_ThenThrowsArgumentException()
    {
        //Arrange/Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.UpdateExpirationDate(Guid.NewGuid(), default, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("newExpirationDate"));
    }
    
    [Test]
    public async Task GivenNewExpirationDateIsOlderThanOldExpirationDate_ThenThrowsArgumentException()
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
        
        var createdLink = await referralLinkRepository.Create(referralLink, cancellationToken);
        
        //Assert
        Assert.That(referralLink.ExpirationDate, Is.EqualTo(createdLink.ExpirationDate));
        
        //Act
        var olderExpirationDate = createdLink.ExpirationDate.AddDays(-30);
        var updatedLink = await referralLinkRepository.UpdateExpirationDate(createdLink.UserId, olderExpirationDate, cancellationToken);
        
        //Assert
        Assert.That(updatedLink?.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(updatedLink.BaseDeepLink, Is.EqualTo(createdLink.BaseDeepLink));
        Assert.That(updatedLink.DateCreated,Is.EqualTo(createdLink.DateCreated));
        Assert.That(updatedLink.UserId, Is.EqualTo(createdLink.UserId));
        Assert.That(updatedLink.ThirdPartyId, Is.EqualTo(createdLink.ThirdPartyId));
        Assert.That(updatedLink.ExpirationDate, Is.EqualTo(referralLink.ExpirationDate));
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

}