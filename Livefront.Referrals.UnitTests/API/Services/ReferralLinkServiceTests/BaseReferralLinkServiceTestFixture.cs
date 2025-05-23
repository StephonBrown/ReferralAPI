using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Livefront.Referrals.UnitTests.API.Services.ReferralLinkServiceTests;

public class BaseReferralLinkServiceTestFixture : BaseServiceTestFixture
{
    protected IExternalDeeplinkApiService mockedExternalDeeplinkApiService =
        Substitute.For<IExternalDeeplinkApiService>();
    protected IReferralLinkRepository mockedReferralLinkRepository = Substitute.For<IReferralLinkRepository>();
    protected ILogger<IReferralLinkService> mockedLogger = Substitute.For<ILogger<IReferralLinkService>>();
    protected IReferralLinkService referralLinkService;

    protected void GivenReferralLinkRepositoryGetByUserIdReturnsNull()
    {
        mockedReferralLinkRepository
            .GetByUserId(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .ReturnsNull();
    }
    protected async Task ThenExternalDeeplinkApiServiceShouldBeCalled(int thirdPartyId, DateTime dateCreated,
        DateTime dateExpires, string link, int numberOfCalls)
    {
        await mockedExternalDeeplinkApiService
            .Received(numberOfCalls)
            .UpdateLinkTimeToLive(
                Arg.Is<DeepLink>(dl => dl.Id == thirdPartyId 
                                       && dl.DateCreated == dateCreated 
                                       && dl.ExpirationDate == dateExpires
                                       && dl.Link == link),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken ));
    }

    protected async Task ThenReferralLinkRepositoryGetByUserIdShouldBeCalled(Guid userId,  int numberOfCalls)
    {
        await mockedReferralLinkRepository
            .Received(numberOfCalls)
            .GetByUserId(Arg.Is<Guid>( id => id == userId ),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken ));
    }

    protected void GivenExternalDeeplinkApiServiceUpdateLinkTimeToLiveReturnsNull()
    {
        mockedExternalDeeplinkApiService.UpdateLinkTimeToLive(
                Arg.Any<DeepLink>(), 
                Arg.Any<CancellationToken>())
            .ReturnsNull();
    }

    protected void GivenReferralLinkRepositoryGetByUserIdReturnsReferralLink(ReferralLink referralLink)
    {
        mockedReferralLinkRepository
            .GetByUserId(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .Returns(referralLink);
    }
    
    protected void GivenExternalDeeplinkApiServiceUpdateLinkTimeToLiveReturnsDeeplink(DeepLink updatedDeepLink)
    {
        mockedExternalDeeplinkApiService.UpdateLinkTimeToLive(
                Arg.Any<DeepLink>(), 
                Arg.Any<CancellationToken>())
            .Returns(updatedDeepLink);
    }
    protected async Task ThenExternalDeeplinkApiServiceUpdateLinkTimeToLiveShouldBeCalled(int thirdPartyId,
        DateTime dateCreated, DateTime dateExpires, string link, int numberOfCalls)
    {
        await mockedExternalDeeplinkApiService
            .Received(numberOfCalls)
            .UpdateLinkTimeToLive(
                Arg.Is<DeepLink>(dl => dl.Id == thirdPartyId 
                                       && dl.DateCreated == dateCreated 
                                       && dl.ExpirationDate == dateExpires
                                       && dl.Link == link),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken ));
    }

    protected void GivenReferralLinkRepositoryUpdateExpirationDateReturnsReferralLink(ReferralLink updatedReferralLink)
    {
        mockedReferralLinkRepository
            .UpdateExpirationDate(Arg.Any<Guid>(), 
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(updatedReferralLink);
    }
    
    protected async Task ThenReferralLinkRepositoryCreateShouldBeCalled(ReferralLink referralLink, int numberOfCalls)
    {
        await mockedReferralLinkRepository
            .Received(numberOfCalls)
            .Create(
                Arg.Is<ReferralLink>(submittedReferral =>
                    submittedReferral.BaseDeepLink == referralLink.BaseDeepLink &&
                    submittedReferral.ExpirationDate == referralLink.ExpirationDate &&
                    submittedReferral.ThirdPartyId == referralLink.ThirdPartyId &&
                    submittedReferral.DateCreated == referralLink.DateCreated &&
                    submittedReferral.Id == Guid.Empty),
                cancellationToken);
    }

    protected void GivenReferralLinkRepositoryCreateReturnsReferralLink(ReferralLink referralLink)
    {
        mockedReferralLinkRepository
            .Create(Arg.Any<ReferralLink>(), 
                Arg.Any<CancellationToken>())
            .Returns(referralLink);
    }
    protected void GivenReferralLinkRepositoryCreateReturnsNull()
    {
        mockedReferralLinkRepository
            .Create(Arg.Any<ReferralLink>(),
                Arg.Any<CancellationToken>())
            .ReturnsNull();
    }

    protected void GivenExternalDeeplinkApiServiceGenerateLinkReturnsDeeplink(DeepLink deepLink)
    {
        mockedExternalDeeplinkApiService.GenerateLink(
                Arg.Any<string>(), 
                Arg.Any<CancellationToken>())
            .Returns(deepLink);
    }
    protected async Task ThenExternalDeeplinkApiServiceGenerateLinkShouldBeCalled(User user, int numberOfCalls)
    {
        await mockedExternalDeeplinkApiService
            .Received(numberOfCalls)
            .GenerateLink(
                Arg.Is<string>(referralCode => referralCode == user.ReferralCode), cancellationToken);
    }
}