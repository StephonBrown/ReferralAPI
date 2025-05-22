namespace Livefront.Referrals.IntegrationTests.DataAccess.Repositories.LinkRepositoryTests;

public class WhenUpdatingReferralLink
{
    [SetUp]
    public async Task SetUp()
    {
        await CreateDatabaseContext();
        referralLinkRepository = new ReferralLinkRepository(dbContext, logger);
    }
    
    
    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }
}