using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

namespace Livefront.Referrals.API.Extensions;

public static class ReferralLinkExtensions
{
    public static ReferralLinkDTO? ToReferralLinkDto(this ReferralLink? referralLink)
    {
        if(referralLink == null)
        {
            return null;
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