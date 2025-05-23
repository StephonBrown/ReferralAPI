using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

/// <summary>
/// A repository for retrieving user profile details that can be implemented to retrieve information from a database
/// or an internal service.
/// </summary>
public interface IUserRepository
{
    
    /// <summary>
    /// Retrieve a user by their unique id
    /// </summary> 
    /// <param name="userId">the unique identifier of the user</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns the matching user or null if not found</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    Task<User?> GetById(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Check if a user exists in the system by their unique id
    /// </summary>
    /// <param name="userId">the unique identifier of the user</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns true if the user exists, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    Task<bool> ExistsById(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Check if a user exists in the system by their unique referral code
    /// </summary>
    /// <param name="referralCode">A unique referral code that is used to identify a user</param> 
    /// <param name="cancellationToken">cancellation token to cancel the operation or timeout</param>
    /// <exception cref="ArgumentException">Thrown when the referralCode is not valid</exception>
    /// <returns>Returns true if the user exists, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when the referralCode is not valid</exception>
    Task<bool> ExistsByReferralCode(string referralCode, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve a user's referral code from user profile details using the user id
    /// </summary>
    /// <param name="userId"> The identifier of the user</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param> 
    /// <returns> The referral code of the identified user</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    Task<string?> GetReferralCodeByUserId(Guid userId,  CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieve a user by their unique referral code in profile details
    /// </summary>
    /// <param name="referralCode"> The referral code of a user</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param> 
    /// <returns> The user with a matching referral code</returns>
    /// <exception cref="ArgumentException">Thrown when the referralCode is not valid</exception>
    Task<User?> GetUserByReferralCode(string referralCode, CancellationToken cancellationToken);
}