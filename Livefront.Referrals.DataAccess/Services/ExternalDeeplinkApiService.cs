using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.DataAccess.Services;

public class DeeplinkApiService : IDeeplinkApiService
{
    public Task<ReferralLink> GenerateLink(string referralCode, string channel)
    {
        throw new NotImplementedException();
    }
}