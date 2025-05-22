using Livefront.Referrals.DataAccess.Models.DeeplinkApi;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

namespace Livefront.Referrals.DataAccess.Services;

public interface IExternalDeeplinkApiService
{
    Task<DeepLink?> GenerateLink(string referralCode, string channel, CancellationToken cancellationToken);
    Task<DeepLink?> UpdateLinkTimeToLive(DeepLink? deepLink, CancellationToken cancellationToken);
    Task<DeepLink?> DeleteLink(DeepLink? deepLink, CancellationToken cancellationToken);
}