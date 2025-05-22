using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Livefront.Referrals.DataAccess.Repositories;

public class UserRepository : IUserRespository
{
    private readonly DbContext referralsContext;
    private readonly ILogger<ReferralRepository> logger;
    
    public UserRepository(DbContext referralsContext, ILogger<ReferralRepository> logger)
    {
        this.referralsContext = referralsContext;
        this.logger = logger;
    }
    
    public User GetById(Guid userId)
    {
        throw new NotImplementedException();
    }

    public string GetReferralCodeByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }

    public User GetUserByReferralCode(string referralCode)
    {
        throw new NotImplementedException();
    }
}