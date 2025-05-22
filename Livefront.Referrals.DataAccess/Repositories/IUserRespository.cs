using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

/// <summary>
/// A repository for retrieving user profile details that can be implemented to retrieve information from a database
/// or an internal service.
/// </summary>
public interface IUserRespository
{
    
    User GetById(Guid userId);
    
    /// <summary>
    /// Retrieve a user's referral code from user profile details using the user id
    /// </summary>
    /// <param name="userId"> The identifier of the user</param>
    /// <returns> The referral code of the identified user</returns>
    string GetReferralCodeByUserId(Guid userId);
    /// <summary>
    /// Retrieve a user by their unique referral code in profile details
    /// </summary>
    /// <param name="referralCode"> The referral code of a user</param>
    /// <returns> The user with a matching referral code</returns>
    User GetUserByReferralCode(string referralCode);
}