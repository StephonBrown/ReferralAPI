using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

/// <summary>
/// The implementations of this repository are responsible for persisting and retrieving
/// referral information pertaining to connections between referrers, referees, and time surrounding referrals
/// </summary>
public interface IReferralRepository
{
    /// <summary>
    /// Get a referral by its unique identifier
    /// </summary>
    /// <param name="referralId">the unique id of the referral</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns a unique referral with the a matching id or null</returns>
    Task<Referral?> GetById(Guid referralId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get all referrals by the a referrer's id
    /// </summary>
    /// <param name="userId">the unique user id of the referrer</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <cancellationToken>cancellation token to cancel the operation or timeout</cancellationToken>
    /// <returns>Returns a list of referrals belonging to the referrer, or an empty list when none exist</returns>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    /// <exception cref="DataPersistenceException">Thrown when the referrals cannot be retrieved</exception>
    Task<IEnumerable<Referral>> GetReferralsByReferrerId(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new unique referral
    /// </summary>
    /// <param name="referral">the referral to be persisted</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <returns>Returns the newly persisted referral</returns>
    /// <exception cref="ArgumentNullException">Thrown when the referral is null</exception>
    /// <exception cref="ArgumentException">Thrown when one of the referral properties is not valid</exception>
    /// <exception cref="DataPersistenceException">Thrown when the referral cannot be persisted</exception>
    /// <exception cref="ReferralAlreadyExistsException">Thrown when a referral already exists for the referrer and referee</exception>
    /// <remarks>a referral is marked complete when the referral is created because the refereeId is known and the referrerId is known</remarks>
    Task<Referral> Create(Referral referral, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a referral by its unique identifier
    /// </summary>
    /// <param name="referralId">the unique id of the referral</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or timeout</param>
    /// <exception cref="ArgumentException">Thrown when the referral is null</exception>
    /// <exception cref="DataPersistenceException">Thrown when the referral cannot be deleted</exception>
    Task Delete(Guid referralId, CancellationToken cancellationToken);
}