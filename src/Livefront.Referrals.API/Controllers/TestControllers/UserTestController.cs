using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers.TestControllers;

/// <summary>
/// This is a test controller to provide developers and tests with a way to
/// create and retrieve users in the test database
/// This is not intended, or recommended for production use.
/// In production use, the user should be created through the normal registration process using the user API.
/// </summary>
[ApiController]
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
    

    [HttpGet]
    [Route("get-test-user")]
    public async Task<IActionResult> GetTestUser(CancellationToken cancellationToken)
    {
        logger.LogInformation("User test endpoint called");
        var testUser = await userRepository.GetUserByReferralCode("TESTCODE", cancellationToken);
        return Ok(testUser);
    }
    
    [HttpGet]
    [Route("/{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Get test user by ID endpoint called");
        if (id == Guid.Empty)
        {
            return BadRequest("User ID cannot be empty.");
        }
        
        var testUser = await userRepository.GetById(id, cancellationToken);
        if (testUser == null)
        {
            return NotFound($"User with ID {id} not found.");
        }
        
        return Ok(testUser);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTestUser([FromBody] User? user, CancellationToken cancellationToken)
    {
        logger.LogInformation("Create test user endpoint called");
        if (user == null)
        {
            return BadRequest("User cannot be null.");
        }
        
        var createdUser = await userRepository.Create(user, cancellationToken);
        return Created($"/api/userTest/{createdUser.Id}", createdUser);
    }
    
    
}