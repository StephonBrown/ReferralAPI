using Livefront.BusinessLogic.Exceptions;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Exceptions;

namespace Livefront.Referrals.API.Services;

/// <summary>
/// A service for managing referral links
/// </summary>
public interface IReferralLinkService
{
    /// <summary>
    /// Creates a new referral link for a user or returns an existing one if it already exists.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed or on timeout.</param> 
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the <see cref="ReferralLinkDTO"/>.</returns>
    /// <exception cref="DataPersistenceException">Thrown when there is an error while creating the referral link.</exception>
    /// <exception cref="ExternalApiServiceException">Thrown when there is an error while generating the referral link with the external service.</exception>
    /// <exception cref="ArgumentException">Thrown when the userId is not valid.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified userId does not exist.</exception>
    Task<ReferralLinkDTO> CreateReferralLink(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves the referral link for a specific user if it exists, or returns null if it doesn't.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed or on timeout.</param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the <see cref="ReferralLinkDTO"/>.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified userId does not exist.</exception>
    /// <exception cref="DataPersistenceException">Thrown when there is an error while retrieving the referral link.</exception>
    /// <exception cref="ReferralLinkNotFoundException">Thrown when the referral link for the specified userId does not exist.</exception>
    /// <exception cref="ArgumentException">Thrown when the userId is not valid.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified userId does not exist.</exception>
    Task<ReferralLinkDTO> GetReferralLink(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Extends the time-to-live(TTL) of a referral link for a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed or on timeout.</param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the updated <see cref="ReferralLinkDTO"/>.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified userId does not exist.</exception>
    /// <exception cref="DataPersistenceException">Thrown when there is an error while retrieving the referral link.</exception>
    /// <exception cref="ReferralLinkNotFoundException">Thrown when the referral link for the specified userId does not exist.</exception>
    /// <exception cref="ArgumentException">Thrown when the userId is not valid.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user with the specified userId does not exist.</exception>
    /// <exception cref="ExternalApiServiceException">Thrown when there is an error while extending the referral link with the external service.</exception>
    Task<ReferralLinkDTO?> ExtendReferralLinkTimeToLive(Guid userId, CancellationToken cancellationToken);
}