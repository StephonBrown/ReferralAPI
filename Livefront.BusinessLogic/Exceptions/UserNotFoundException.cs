namespace Livefront.Referrals.API.Exceptions;

public class UserNotFoundException : Exception
{
    public Guid UserId { get; private set; }
    public string ReferralCode { get; private set; }
    
    public UserNotFoundException(Guid userId) : base($"User {userId} was not found")
    {
        UserId = userId;
    }
    public UserNotFoundException(string referralCode) : base($"User with referral code {referralCode} was not found")
    {
        ReferralCode = referralCode;
    }


}