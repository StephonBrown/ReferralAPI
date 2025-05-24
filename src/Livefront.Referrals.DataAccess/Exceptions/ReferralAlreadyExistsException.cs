namespace Livefront.Referrals.DataAccess.Exceptions;

public class ReferralAlreadyExistsException : DataPersistenceException
{
    public Guid ReferrerId { get; init; }
    public Guid RefereeId { get; init; }

    public ReferralAlreadyExistsException(string message) : base(message)
    {
    }

    public ReferralAlreadyExistsException(string message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public ReferralAlreadyExistsException(string message, Guid referrerId, Guid refereeId) : base(message)
    {
        ReferrerId = referrerId;
        RefereeId = refereeId;
    }

}