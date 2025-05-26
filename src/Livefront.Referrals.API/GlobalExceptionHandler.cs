using System.Net;
using Livefront.BusinessLogic.Exceptions;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
        var problemDetails = CreateProblemDetailsBasedOnException(exception);
        httpContext.Response.StatusCode = (int)problemDetails.Status!;
        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private ProblemDetails CreateProblemDetailsBasedOnException(Exception exception)
    {
        var problemDetails = new ProblemDetails();
        
        switch (exception)
        {
            case UserNotFoundException userNotFoundException:
                problemDetails.Detail = userNotFoundException.Message;
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "User Not Found";
                break;
            case ReferralLinkAlreadyExistsException:
            case ReferralAlreadyExistsException:
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Title = "Conflict";
                problemDetails.Detail = exception.Message;
                break;
            case ExternalApiServiceException:
                problemDetails.Status = (int)HttpStatusCode.BadGateway;
                problemDetails.Title = "Bad Gateway";
                problemDetails.Detail = "Unable to connect to required services";
                break;
            case ArgumentException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = exception.GetType().Name;
                problemDetails.Detail = exception.Message;
                break;
            case UnauthorizedAccessException:
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = "You are not authorized to access this resource";
                break;
            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "Internal Server Error";
                break;
        }
        
        return problemDetails;
    }
}