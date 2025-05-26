using System.Security.Claims;
using Livefront.BusinessLogic.Models;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers
{
    /// <summary>
    /// Controller for managing referral links.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralLinksController : ControllerBase
    {
        private readonly ILogger<ReferralLinksController> logger;
        private readonly IUserRepository userRepository;
        private readonly IReferralLinkService referralLinkService;

        public ReferralLinksController(ILogger<ReferralLinksController> logger, IUserRepository userRepository, IReferralLinkService referralLinkService)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.referralLinkService = referralLinkService;
        }
        
        /// <summary>
        /// Retrieves the Referral Link for the authorized user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>The referral link for the user</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /referrallinks
        ///
        /// </remarks>
        /// <response code="200">Returns the referral link for the user</response>
        /// <response code="400">If the item is invalid or the user is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user or referral link is not found</response>
        /// <response code="500">If there is an internal server error</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferralLinkDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetReferralLink(CancellationToken cancellationToken)
        {
            logger.LogInformation("GetReferralLink called");
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID not found in claims");
                throw new UnauthorizedAccessException("User ID not found in claims. Ensure the user is authenticated.");
            }
            var referralLink = await referralLinkService.GetReferralLink(Guid.Parse(userId), cancellationToken);
            return Ok(referralLink);
        }
        
        /// <summary>
        /// Creates a Referral Link for the authorized user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>The created or existing referral link</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /referrallinks
        ///
        /// </remarks>
        /// <response code="200">Returns the newly created referral link</response>
        /// <response code="400">If the item is invalid or the user is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="409">If the referral link already exists</response>
        /// <response code="404">If the user or referral link is not found</response>
        /// <response code="500">If there is an internal server error</response>
        /// <response code="502">If there is a bad gateway error with the external service</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferralLinkDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status502BadGateway, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateReferralLink(CancellationToken cancellationToken)
        {
            logger.LogInformation("CreateReferral called");
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID not found in claims");
                throw new UnauthorizedAccessException("User ID not found in claims. Ensure the user is authenticated.");
            }
            
            var referralLink = await referralLinkService.CreateReferralLink(Guid.Parse(userId), cancellationToken);
            return Ok(referralLink);
        }

        /// <summary>
        /// Updates the expiration date of the referral link for the authorized user.
        /// This is an administrative action to extend the lifetime of the referral link for a user
        /// It will likely be used by a service or admin user to ensure that referral links remain valid for a longer period.
        ///
        /// </summary>
        /// <param name="userId">The ID of the user whose referral link is being updated</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>The updated referral link</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /referrallinks/{userId}
        /// 
        /// </remarks>
        /// <response code="200">Returns the referral link for the user</response>
        /// <response code="400">If the item is invalid or the user is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user or referral link is not found</response>
        /// <response code="500">If there is an internal server error</response>
        /// <response code="502">If there is a bad gateway error with the external service</response>
        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferralLinkDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status502BadGateway, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateExpirationDate(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateExpirationDate called");
            if (userId == Guid.Empty)
            {
                logger.LogWarning("User ID is empty");
                throw new ArgumentException("User ID must not be empty", nameof(userId));
            }
            var referralLink = await referralLinkService.ExtendReferralLinkTimeToLive(userId, cancellationToken);
            return Ok(referralLink);
        }
    }
}
