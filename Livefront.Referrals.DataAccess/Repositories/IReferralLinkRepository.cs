using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

public interface IReferralLinkRepository
{
    Task<ReferralLink?> GetByUserId(Guid userId, CancellationToken cancellationToken);
    Task<ReferralLink> Create(ReferralLink referralLink, CancellationToken cancellationToken);
    Task<ReferralLink?> DeleteByUserId(Guid userId, CancellationToken cancellationToken);
    Task<ReferralLink?> UpdateExpirationDate(Guid userId, DateTime newExpirationDate, CancellationToken cancellationToken);
}