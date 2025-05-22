using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.ReferralLinkRepositoryTests;

[TestFixture]
public class WhenDeletingByUserId : BaseRepositoryTestFixture
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
        ;
        await referralLinkRepository.Create(referralLink, cancellationToken);
        await referralLinkRepository.Create(referralLink2, cancellationToken);

        //Act
        await referralLinkRepository.DeleteByUserId(referralLink.UserId, cancellationToken);
        var getReferralLink = await referralLinkRepository.GetByUserId(referralLink.UserId, cancellationToken);
        
        //Assert
        Assert.That(getReferralLink, Is.Null);
        Assert.That(dbContext.ReferralLinks.Count(), Is.EqualTo(1));
    }
    
    [Test]
    public void GivenInvalidUserId_ThenThrowsArgumentException()
    {
        //Arrange/Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.DeleteByUserId(Guid.Empty, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("userId"));
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

}