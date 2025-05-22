using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Repositories;

public interface ILinkRepository
{
    Task<ReferralLink> GetLinkByUserId(Guid userId);
    Task<ReferralLink> Create(Guid userId, Uri linkUrl);
    Task<ReferralLink> Update(Guid userId);
}