using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

/// <summary>
/// The implementations of this repository are responsible for managing
/// referral links which are deferred deep links that are used to track referrals
/// </summary>
public interface IReferralLinkRepository
{
    /// <summary>
    /// Get a referral link by its unique identifier
    /// </summary>
    /// <param name="userId">the unique identifier of the user</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns the unique referral link belonging to the user or null if not found</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    Task<ReferralLink?> GetByUserId(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Persists a new unique referral link to the database
    /// </summary>
    /// <param name="referralLink">the referral link to be persisted</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns the newly persisted referral link belonging to the user</returns>
    /// <exception cref="ArgumentNullException">Thrown when the referralLink is null</exception>
    /// <exception cref="ArgumentException">Thrown when one of the referralLink properties is not valid</exception>
    /// <exception cref="DataPersistenceException">Thrown when the referralLink cannot be persisted</exception>
    /// <exception cref="ReferralLinkAlreadyExistsException">Thrown when a user's unique referralLink already exists</exception>
    Task<ReferralLink> Create(ReferralLink referralLink, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a referral link by its unique identifier
    /// </summary>
    /// <param name="userId">the unique id of the user</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns the deleted referral link </returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    /// <exception cref="DataPersistenceException">Thrown when the referralLink cannot be deleted</exception>
    Task<ReferralLink?> DeleteByUserId(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates the expiration date or Time to Live(TTL) of a referral link
    /// </summary>
    /// <param name="userId">the unique id of the user</param>
    /// <param name="newExpirationDate">the new expiration date of the referral link</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns the updated referral link</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty or when the newExpirationDate is in the past </exception> 
    /// <exception cref="DataPersistenceException">Thrown when the referralLink cannot be updated</exception>
    Task<ReferralLink?> UpdateExpirationDate(Guid userId, DateTime newExpirationDate, CancellationToken cancellationToken);
}