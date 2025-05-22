using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Services;

public interface IDeeplinkApiService
{
    Task<ReferralLink> GenerateLink(string referralCode, string channel);
}