using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Livefront.Referrals.API.Controllers;

/// <summary>
/// This is  a test controller to provide developers and tests with a way to
/// access the API without having to go through the full authentication process.
/// This is not intended, or recommended for production use.
/// The test user that is returned is seeded into the database for test usage.
/// </summary>
[ApiController]
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
    
    [AllowAnonymous]
    [HttpPost]
    [Route("get-bearer-token")]
    public async Task<IActionResult> GetJWTBearerToken([FromBody]RequestSecret requestSecret, CancellationToken cancellationToken)
    {
        
        logger.LogInformation("Auth test endpoint called");
        if(requestSecret.SecretCode == "TEST")
        {
            var testUser = await userRepository.GetUserByReferralCode("TESTCODE", cancellationToken);
            var token = GenerateAccessToken(testUser!);
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
    /// <returns>the generated JWT token</returns>
    private JwtSecurityToken GenerateAccessToken(User user)
    {
        // Create user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Sid, user.Id.ToString()),
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

    