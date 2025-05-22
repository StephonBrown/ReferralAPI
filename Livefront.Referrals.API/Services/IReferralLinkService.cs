using Livefront.Referrals.API.Models;

namespace Livefront.Referrals.API.Services;

/// <summary>
/// A service for managing User ReferralLinks 
/// </summary>
public interface IReferralLinkService
{
    /// <summary>
    /// Creates a new referral link for a user or retrieves an existing one if it already exists.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the <see cref="ReferralLinkDTO"/>.</returns>
    Task<ReferralLinkDTO?> CreateOrGetReferralLink(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Extends the time-to-live (TTL) of a referral link for a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the updated <see cref="ReferralLinkDTO"/>.</returns>
    Task<ReferralLinkDTO?> ExtendReferralLinkTimeToLive(Guid userId, CancellationToken cancellationToken);
}