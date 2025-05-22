using Livefront.Referrals.API.Models;

namespace Livefront.Referrals.API.Services;

/// <summary>
/// Defines the contract for managing referrals within the system.
/// </summary>
public interface IReferralService
{
    /// <summary>
    /// Retrieves a collection of referrals associated with a specific user id.
    /// </summary>
    /// <param name="userId">The unique id of the referrer.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ReferralDTO"/> representing the referrals.</returns>
    IEnumerable<ReferralDTO> GetReferralsByUserId(Guid userId);

    /// <summary>
    /// Retrieves a collection of referrals based on a given referral code.
    /// </summary>
    /// <param name="referralCode">The referral code used to find associated referrals.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ReferralDTO"/> representing the referrals.</returns>
    IEnumerable<ReferralDTO> GetReferralsByReferralCode(string referralCode);

    /// <summary>
    /// Creates a new referral record.
    /// </summary>
    /// <param name="refereeUserId">The unique identifier of the user who was referred.</param>
    /// <param name="referralCode">The referral code that was used.</param>
    /// <returns>The newly created <see cref="ReferralDTO"/>.</returns>
    ReferralDTO CreateReferral(Guid refereeUserId, string referralCode);
    
    /// Deletes a referral record by its unique identifier.
    /// <remarks>This operation is typically restricted to administrative users.</remarks>
    /// </summary>
    /// <param name="referralId">The unique identifier of the referral to delete.</param>
    void DeleteReferral(Guid referralId);

    /// <summary>
    /// Updates an existing referral record.
    /// <remarks>This operation is typically restricted to administrative users.</remarks>
    /// </summary>
    /// <param name="referralId">The unique identifier of the referral to update.</param>
    /// <returns>The updated <see cref="ReferralDTO"/>.</returns>
    ReferralDTO UpdateReferral(Guid referralId);
}