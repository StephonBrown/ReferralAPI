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
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the <see cref="ReferralLinkDTO"/>.</returns>
    Task<ReferralLinkDTO> CreateOrRetrieveReferralLink(Guid userId);

    /// <summary>
    /// Retrieves a referral link by its user's referral code.
    /// </summary>
    /// <param name="referralCode">The referral code string.</param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the <see cref="ReferralLinkDTO"/> if found; otherwise, <see langword="null"/>.</returns>
    Task<ReferralLinkDTO> GetReferralLinkByReferralCode(string referralCode);

    /// <summary>
    /// Deletes a referral link associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <returns>A <see cref="Task"/> that represents an async function/returns>
    Task DeleteReferralLink(Guid userId);

    /// <summary>
    /// Extends the time-to-live (TTL) of a referral link for a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user.</param>
    /// <returns>A <see cref="Task"/> that represents the async operation, containing the updated <see cref="ReferralLinkDTO"/>.</returns>
    Task<ReferralLinkDTO> ExtendReferralLinkTimeToLive(Guid userId);
}