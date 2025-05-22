using Livefront.Referrals.DataAccess;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApiRequests;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.LinkRepositoryTests;

[TestFixture]
public class WhenCreatingAReferralLink : BaseRepositoryTestFixture
{
    private IReferralLinkRepository referralLinkRepository;
    private ILogger<IReferralLinkRepository> logger = Substitute.For<ILogger<IReferralLinkRepository>>();

    [SetUp]
    public void SetUp()
    {
        dbConnection = new SqliteConnection(databaseConnectionString);
        dbConnection.Open();
        dbContextOptions = new DbContextOptionsBuilder<ReferralsContext>()
            .UseSqlite(databaseConnectionString)
            .Options;
        using var context = new ReferralsContext(dbContextOptions);
        context.Database.EnsureCreated();
        referralLinkRepository = new ReferralLinkRepository(context, logger);
    }

    [Test]
    public async Task GoldenPath()
    {
        //Arrange
        var userGuid = Guid.NewGuid();
        var deepLink = new DeepLink(50, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "https://generated-link.com?channel=default");
        
        //Act
        var createdLink = await referralLinkRepository.Create(userGuid, deepLink, cancellationToken);
        
        //Assert
        Assert.That(createdLink.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(createdLink.Link.AbsoluteUri, Is.EqualTo(deepLink.Link));
        Assert.That(createdLink.DateCreated,Is.EqualTo(deepLink.DateCreated));
        Assert.That(createdLink.UserId, Is.EqualTo(userGuid));
        Assert.That(createdLink.ThirdPartyId, Is.EqualTo(deepLink.Id));
    }

    [TearDown]
    public void TearDown()
    {
        dbConnection.Dispose();
    }


}