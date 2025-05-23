using System.Net;

namespace Livefront.Referrals.DataAccess.Exceptions;

public class ExternalApiServiceException : Exception
{
    public HttpStatusCode? HttpStatusCode { get; }
    public string? ApiResponseContent{ get; }
    
    public ExternalApiServiceException(string message) : base(message)
    {
    }
    
    public ExternalApiServiceException(string message, Exception? innerException) : base(message, innerException)
    {
    }
    
    public ExternalApiServiceException(string message, HttpStatusCode statusCode, string apiResponseContent, Exception innerException = null) : base(message)
    {
        HttpStatusCode = statusCode;
        ApiResponseContent = apiResponseContent;
    }
    
}