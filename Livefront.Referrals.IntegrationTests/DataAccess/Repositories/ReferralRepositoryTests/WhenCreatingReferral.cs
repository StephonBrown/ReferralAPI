using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.ReferralRepositoryTests;

public class WhenCreatingReferral : BaseRepositoryTestFixture
{
    private IReferralRepository referralRepository;
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
        //Arrange

        var referee = Guid.NewGuid();
        var referrer = Guid.NewGuid();
        
        var referral = new Referral
        {
            DateCreated = DateTime.UtcNow,
            RefereeId = referee,
            ReferrerId = referrer,
            Status = ReferralStatus.Complete,
            ReferralCode = "acodeforyou"
        };
        
        //Act
        var createdReferral = await referralRepository.Create(referral, cancellationToken);
        
        //Assert
        Assert.That(createdReferral.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(createdReferral.RefereeId, Is.EqualTo(referral.RefereeId));
        Assert.That(createdReferral.DateCreated,Is.EqualTo(referral.DateCreated));
        Assert.That(createdReferral.ReferrerId, Is.EqualTo(referral.ReferrerId));
        Assert.That(createdReferral.Status, Is.EqualTo(ReferralStatus.Complete));
    }
    
       
    [Test]
    public async Task GivenReferralExists_And_WeAddAReferralWithTheSameReferrerAndReferee_ThenThrowsReferralAlreadyExistsException()
    {
        //Arrange
        var referee = Guid.NewGuid();
        var referrer = Guid.NewGuid();
        
        var referral = new Referral
        {
            DateCreated = DateTime.UtcNow,
            RefereeId = referee,
            ReferrerId = referrer,
            Status = ReferralStatus.Complete,
            ReferralCode = "acodeforyou"
        };
        
        var referral2 = new Referral
        {
            DateCreated = DateTime.UtcNow.AddDays(5),
            RefereeId = referee,
            ReferrerId = referrer,
            Status = ReferralStatus.Complete,
            ReferralCode = "acodeforyou"
        };
        
        //Act
        await referralRepository.Create(referral, cancellationToken);
        
        //Assert
        var exception = Assert.ThrowsAsync<ReferralAlreadyExistsException>(async () => await referralRepository.Create(referral2, cancellationToken));
        Assert.That(exception.InnerException, Is.TypeOf<DbUpdateException>());
    }
    
    [Test]
    public void GivenEmptyReferrerId_ThenThrowsArgumentException()
    {
        //Arrange
        var referrer = Guid.Empty;
        var referee = Guid.NewGuid();
        
        var referral = new Referral
        {
            DateCreated = DateTime.UtcNow,
            RefereeId = referee,
            ReferrerId = referrer,
            Status = ReferralStatus.Complete,
            ReferralCode = "acodeforyou"
        };
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralRepository.Create(referral, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referral"));
        
    }
    
    [Test]
    public void GivenEmptyRefereeId_ThenThrowsArgumentException()
    {
        //Arrange
        var referrer = Guid.NewGuid();
        var referee = Guid.Empty;
        
        var referral = new Referral
        {
            DateCreated = DateTime.UtcNow,
            RefereeId = referee,
            ReferrerId = referrer,
            Status = ReferralStatus.Complete,
            ReferralCode = "acodeforyou"
        };
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralRepository.Create(referral, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referral"));
        
    }
    
    [Test]
    public void GivenDateCreatdIsInvalid_ThenThrowsArgumentException()
    {
        //Arrange
        var referrer = Guid.NewGuid();
        var referee = Guid.NewGuid();
        
        var referral = new Referral
        {
            DateCreated = default,
            RefereeId = referee,
            ReferrerId = referrer,
            Status = ReferralStatus.Complete,
            ReferralCode = "acodeforyou"
        };
        
        //Act/Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => await referralRepository.Create(referral, cancellationToken));
        Assert.That(exception.ParamName, Is.EqualTo("referral"));
    }
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }
}