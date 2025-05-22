using System.Net;
using Livefront.Referrals.API.Exceptions;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace Livefront.Referrals.API;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var errorResponse = CreateErrorResponseBasedOnException(exception);
        httpContext.Response.StatusCode = errorResponse.StatusCode;
        await httpContext
            .Response
            .WriteAsJsonAsync(errorResponse, cancellationToken);
        return true;
    }

    private ErrorResponse CreateErrorResponseBasedOnException(Exception exception)
    {
        var errorResponse = new ErrorResponse();
        
        switch (exception)
        {
            case UserNotFoundException userNotFoundException:
                errorResponse.Message = userNotFoundException.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Title = "User Not Found";
                break;
            case ReferralLinkAlreadyExistsException:
                errorResponse.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Title = "Conflict";
                errorResponse.Message = "Referral link already exists.";
                break;
            case ExternalApiServiceException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadGateway;
                errorResponse.Title = "Bad Gateway";
                errorResponse.Message = "Unable to connect to required services";
                break;
            case ArgumentException:
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = exception.GetType().Name;
                errorResponse.Message = exception.Message;
                break;
            default:
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Title = "Internal Server Error";
                errorResponse.Message = "Internal Server Error";
                break;
        }
        
        return errorResponse;
    }
}