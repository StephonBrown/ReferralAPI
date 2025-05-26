using System.Text;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

namespace Livefront.Referrals.DataAccess.Services;

public class MockDeeplinkApiService : IExternalDeeplinkApiService
{
    public Task<DeepLink> GenerateLink(string referralCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(referralCode))
        {
            throw new ArgumentException("Referral code must not be empty", nameof(referralCode));
        }
        

        return Task.FromResult(new DeepLink
        {
            Id = Random.Shared.Next(1, 1000000), // Simulate a unique ID from a real deep link service
            Link= $"https://carton-caps.com/{GetRandomLinkString()}?referral_code={referralCode}",
            DateCreated = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(30)
        });
    }

    public async Task<DeepLink?> UpdateLinkTimeToLive(DeepLink? deepLink, CancellationToken cancellationToken)
    {
        if (deepLink == null)
        {
            throw new ArgumentNullException(nameof(deepLink), $"{nameof(deepLink)} must not be null");
        }

        if (deepLink.Id <= 0)
        {
            throw new ArgumentException($"{nameof(deepLink.Id)} must be greater than 0", nameof(deepLink.Id));
        }

        deepLink.ExpirationDate = deepLink.ExpirationDate.AddDays(30); // Simulate updating the expiration date

        return await Task.FromResult(deepLink);
    }

    public async Task<DeepLink?> DeleteLink(DeepLink? deepLink, CancellationToken cancellationToken)
    {
        if (deepLink == null)
        {
            throw new ArgumentNullException(nameof(deepLink), $"{nameof(deepLink)} must not be null");
        }

        if (deepLink.Id <= 0)
        {
            throw new ArgumentException($"{nameof(deepLink.Id)} must be greater than 0", nameof(deepLink.Id));
        }

        // Simulate deletion by returning null
        return await Task.FromResult<DeepLink?>(deepLink);
    }
    
    private string GetRandomLinkString()
    {
        var length = 5; // Length of the random string
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";       
        StringBuilder sb = new StringBuilder(length);
        
        for (int i = 0; i < length; i++)
        {
            sb.Append(chars[Random.Shared.Next(chars.Length)]);
        }

        return sb.ToString();

    }
}