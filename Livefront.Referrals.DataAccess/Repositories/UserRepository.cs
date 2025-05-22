using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

public class MockUserRepository : IUserRespository
{
    public User GetById(Guid userId)
    {
        throw new NotImplementedException();
    }

    public string GetReferralCodeByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }
    
}