namespace Livefront.Referrals.DataAccess.Exceptions;

public class ReferralLinkAlreadyExistsException : DataPersistenceException
{
    public Guid UserId { get; init; }
    
    public ReferralLinkAlreadyExistsException(string message) : base(message)
    {
    }

    public ReferralLinkAlreadyExistsException(string message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public ReferralLinkAlreadyExistsException(string message, Guid userId) : base(message)
    {
        UserId = userId;
    }
}