using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.API.Extensions;

public static class ReferralExtensions
{
    public static ReferralDTO? ToReferralDto(this Referral? referral, User referee)
    {
        if(referral == null)
        {
            return null;
        }
        
        if(referee == null)
        {
            throw new ArgumentNullException(nameof(referee));
        }

        if (string.IsNullOrWhiteSpace(referee.FirstName))
        {
            throw new ArgumentNullException(nameof(referee.FirstName), "Referee first name cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(referee.LastName))
        {
            throw new ArgumentNullException(nameof(referee.LastName), "Referee last name cannot be null or empty.");
        }
        
        return new ReferralDTO
        {
            FirstName = referee.FirstName,
            LastName = referee.LastName,
            Status = referral.Status,
        };
        
    }
}