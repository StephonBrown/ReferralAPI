using System.Security.Claims;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers
{
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
        
        [Authorize]
        [HttpGet]
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
        
        [Authorize]
        [HttpPost]
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

        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateExpirationDate(CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateReferral called");
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID not found in claims");
                throw new UnauthorizedAccessException("User ID not found in claims. Ensure the user is authenticated.");
            }
            var referralLink = await referralLinkService.ExtendReferralLinkTimeToLive(Guid.Parse(userId), cancellationToken);
            return Ok(referralLink);
        }
    }
}
