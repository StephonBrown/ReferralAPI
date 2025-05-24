using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

namespace Livefront.Referrals.DataAccess.Services;

/// <summary>
/// This interface defines a generic contract for the external deeplink API service.
/// This service is responsible for generating, updating, and deleting deferred deeplinks.
/// </summary>
public interface IExternalDeeplinkApiService
{
    /// <summary>
    /// Generates a new deferred deeplink using the provided referral code.
    /// The deeplink will be created with a default expiration time of 30 days.
    /// The referral code will be an associated parameter in the deeplink.
    /// </summary>
    /// <param name="referralCode">a unique referral code that is used to identify a user</param> 
    /// <param name="cancellationToken">a cancellation token to cancel the operation or allow timout</param> 
    /// <returns >Returns the generated deeplink</returns>
    /// <exception cref="ArgumentException">Thrown when the referralCode is not valid</exception>
    /// <exception cref="NullReferenceException">Thrown when the deeplink is null</exception>
    /// <exception cref="ExternalApiServiceException">Thrown when the deeplink cannot be generated due to third-party issues</exception>
    Task<DeepLink> GenerateLink(string referralCode, CancellationToken cancellationToken);
    /// <summary>
    /// Updates the expiration date or Time to Live(TTL) of a deferred deeplink.
    /// </summary>
    /// <param name="deepLink">the deeplink to be updated</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or allow timeout</param>
    /// <returns>Returns the deeplink with an updated expiration time</returns>
    /// <exception cref="ArgumentNullException">Thrown when the deepLink is null or the id is invalid</exception>
    /// <exception cref="ArgumentException">Thrown when the deepLink id is not valid</exception>
    /// <exception cref="ExternalApiServiceException">Thrown when the deeplink cannot be updated due to third-party issues</exception>
    /// <remarks> The deepLink must have a valid id with in line with the external API, and the default expiration date extension is 30 days.</remarks>
    Task<DeepLink?> UpdateLinkTimeToLive(DeepLink? deepLink, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a deferred deeplink on the external platform.
    /// </summary>
    /// <param name="deepLink">the deeplink to be deleted</param>
    /// <param name="cancellationToken">a cancellation token to cancel the operation or allow timeout</param>
    /// <returns>Returns the deleted deeplink</returns>
    /// <exception cref="ArgumentNullException">Thrown when the deepLink is null</exception>
    /// <exception cref="ArgumentException">Thrown when the deepLink id is not valid</exception>
    /// <exception cref="ExternalApiServiceException">Thrown when the deeplink cannot be deleted due to third-party issues</exception>
    Task<DeepLink?> DeleteLink(DeepLink? deepLink, CancellationToken cancellationToken);
}