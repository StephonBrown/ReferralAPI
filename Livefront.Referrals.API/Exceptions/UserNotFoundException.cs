namespace Livefront.Referrals.API.Exceptions;

public class UserNotFoundException(Guid userId) : Exception($"User {userId} was not found")
{
    public Guid UserId { get; private set; } = userId;
}