using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Livefront.Referrals.DataAccess.Services;

namespace Livefront.Referrals.API.Services;


public class ReferralLinkService : IReferralLinkService
{
    private readonly IReferralLinkRepository referralLinkRepository;
    private readonly ExternalDeeplinkApiService externalDeeplinkApiService;
    private readonly IUserRespository userRespository;
    
    public ReferralLinkService(IReferralLinkRepository referralLinkRepository, 
        IUserRespository userRespository, 
        ExternalDeeplinkApiService externalDeeplinkApiService)
    {
        this.referralLinkRepository = referralLinkRepository;
        this.userRespository = userRespository;
        this.externalDeeplinkApiService = externalDeeplinkApiService;
    }
    
    /// <inheritdoc />
    public async Task<ReferralLinkDTO> CreateOrGetReferralLink(Guid userId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<ReferralLinkDTO> ExtendReferralLinkTimeToLive(Guid userId)
    {
        throw new NotImplementedException();
    }
}