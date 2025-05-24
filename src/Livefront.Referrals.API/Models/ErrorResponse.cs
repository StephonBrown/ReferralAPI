using System.Net;

namespace Livefront.Referrals.API.Models;

public class ErrorResponse
{
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public string? Title { get; set; }
}