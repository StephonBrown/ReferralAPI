using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Extensions;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Livefront.Referrals.DataAccess.Services;

namespace Livefront.Referrals.API.Services;

public class ReferralLinkService : IReferralLinkService
{
    private readonly IReferralLinkRepository referralLinkRepository;
    private readonly IExternalDeeplinkApiService externalDeeplinkApiService;
    private readonly IUserRepository userRepository;
    private readonly ILogger<IReferralLinkService> logger;
    private IReferralLinkService _referralLinkServiceImplementation;

    public ReferralLinkService(IReferralLinkRepository referralLinkRepository, 
        IUserRepository userRepository, 
        IExternalDeeplinkApiService externalDeeplinkApiService, ILogger<IReferralLinkService> logger)
    {       
        this.referralLinkRepository = referralLinkRepository;
        this.userRepository = userRepository;
        this.externalDeeplinkApiService = externalDeeplinkApiService;
        this.logger = logger;
    }
    /// <inheritdoc />
    public async Task<ReferralLinkDTO?> CreateReferralLink(Guid userId, CancellationToken cancellationToken)
    {
        var user = await ValidateUserIdAndReturnUser(userId, cancellationToken);

        var referralLink = await referralLinkRepository.GetByUserId(user.Id, cancellationToken);
        
        if (referralLink == null)
        {
            var deepLink = await externalDeeplinkApiService.GenerateLink(user.ReferralCode, cancellationToken);
            if (deepLink == null)
            {
                logger.LogWarning("Generated deeplink was not returned");
                throw new ExternalApiServiceException("Error while creating deep link with external service");
            }
            
            logger.LogDebug("Deep link generated. Creating new referral link");
            
            var newReferralLink = new ReferralLink
            {
                UserId = user.Id,
                DateCreated = deepLink.DateCreated,
                ExpirationDate = deepLink.ExpirationDate,
                ThirdPartyId = deepLink.Id,
                BaseDeepLink = deepLink.Link
            };
            
            await referralLinkRepository.Create(newReferralLink, cancellationToken);
            
            logger.LogDebug("Referral link created for user {UserId}", userId);
            return newReferralLink.ToReferralLinkDto();
        }

        return referralLink.ToReferralLinkDto();
    }
    

    public async Task<ReferralLinkDTO?> GetReferralLink(Guid userId, CancellationToken cancellationToken)
    {
        var user = await ValidateUserIdAndReturnUser(userId, cancellationToken);

        var referralLink = await referralLinkRepository.GetByUserId(user!.Id, cancellationToken);
        return referralLink.ToReferralLinkDto();
    }

    /// <inheritdoc />
    public async Task<ReferralLinkDTO?> ExtendReferralLinkTimeToLive(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning($"User {userId} has not been provided");
            throw new ArgumentException("User id cannot be empty", nameof(userId));
        }
        var user = await userRepository.GetById(userId, cancellationToken);
        
        if (user == null)
        {
            logger.LogWarning($"User {userId} does not exist");
            throw new UserNotFoundException(userId);
        }
        
        //Let's check if the referral code exists and return it if it does.
        var referralLink = await referralLinkRepository.GetByUserId(user.Id, cancellationToken);

        if (referralLink == null)
        {
            logger.LogWarning("Referral link for {UserId} could not be found", userId);
            throw new ReferralLinkNotFoundException(userId);
        }

        var deepLink = referralLink.ToDeepLink();
        var updatedDeeplink = await externalDeeplinkApiService.UpdateLinkTimeToLive(deepLink, cancellationToken);
        if (updatedDeeplink == null)
        {
            logger.LogWarning("Deep link update was not returned.");
            throw new ExternalApiServiceException("Error while updating deep link");
        }
        
        logger.LogDebug("Deep link updated. Updating new referral link");
        var updatedReferralLink = await referralLinkRepository.UpdateExpirationDate(user.Id, updatedDeeplink.ExpirationDate, cancellationToken);

        if (updatedReferralLink == null)
        {
            logger.LogWarning("Referral link update was not returned.");
            throw new DataPersistenceException("Error while updating referral link");
        }
        return updatedReferralLink.ToReferralLinkDto(); 
    }
    
    private async Task<User?> ValidateUserIdAndReturnUser(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning($"User {userId} has not been provided");
            throw new ArgumentException("User id cannot be empty", nameof(userId));
        }

        var user = await userRepository.GetById(userId, cancellationToken);

        if (user == null)
        {
            logger.LogWarning($"User {userId} does not exist");
            throw new UserNotFoundException(userId);
        }

        return user;
    }
}