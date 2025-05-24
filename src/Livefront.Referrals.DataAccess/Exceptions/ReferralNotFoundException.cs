namespace Livefront.Referrals.DataAccess.Repositories;

public class ReferralNotFoundException : Exception
{
    public ReferralNotFoundException(Guid id) : base($"Referral with ID {id} not found.")
    {
    }
}