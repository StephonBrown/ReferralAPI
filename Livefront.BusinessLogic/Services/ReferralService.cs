using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Extensions;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;

namespace Livefront.Referrals.API.Services;

public class ReferralService : IReferralService
{
    private readonly IReferralRepository referralRepository;
    private readonly IUserRepository userRepository;
    private readonly ILogger<IReferralService> logger;
    
    public ReferralService(IReferralRepository referralRepository, IUserRepository userRepository, ILogger<IReferralService> logger)
    {
        this.referralRepository = referralRepository;
        this.userRepository = userRepository;
        this.logger = logger;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<ReferralDTO>> GetReferralsByReferrerUserId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public async Task<ReferralDTO?> CreateReferral(Guid refereeUserId, string referralCode, CancellationToken cancellationToken)
    {
        if (refereeUserId == Guid.Empty)
        {
            logger.LogWarning("Referee user ID is empty. Referee user ID: {RefereeUserId}", refereeUserId);
            throw new ArgumentException("Referee user ID cannot be empty.", nameof(refereeUserId));
        }
        if (string.IsNullOrWhiteSpace(referralCode))
        {
            logger.LogWarning("Referral code is null or empty. Referral code: {ReferralCode}", referralCode);
            throw new ArgumentNullException(nameof(referralCode), "Referral code cannot be null or empty.");
        }
        
        var referrer = await userRepository.GetUserByReferralCode(referralCode, cancellationToken);
        if (referrer == null)
        {
            logger.LogWarning("Referrer not found for referral code: {ReferralCode}", referralCode);
            throw new UserNotFoundException(referralCode);
        }
        
        var referee = await userRepository.GetById(refereeUserId, cancellationToken);
        if (referee == null)
        {
            logger.LogWarning("Referee not found for user ID: {UserId}", refereeUserId);
            throw new UserNotFoundException(refereeUserId);
        }
        
        var referral = new Referral
        {
            ReferrerId = referrer.Id,
            RefereeId = referee.Id,
            ReferralCode = referralCode,
            Status = ReferralStatus.Complete,
            DateCreated = DateTime.UtcNow,
        };
        
        var createdReferral = await referralRepository.Create(referral, cancellationToken);
        
        if (createdReferral == null)
        {
            logger
                .LogError("Failed to create referral for referrer user ID: {ReferrerUd} and referee user ID {RefereeId}",
                    referrer.Id, referee.Id);
            throw new DataPersistenceException("Failed to create referral.");
        }
        
        logger.LogInformation("Referral created successfully. Referral ID: {ReferralId}", createdReferral.Id);
        return createdReferral.ToReferralDto(referee);
    }
    
    /// <inheritdoc />
    public async Task DeleteReferral(Guid referralId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}