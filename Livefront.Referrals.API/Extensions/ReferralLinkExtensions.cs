using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.API.Extensions;

public static class ReferralLinkExtensions
{
    public static ReferralLinkDTO ReferralLinkToReferralLinkDto(this ReferralLink referralLink)
    {
        return new ReferralLinkDTO()
        {
            ReferralLink = referralLink.BaseDeepLink,
            ExpirationDate = referralLink.ExpirationDate,
        };
    }
}