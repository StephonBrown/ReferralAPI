using Livefront.BusinessLogic.Exceptions;
using Livefront.BusinessLogic.Extensions;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace Livefront.BusinessLogic.Services;

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
    public async Task<IEnumerable<ReferralDTO>> GetReferralsByReferrerUserId(Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning("Referrer user ID is empty. Referee user ID: {RefereeUserId}", userId);
            throw new ArgumentException("Referrer user ID cannot be empty.", nameof(userId));
        }
        
        var referrer = await userRepository.GetById(userId, cancellationToken);
        if (referrer == null)
        {
            logger.LogWarning("Referrer not found for user ID: {UserId}", userId);
            throw new UserNotFoundException(userId);
        }

        var referrals = await referralRepository
            .GetReferralsByReferrerId(referrer.Id, cancellationToken);
        var referralsList = referrals.ToList();
        if ( referralsList.Count == 0)
        {
            logger.LogWarning("No referrals found for user ID: {UserId}", userId);
            return Enumerable.Empty<ReferralDTO>();
        }
        
        var refereeIds = referralsList.Select(r => r.RefereeId).Distinct().ToList();
        var referees = await userRepository.GetByIds(refereeIds, cancellationToken);
        var refereesList = referees.ToList();
        
        // An unlikely case when a referral may exist but the user is not found in the database (They've been deleted or deactivated)
        if (refereesList.Count == 0)
        {
            logger.LogWarning("No referees found for user ID: {UserId}", userId);
            return Enumerable.Empty<ReferralDTO>();
        }
        
        logger
            .LogDebug("{NumberOfRefer} Referrals found for user ID: {UserId}",
                referralsList.Count, 
                userId);
        
        // Map each referral to its corresponding referee
        var referralDtos = new List<ReferralDTO>();
        foreach (var referral in referralsList)
        {
            var referee = refereesList.FirstOrDefault(r => r.Id == referral.RefereeId);
            if (referee != null)
            {
                referralDtos.Add(referral.ToReferralDto(referee));
            }
        }

        return referralDtos;
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
        
        if(referrer.Id == referee.Id)
        {
            logger.LogWarning("Referrer and referee cannot be the same user. Referrer ID: {ReferrerId}, Referee ID: {RefereeId}", referrer.Id, referee.Id);
            throw new ArgumentException("Referrer and referee cannot be the same user.", nameof(refereeUserId));
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