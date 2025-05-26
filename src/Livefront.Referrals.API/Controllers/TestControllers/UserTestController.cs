using Livefront.BusinessLogic.Extensions;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers.TestControllers;

/// <summary>
/// This is a test controller to provide developers and tests with a way to
/// create and retrieve users in the test database
/// This is not intended, or recommended for production use.
/// In production use, the user should be created through the normal registration process using the user API.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class UserTestController : ControllerBase
{
    private readonly ILogger<UserTestController> logger;
    private readonly IUserRepository userRepository;
    private readonly IConfiguration configuration;

    public UserTestController(IUserRepository userRepository, IConfiguration configuration, ILogger<UserTestController> logger)
    {
        this.userRepository = userRepository;
        this.configuration = configuration;
        this.logger = logger;
    }
    
    /// <summary>
    /// Retrieves a test user by their referral code.
    /// This is intended for testing purposes only.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// GET /api/userTest/get-test-user
    /// </remarks>
    /// <returns>A test user DTO with the referral code "TESTCODE".</returns>
    /// <response code="200">Returns the test user.</response>
    /// <response code="404">If the test user is not found.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpGet]
    [Route("get-test-user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetTestUser(CancellationToken cancellationToken)
    {
        logger.LogInformation("User test endpoint called");
        var testUser = await userRepository.GetUserByReferralCode("TESTCODE", cancellationToken);
        return Ok(testUser!.ToUserDTO());
    }
    
    /// <summary>
    /// Creates a user in the database.
    /// </summary>
    /// <param name="user"> The user to create.</param>
    /// <param name="cancellationToken"> Cancellation token to cancel the operation.</param>
    /// <returns> The created user DTO.</returns>
    /// <remarks>
    /// Sample request:
    /// POST /api/userTest/create-user
    /// {
    ///     "first_name": "Test",
    ///     "last_name": "User",
    ///     "referral_code": "ANOTHERCODE",
    ///     "email": "user@user.com"
    /// }
    /// </remarks>
    /// <response code="201">Returns the created user.</response>
    /// <response code="400">If the user is null or invalid.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))] 
    public async Task<IActionResult> CreateUser([FromBody] User? user, CancellationToken cancellationToken)
    {
        logger.LogInformation("Create test user endpoint called");
        if (user == null)
        {
            return BadRequest("User cannot be null.");
        }
        
        var createdUser = await userRepository.Create(user, cancellationToken);
        return Created($"/api/userTest/{createdUser.Id}", createdUser.ToUserDTO());
    }
    
    
}