using System.Security.Claims;
using Livefront.BusinessLogic.Models;
using Livefront.BusinessLogic.Services;
using Livefront.Referrals.API.Models;
using Livefront.Referrals.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralsController : ControllerBase
    {
        private readonly ILogger<ReferralsController> logger;
        private readonly IReferralService referralService;

        public ReferralsController(ILogger<ReferralsController> logger, IReferralService referralService)
        {
            this.logger = logger;
            this.referralService = referralService;
        }
        
        /// <summary>
        /// Retrieves the referrals for the authorized user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns> The list of referrals for the user or an empty list if no referrals exist.</returns>
        /// <remarks>
        /// Sample request:
        ///    GET /referrals
        /// </remarks>
        /// <response code="200">Returns the list of referrals for the user</response>
        /// <response code="400">If the request is invalid or the user is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there is an internal server error</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReferralDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async  Task<IActionResult> GetReferrals(CancellationToken cancellationToken)
        {
            logger.LogDebug("GetReferrals called");
            
            // Retrieve the user ID from the access token claims
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID not found in claims");
                throw new UnauthorizedAccessException("User ID not found in claims. Ensure the user is authenticated.");
            }

            var referrals = await referralService.GetReferralsByReferrerUserId(Guid.Parse(userId), cancellationToken);
            return Ok(referrals);
        }
        
        /// <summary>
        /// Completes a referral for a user who was referred by another user.
        /// </summary>
        /// <param name="createReferralRequest">The request containing the referee user ID and referrer's referral code.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns> The newly created referral</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /referrals
        ///     {
        ///        "refereeId": "00000000-0000-0000-0000-000000000000",
        ///        "referralCode": "referral-code"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the newly created referral</response>
        /// <response code="400">If the request is invalid or the user is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="409">If the referral already exists with the referee user ID and referrer's user ID</response>
        /// <response code="500">If there is an internal server error</response>
        ///
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferralDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CompleteReferral([FromBody]CreateReferralRequest? createReferralRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("CompleteReferral called");
            ValidateCreateReferralRequest(createReferralRequest);
            var referralDto = await referralService.CreateReferral(createReferralRequest!.RefereeId, createReferralRequest.ReferralCode, cancellationToken);
            return Ok(referralDto);
        }

        private void ValidateCreateReferralRequest(CreateReferralRequest? createReferralRequest)
        {
            if (createReferralRequest == null)
            {
                logger.LogWarning("CreateReferralRequest is null");
                throw new ArgumentNullException(nameof(createReferralRequest), "CreateReferralRequest cannot be null");
            }

            if (string.IsNullOrWhiteSpace(createReferralRequest.ReferralCode))
            {
                logger.LogWarning("Referral code is null or empty");
                throw new ArgumentException("Referral code cannot be null or empty",
                    nameof(createReferralRequest.ReferralCode));
            }

            if (createReferralRequest.RefereeId == Guid.Empty)
            {
                logger.LogWarning("Referee user ID is empty");
                throw new ArgumentException("Referee user ID cannot be empty", nameof(createReferralRequest.RefereeId));
            }
        }
    }
}
