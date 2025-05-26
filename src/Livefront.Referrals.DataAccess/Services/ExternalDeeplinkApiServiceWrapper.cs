using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi;
using Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Livefront.Referrals.DataAccess.Services;

public class ExternalDeeplinkApiServiceWrapper : IExternalDeeplinkApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<ExternalDeeplinkApiServiceWrapper> logger;
    public ExternalDeeplinkApiServiceWrapper(HttpClient httpClient, ILogger<ExternalDeeplinkApiServiceWrapper>  logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<DeepLink> GenerateLink(string referralCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(referralCode))
        {
            logger.LogWarning("Invalid referral code {@ReferralCode}", referralCode);
            throw new ArgumentException("Referral code must not be empty", nameof(referralCode));
        }

        var generateLinkApiRequest = new CreateDeeplinkApiRequest(userReferralCode: referralCode);
        
        return await SendRequest<DeepLink, CreateDeeplinkApiRequest>(HttpMethod.Post, DeeplinkApiConstants.LinkGenerationEndpoint, generateLinkApiRequest, cancellationToken);
    }



    public async Task<DeepLink?> UpdateLinkTimeToLive(DeepLink? deepLink, CancellationToken cancellationToken)
    {
        if (deepLink == null)
        {
            logger.LogWarning("Invalid referral link {@DeepLink}", deepLink);
            throw new NullReferenceException($"{nameof(deepLink)} must not be null");
        }
        
        if (deepLink.Id <= 0)
        {
            logger.LogWarning("Invalid Id: {@Id} on deepLink of Id: {@DeepLinkId}", deepLink.Id, deepLink.Id);
            throw new ArgumentException($"{nameof(deepLink.Id)} must be greater than 0", nameof(deepLink.Id));
        }
        
        var updateRequest = new UpdateDeeplinkApiRequest(deepLink);
        
        return await SendRequest<DeepLink, UpdateDeeplinkApiRequest>(HttpMethod.Put, DeeplinkApiConstants.UpdateTimeToLiveEndpoint, updateRequest, cancellationToken);
    }

    public async Task<DeepLink?> DeleteLink(DeepLink? deepLink, CancellationToken cancellationToken)
    {
        if (deepLink == null)
        {
            logger.LogWarning("Invalid referral link {@DeepLink}", deepLink);
            throw new NullReferenceException($"{nameof(deepLink)} must not be null");
        }
        
        if (deepLink.Id <= 0)
        {
            logger.LogWarning("Invalid Id: {@Id} on deepLink of Id: {@DeepLinkId}", deepLink.Id, deepLink.Id);
            throw new ArgumentException($"{nameof(deepLink.Id)} must be greater than 0", nameof(deepLink.Id));
        }
        
        var deleteRequest = new DeleteDeeplinkApiRequest(deepLink);
        var deleteEndpoint = DeeplinkApiConstants.DeleteLinkEndpoint + $"/{deleteRequest.Id}";
        return await SendRequest<DeepLink, DeleteDeeplinkApiRequest>(HttpMethod.Delete, deleteEndpoint, deleteRequest, cancellationToken);    
    }
    
    /// <summary>
    /// This creates a more generic request so we can reuse this method in our other API calls
    /// </summary>
    /// <param name="httpMethod">The HTTP verb used to send the request</param>
    /// <param name="endpoint">The request endpoint including query parameters</param>
    /// <param name="requestData">The data to be sent in the body of the request</param>
    /// <param name="cancellationToken"> a cancellation token</param>
    /// <typeparam name="TResponse">A Generic type of the expected return type for the request</typeparam>
    /// <typeparam name="TRequest"> A Generic request type to allow different operations</typeparam>
    /// <returns>The type specified in the call</returns>
    /// <exception cref="ExternalApiServiceException"></exception>
    private async Task<TResponse> SendRequest<TResponse, TRequest>(HttpMethod httpMethod, string endpoint, TRequest? requestData, CancellationToken cancellationToken)
    {
        var contextualLogger = Log
            .ForContext<ExternalDeeplinkApiServiceWrapper>()
            .ForContext("HttpMethod", httpMethod)
            .ForContext("Endpoint", endpoint);
        
        contextualLogger.Debug("Preparing to send a {@HttpMethod} request for to {@Endpoint} with request data of {@RequestData}", httpMethod, endpoint, requestData);
        var request = new HttpRequestMessage(httpMethod, endpoint);
        // If the request uses a verb that requires a body, we serialize the request into string content to send.
        if (requestData != null && 
            (httpMethod == HttpMethod.Post || 
            httpMethod == HttpMethod.Put || 
            httpMethod == HttpMethod.Patch))
        {
            contextualLogger
                .ForContext("RequestData", requestData);
            request.Content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json"); ;
        }
        
        try
        {
            var response = await httpClient.SendAsync(request, cancellationToken);
            
            if(!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                contextualLogger
                    .ForContext("ErrorHttpResponseCode", response.StatusCode)
                    .ForContext("ErrorReasonPhrase", response.ReasonPhrase)
                    .Error(errorResponse);
                throw new ExternalApiServiceException("Failed to generate link", response.StatusCode, errorResponse );
            }
            
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            contextualLogger
                .ForContext("ErrorHttpResponseCode", exception.StatusCode)
                .Error(exception, "HttpRequestException occured while sending a request");
            throw new ExternalApiServiceException("Network issue occurred while running request.", exception);
        }
        catch (Exception exception)
        {
            contextualLogger
                .Error(exception, "An unexpected error occurred");
            throw new ExternalApiServiceException("An unexpected error occurred.", exception);
        }
    }
}