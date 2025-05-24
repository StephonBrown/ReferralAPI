using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.ReferralRepositoryTests;

public class WhenGettingReferralsByReferrerId : BaseRepositoryTestFixture
{
    private IReferralRepository referralRepository = null!;
    private ILogger<IReferralRepository> logger = Substitute.For<ILogger<IReferralRepository>>();

    [SetUp]
    public async Task SetUp()
    {
        await ConfigureDbContextAsync();
        referralRepository = new ReferralRepository(dbContext, logger);
    }
    
    [Test]
    public async Task GoldenPath()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        var referral1 = new Referral
        {
            ReferrerId = referrerId,
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete,
            ReferralCode = "code1"
        };
        
        var referral2 = new Referral
        {
            ReferrerId = referrerId,
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete,
            ReferralCode = "code2"
        };
        var savedRef1 = await referralRepository.Create(referral1, cancellationToken);
        var savedRef2 = await referralRepository.Create(referral2, cancellationToken);
        
        // Act
        var referrals = await referralRepository.GetReferralsByReferrerId(referrerId, cancellationToken);
        var referralsList = referrals.ToList();
        
        // Assert
        Assert.That(referralsList.Count, Is.EqualTo(2));
        Assert.That(referralsList.All(r => r.ReferrerId == referrerId), Is.True);
        Assert.That(referralsList.Any(r => r.Id == savedRef1.Id), Is.True);
        Assert.That(referralsList.Any(r => r.Id == savedRef2.Id), Is.True);
        Assert.That(referralsList.Any(r => r.RefereeId == savedRef1.RefereeId), Is.True);
        Assert.That(referralsList.Any(r => r.RefereeId == savedRef2.RefereeId), Is.True);
        Assert.That(referralsList.Any(r => r.ReferralCode == savedRef1.ReferralCode), Is.True);
        Assert.That(referralsList.Any(r => r.ReferralCode == savedRef2.ReferralCode), Is.True);
    }
    
    [Test]
    public void GivenEmptyUserId_ThenThrowsArgumentException()
    {
        // Arrange
        var emptyUserId = Guid.Empty;
        
        // Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            await referralRepository.GetReferralsByReferrerId(emptyUserId, cancellationToken));
        Assert.That(exception!.ParamName, Is.EqualTo("userId"));
    }
    
    [Test]
    public async Task GivenNoReferrals_ThenReturnsEmptyList()
    {
        // Arrange
        var referrerId = Guid.NewGuid();
        
        // Act
        var referrals = await referralRepository.GetReferralsByReferrerId(referrerId, cancellationToken);
        
        // Assert
        Assert.That(referrals, Is.Empty);
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }
    
}