using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.LinkRepositoryTests;

[TestFixture]
public class WhenGettingByUserId : BaseRepositoryTestFixture
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
            BaseDeepLink = "https://generated-link.com/AAAA",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            ThirdPartyId = 50
        };
        var referralLink2 = new ReferralLink
        {
            UserId = Guid.NewGuid(),
            BaseDeepLink = "https://generated-link.com/BBBB",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(25),
            ThirdPartyId = 13
        };
        await referralLinkRepository.Create(referralLink, cancellationToken);
        await referralLinkRepository.Create(referralLink2, cancellationToken);


        //Act
        var returnedReferralLink = await referralLinkRepository.GetByUserId(referralLink.UserId, cancellationToken);
        
        //Assert
        Assert.That(returnedReferralLink?.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(returnedReferralLink.BaseDeepLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(returnedReferralLink.DateCreated,Is.EqualTo(referralLink.DateCreated));
        Assert.That(returnedReferralLink.UserId, Is.EqualTo(referralLink.UserId));
        Assert.That(returnedReferralLink.ThirdPartyId, Is.EqualTo(referralLink.ThirdPartyId));
    }
    
    [Test]
    public void GivenInvalidUserId_ThenThrowsArgumentException()
    {
        //Arrange/Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.GetByUserId(Guid.Empty, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

}