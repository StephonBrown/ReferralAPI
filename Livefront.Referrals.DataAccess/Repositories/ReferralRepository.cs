using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

public class MockReferralRepository : IReferralRepository
{
    
    public Referral ReferralGetById()
    {
        throw new NotImplementedException();
    }

    public Referral GetByRefereeId()
    {
        throw new NotImplementedException();
    }

    public Referral GetByReferrerId()
    {
        throw new NotImplementedException();
    }
}