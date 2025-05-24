using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

namespace Livefront.BusinessLogic.Extensions;

public static class ReferralLinkExtensions
{
    public static ReferralLinkDTO ToReferralLinkDto(this ReferralLink referralLink)
    {
        if (referralLink == null)
        {
            throw new ArgumentNullException(nameof(referralLink), "Referral link cannot be null.");
        }
        if(string.IsNullOrWhiteSpace(referralLink.BaseDeepLink))
        {
            throw new ArgumentNullException(nameof(referralLink), "Referral link cannot be null or empty.");
        }
        if(referralLink.ExpirationDate == default)
        {
            throw new ArgumentNullException(nameof(referralLink), "Referral link expiration date must be valid.");
        }
        
        return new ReferralLinkDTO
        {
            ReferralLink = referralLink.BaseDeepLink,
            ExpirationDate = referralLink.ExpirationDate,
        };
    }
    
    public static DeepLink? ToDeepLink(this ReferralLink? referralLink)
    {
        if(referralLink == null)
        {
            return null;
        }
        
        return new DeepLink
        {
            Id = referralLink.ThirdPartyId,
            Link = referralLink.BaseDeepLink,
            DateCreated = referralLink.DateCreated,
            ExpirationDate = referralLink.ExpirationDate,
        };
    }
}