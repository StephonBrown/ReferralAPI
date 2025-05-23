using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Repositories;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Livefront.Referrals.UnitTests.API.Services.ReferralServiceTests;

public class BaseReferralServiceTestFixture : BaseServiceTestFixture
{
    protected IExternalDeeplinkApiService mockedExternalDeeplinkApiService =
        Substitute.For<IExternalDeeplinkApiService>();
    protected IReferralRepository mockedReferralRepository = Substitute.For<IReferralRepository>();
    protected ILogger<IReferralService> mockedLogger = Substitute.For<ILogger<IReferralService>>();
    protected IReferralService referralService;
    
    
}