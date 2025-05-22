namespace Livefront.Referrals.API.Persistence;

/// <summary>
/// A repository for retrieving user profile details that can be implemented to retrieve information from a database
/// or an internal service.
/// </summary>
public interface IUserRespository
{
    /// <summary>
    /// Retrieve a user's referral code from user profile details using the user id
    /// </summary>
    /// <param name="userId"> The identifier of the user</param>
    /// <returns> The referral code of the identified user</returns>
    string GetReferralCodeByUserId(Guid userId);
}