using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.BusinessLogic.Extensions;

public static class ReferralExtensions
{
    public static ReferralDTO ToReferralDto(this Referral referral, User referee)
    {
        if(referral == null)
        {
            throw new ArgumentNullException(nameof(referral), "Referral cannot be null.");
        }

        ArgumentNullException.ThrowIfNull(referee);

        if (string.IsNullOrWhiteSpace(referee.FirstName))
        {
            throw new ArgumentNullException(nameof(referee), "Referee first name cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(referee.LastName))
        {
            throw new ArgumentNullException(nameof(referee), "Referee last name cannot be null or empty.");
        }
        
        return new ReferralDTO
        {
            FirstName = referee.FirstName,
            LastName = referee.LastName,
            Status = referral.Status,
        };
        
    }
}