using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.API.Services;

public interface IReferralLinkService
{
    public ReferralLink CreateReferralLink();
    public ReferralLink GetReferralLink();
    public void DeleteReferralLink();
    public ReferralLink UpdateReferralLink();
}