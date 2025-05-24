using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Services.ReferralServiceTests;

public class BaseReferralServiceTestFixture : BaseServiceTestFixture
{
    protected IReferralRepository mockedReferralRepository = Substitute.For<IReferralRepository>();
    protected ILogger<IReferralService> mockedLogger = Substitute.For<ILogger<IReferralService>>();
    protected IReferralService referralService = null!;
    
    protected void GivenReferralRepositoryCreateReturnsReferral(Referral referral)
    {
        mockedReferralRepository.Create(Arg.Any<Referral>(), cancellationToken).Returns(referral);
    }
    
    protected void GivenReferralRepositoryGetReferralsByReferrerIdReturnsReferral(IEnumerable<Referral> referrals)
    {
        mockedReferralRepository.GetReferralsByReferrerId(Arg.Any<Guid>(), cancellationToken).Returns(referrals);
    }
    
    protected async Task ThenReferralRepositoryGetReferralsByReferrerIdShouldBeCalled(Guid referralId, int numberOfCalls)
    {
        await mockedReferralRepository
            .Received(numberOfCalls)
            .GetReferralsByReferrerId(Arg.Is<Guid>(id => id == referralId), 
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
    }
    protected void GivenReferralRepositoryGetReferralsByUserIdReturnEmptyIEnumerable()
    {
        mockedReferralRepository
            .GetReferralsByReferrerId(Arg.Any<Guid>(), cancellationToken)
            .Returns(Enumerable.Empty<Referral>());
    }
    
    protected void GivenReferralRepositoryCreateReturnsNull()
    {
        mockedReferralRepository.Create(Arg.Any<Referral>(), cancellationToken).ReturnsNull();
    }

    protected async Task ThenReferralRepositoryCreateShouldBeCalled(Referral referral, Guid referrerId, Guid refereeId, int numberOfCalls)
    {
        await mockedReferralRepository
            .Received(numberOfCalls)
            .Create(Arg.Is<Referral>(refer => refer.ReferrerId == referrerId &&
                                              refer.RefereeId == refereeId &&
                                              refer.Status == ReferralStatus.Complete &&
                                              refer.DateCreated != default),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
    }
    
}