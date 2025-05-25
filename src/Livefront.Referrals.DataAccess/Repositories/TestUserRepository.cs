using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Livefront.Referrals.DataAccess.Repositories;

/// <remarks> This is a test user  repository.In a real-world scenario, this would already exist as a service and we would user the interface as a wrapper</remarks>
public class TestUserRepository : IUserRepository
{
    private readonly ReferralsContext referralsContext;
    private readonly ILogger<ReferralRepository> logger;
    
    public TestUserRepository(ReferralsContext referralsContext, ILogger<ReferralRepository> logger)
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

    public async Task<User> Create(User user, CancellationToken cancellationToken)
    {
        ValidateUser(user);
        await referralsContext.Users.AddAsync(user, cancellationToken);
        await referralsContext.SaveChangesAsync(cancellationToken);

        return user;
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
    
    private void ValidateUser(User user)
    {
        if (user == null)
        {
            logger.LogWarning("User is null. User: {User}", user);
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            logger.LogWarning("User email is null or empty. User: {User}", user);
            throw new ArgumentException("User email cannot be null or empty.", nameof(user.Email));
        }
        if (string.IsNullOrWhiteSpace(user.ReferralCode))
        {
            logger.LogWarning("User referral code is null or empty. User: {User}", user);
            throw new ArgumentException("User referral code cannot be null or empty.", nameof(user.ReferralCode));
        }
        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            logger.LogWarning("User first name is null or empty. User: {User}", user);
            throw new ArgumentException("User first name cannot be null or empty.", nameof(user.FirstName));
        }
        if (string.IsNullOrWhiteSpace(user.LastName))
        {
            logger.LogWarning("User last name is null or empty. User: {User}", user);
            throw new ArgumentException("User last name cannot be null or empty.", nameof(user.LastName));
        }
    }
}