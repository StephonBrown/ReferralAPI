using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Extensions;
using Livefront.Referrals.API.Models;
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
    public async Task<ReferralLinkDTO> CreateOrGetReferralLink(Guid userId, CancellationToken cancellationToken)
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
        
        //If it doesn't exist, let's create a new one
        if (referralLink == null)
        {
            var deepLink = await externalDeeplinkApiService.GenerateLink(user.ReferralCode, cancellationToken);
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
            return newReferralLink.ReferralLinkToReferralLinkDto();
        }

        return referralLink.ReferralLinkToReferralLinkDto();

    }

    /// <inheritdoc />
    public async Task<ReferralLinkDTO> ExtendReferralLinkTimeToLive(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}