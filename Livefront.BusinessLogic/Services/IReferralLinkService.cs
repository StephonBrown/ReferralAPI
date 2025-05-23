using Livefront.Referrals.API.Models;

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
    Task<ReferralLinkDTO?> CreateReferralLink(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves the referral link for a specific user if it exists, or returns null if it doesn't.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed or on timeout.</param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the <see cref="ReferralLinkDTO"/>.</returns>
    Task<ReferralLinkDTO?> GetReferralLink(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Extends the time-to-live(TTL) of a referral link for a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed or on timeout.</param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the updated <see cref="ReferralLinkDTO"/>.</returns>
    Task<ReferralLinkDTO?> ExtendReferralLinkTimeToLive(Guid userId, CancellationToken cancellationToken);
}