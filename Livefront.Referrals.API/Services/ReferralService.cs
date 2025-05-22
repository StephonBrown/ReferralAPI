using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Repositories;

namespace Livefront.Referrals.API.Services;

public class ReferralService : IReferralService
{
    private readonly IReferralRepository referralLinkRepository;
    private readonly IUserRepository userRepository;
    
    public ReferralService(IReferralRepository referralLinkRepository, IUserRepository userRepository)
    {
        this.referralLinkRepository = referralLinkRepository;
        this.userRepository = userRepository;
    }
    public ReferralService()
    {
        
    }
    /// <inheritdoc />
    public IEnumerable<ReferralDTO> GetReferralsByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerable<ReferralDTO> GetReferralsByReferralCode(string referralCode)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ReferralDTO CreateReferral(Guid refereeUserId, string referralCode)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void DeleteReferral(Guid referralId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ReferralDTO UpdateReferral(Guid referralId)
    {
        throw new NotImplementedException();
    }
}