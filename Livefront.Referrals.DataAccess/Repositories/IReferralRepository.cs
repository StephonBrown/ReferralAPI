using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

/// <summary>
/// The implementations of this repository are responsible for persisting and retrieving
/// referral information pertaining to connections between referrers, referees, and time surrounding referrals
/// </summary>
public interface IReferralRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="referralId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Referral> GetById(Guid referralId, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<Referral>> GetReferralsByReferrerId(Guid userId, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="referral"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Referral> Create(Referral referral, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="referral"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Referral> Delete(Referral referral, CancellationToken cancellationToken);
}