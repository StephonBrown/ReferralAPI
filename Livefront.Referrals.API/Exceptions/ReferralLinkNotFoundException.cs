namespace Livefront.Referrals.API.Exceptions;

public class ReferralLinkNotFoundException(Guid userId): Exception($"Referral link for {userId} could not be found")
{
    public Guid UserId { get; private set; } = userId;

}