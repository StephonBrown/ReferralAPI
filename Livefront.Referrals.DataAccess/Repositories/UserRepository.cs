using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Livefront.Referrals.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ReferralsContext referralsContext;
    private readonly ILogger<ReferralRepository> logger;
    
    public UserRepository(ReferralsContext referralsContext, ILogger<ReferralRepository> logger)
    {
        this.referralsContext = referralsContext;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<User?> GetById(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning("User ID is empty. User ID: {UserId}", userId);
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        return await referralsContext
            .Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetByIds(IEnumerable<Guid> userIds, CancellationToken cancellationToken)
    {
        if (userIds == null || !userIds.Any())
        {
            logger.LogWarning("User IDs are null or empty. User IDs: {UserIds}", string.Join(", ", userIds));
            throw new ArgumentException("User IDs cannot be null or empty.", nameof(userIds));
        }

        return await referralsContext
            .Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> GetReferralCodeByUserId(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning("User ID is empty. User ID: {UserId}", userId);
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        var user = await referralsContext
            .Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user?.ReferralCode;
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByReferralCode(string referralCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(referralCode))
        {
            logger.LogWarning("Referral code is null or empty. Referral code: {ReferralCode}", referralCode);
            throw new ArgumentException("Referral code cannot be null or empty.", nameof(referralCode));
        }

        return await referralsContext
            .Users
            .FirstOrDefaultAsync(u => u.ReferralCode == referralCode, cancellationToken);
    }
}