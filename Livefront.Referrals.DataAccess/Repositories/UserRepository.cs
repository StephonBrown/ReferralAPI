using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Livefront.Referrals.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext referralsContext;
    private readonly ILogger<ReferralRepository> logger;
    
    public UserRepository(DbContext referralsContext, ILogger<ReferralRepository> logger)
    {
        this.referralsContext = referralsContext;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<User?> GetById(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetByIds(IEnumerable<Guid> userIds, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<string?> GetReferralCodeByUserId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByReferralCode(string referralCode, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}