using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.LinkRepositoryTests;

public class WhenTestingGetByUserId : BaseReferralLinkRepositoryTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await CreateDatabaseContext();
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
        await referralLinkRepository.Create(referralLink, cancellationToken);
        
        //Act
        var returnedReferralLink = await referralLinkRepository.GetByUserId(referralLink.UserId, cancellationToken);
        Console.WriteLine(returnedReferralLink);
        //Assert
        Assert.That(returnedReferralLink.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(returnedReferralLink.BaseDeepLink, Is.EqualTo(referralLink.BaseDeepLink));
        Assert.That(returnedReferralLink.DateCreated,Is.EqualTo(referralLink.DateCreated));
        Assert.That(returnedReferralLink.UserId, Is.EqualTo(referralLink.UserId));
        Assert.That(returnedReferralLink.ThirdPartyId, Is.EqualTo(referralLink.ThirdPartyId));
    }
    
    [Test]
    public void GivenInvalidUserId_ThenThrowsArgumentException()
    {
        //Arrange/Act/Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await referralLinkRepository.GetByUserId(Guid.Empty, cancellationToken));
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }
}