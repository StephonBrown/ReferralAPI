using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Livefront.Referrals.API.Controllers.TestControllers;

/// <summary>
/// This is  a test controller to provide developers and tests with a way to
/// access the API without having to go through the full authentication process.
/// This is not intended, or recommended for production use.
/// The test user that is returned is seeded into the database for test usage.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    private readonly ILogger<AuthTestController> logger;
    private readonly IUserRepository userRepository;
    private readonly IConfiguration configuration;

    public AuthTestController(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthTestController> logger)
    {
        this.userRepository = userRepository;
        this.configuration = configuration;
        this.logger = logger;
    }
    
    /// <summary>
    /// This endpoint is used to generate a JWT access token for a test user.
    /// </summary>
    /// <param name="requestSecret"> The code to creating a token for a test user.</param>
    /// <param name="cancellationToken"> Cancellation token to cancel the operation.</param>
    /// <returns> A JWT access token for the test user.</returns>
    /// <remarks>
    /// Sample request:
    /// POST /api/authTest/get-bearer-token
    /// {
    ///     "secret_code": "code",
    /// }
    /// </remarks>
    /// <response code="200">Returns the JWT access token for the test user.</response>
    /// <response code="400">If the secret code is invalid.</response>
    /// <response code="404"> If the test user is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost]
    [Route("get-bearer-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccessToken))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetJWTBearerToken([FromBody]RequestSecret requestSecret, CancellationToken cancellationToken)
    {
        
        logger.LogInformation("Auth test endpoint called");
        if(requestSecret.SecretCode == "TEST")
        {
            var testUser = await userRepository.GetUserByReferralCode("TESTCODE", cancellationToken);
            var token = GenerateAccessToken(testUser!, requestSecret.IsEmptyUserId);
            var accessToken = new AccessToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return Ok(accessToken);
        }
        return BadRequest("Invalid secret code. Please provide the correct secret code.");
    }


    /// <summary>
    /// This method generates a JWT access token for the a test user
    /// </summary>
    /// <param name="user">the user who will use the token</param>
    /// <param name="isEmptyUserId"> if true, the user ID will be set to an empty GUID.</param>
    /// <returns>the generated JWT token</returns>
    private JwtSecurityToken GenerateAccessToken(User user, bool isEmptyUserId = false)
    {
        // Create user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Sid, isEmptyUserId ? "" : user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        // Create a JWT
        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), //Expires in an hour
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
                SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
    

}

    