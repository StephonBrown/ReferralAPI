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
        if(referral.Id == Guid.Empty)
        {
            throw new ArgumentException("Referral ID cannot be empty.", nameof(referral));
        }
        
        if (referee == null)
        {
            throw new ArgumentNullException(nameof(referee), "Referee cannot be null.");
        }
        if (referee.Id == Guid.Empty)
        {
            throw new ArgumentException("Referee ID cannot be empty.", nameof(referee));
        }
        
        if (string.IsNullOrWhiteSpace(referee.FirstName))
        {
            throw new ArgumentException("Referee first name cannot be null or empty.", nameof(referee));
        }
        
        if (string.IsNullOrWhiteSpace(referee.LastName))
        {
            throw new ArgumentException("Referee last name cannot be null or empty.", nameof(referee));
        }
        
        return new ReferralDTO
        {
            ReferralId = referral.Id,
            UserId = referee.Id,
            FirstName = referee.FirstName,
            LastName = referee.LastName,
            Status = referral.Status,
        };
        
    }
}