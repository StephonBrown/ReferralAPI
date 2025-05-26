using Livefront.BusinessLogic.Exceptions;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Exceptions;

namespace Livefront.BusinessLogic.Services;

/// <summary>
/// Defines the contract for managing referrals within the system.
/// </summary>
public interface IReferralService
{
    /// <summary>
    /// Retrieves a collection of referrals associated with a specific user id.
    /// </summary>
    /// <param name="userId">The unique id of the referrer.</param>
    /// <param name="cancellationToken"> cancellation token to cancel the operation or timeout </param> 
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ReferralDTO"/> representing the referrals.</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is not valid.</exception>
    Task<IEnumerable<ReferralDTO>> GetReferralsByReferrerUserId(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new referral for a user who was referred by another user.
    /// </summary>
    /// <param name="refereeUserId">The unique identifier of the user who was referred.</param>
    /// <param name="referralCode">The referral code that was used.</param>
    /// <param name="cancellationToken"> cancellation token to cancel the operation or timeout </param>
    /// <returns>The newly created <see cref="ReferralDTO"/> or null</returns>
    /// <exception cref="ArgumentException">Thrown when the refereeUserId is not valid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the referralCode is null or empty.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified refereeUserId or the specified referralCode does not exist.</exception>
    /// <exception cref="DataPersistenceException">Thrown when there is an error while creating the referral.</exception>
    Task<ReferralDTO?> CreateReferral(Guid refereeUserId, string referralCode, CancellationToken cancellationToken);
    
}